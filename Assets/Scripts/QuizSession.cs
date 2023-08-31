using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public enum QuestionType
{
    Feature,
    Shiny,
    Blur,
    Anagram,
    Draw,
    Footprint,
    Team
}

[System.Serializable]
public enum PokemonGen
{
    GEN1,
    GEN2,
    GEN3,
    GEN4,
    GEN5,
    GEN6,
    GEN7,
    GEN8,
    GEN9
}

public enum NextStepButtonState
{
    NextStep,
    Pause,
    Play,
    Empty
}

public class QuizUtils
{
    public static bool validateQuestionObjects = false;
    public static int[] pokemonGenCutoffs = new int[] { 151, 251, 386, 493, 649, 721, 809, 905, 1018 };
}

public class IQuestion : ScriptableObject
{
    public string pokemonName;
    public PokemonGen pokemonGen;
}

public interface IQuestionController {
    void SetData(IQuestion questionData);
    void StartQuestion();
    void NextQuestionStep();
    void ShowSolution();
    void ResetDisplay();
}

[System.Serializable]
public class RoomUpdateResponse
{
    public string room;
    public bool pressed;
    public string pressed_by;
    public List<RoomUser> users;
}

[System.Serializable]
public class RoomUser
{
    public string id;
    public string name;
    public string room;
    public bool isAdmin;
}

[System.Serializable]
public class TextfieldUpdateResponse
{
    public string name;
    public string text;
}


public class QuizSession : MonoBehaviour
{
    public static QuizSession instance;

    [SerializeField] private List<Image> backgroundImages;
    [SerializeField] private Sprite winningBackgroundSprite;

    private FileDataHandler fileDataHandler;

    public Dictionary<QuestionType, List<IQuestion>> questionsDict;
    private Dictionary<QuestionType, int> currentQuestionIndices = new();
    private QuestionType selectedCategory = QuestionType.Feature;

    private AdminPanelUser adminPanelUser;
    private bool adminPanelConnected = false;

    [SerializeField] private QuizSettings settings;

    [SerializeField] private GameObject playerPanels;
    [SerializeField] private GameObject playerPanelPrefab;

    [SerializeField] private GameObject questionContainer;
    [SerializeField] private GameObject buzzerServerSetupPromptPrefab;
    [SerializeField] private GameObject statusPrefab;
    private TMP_Text statusText;
    private bool statusLoopActive;

    [SerializeField] private GameObject featureQuestionPrefab;
    [SerializeField] private GameObject shinyQuestionPrefab;
    [SerializeField] private GameObject blurQuestionPrefab;
    [SerializeField] private GameObject anagramQuestionPrefab;
    [SerializeField] private GameObject drawQuestionPrefab;
    [SerializeField] private GameObject footprintQuestionPrefab;
    [SerializeField] private GameObject teamQuestionPrefab;

    [SerializeField] private GameObject explainScreenPrefab;
    [SerializeField] private GameObject endScreenPrefab;

    [SerializeField] private QuestionDB featureQuestionDB;
    [SerializeField] private QuestionDB shinyQuestionDB;
    [SerializeField] private QuestionDB blurQuestionDB;
    [SerializeField] private QuestionDB anagramQuestionDB;
    [SerializeField] private QuestionDB drawQuestionDB;
    [SerializeField] private QuestionDB footprintQuestionDB;
    [SerializeField] private QuestionDB teamQuestionDB;

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject countdownTimerPrefab;
    private bool countdownTimerActive;

    private int numberOfQuestionTypes;

    private List<PlayerPanelController> playerPanelControllers = new();
    private List<int> playerPoints = new();
    private List<string> quizPlayerNames = new();
    private List<string> playerNamesBuzzerRoom = new();
    private bool quizReady = false;

    private bool singleQuestionList;
    private List<IQuestion> allQuestions = new();
    private int currentQuestionIndex;
    private List<int> questionOrder = new();

    private QuestionType currentQuestionType;
    private IQuestionController currentQuestionController;

    private List<bool> explainedAlready = new();

    private SocketIOUnity socket;
    private bool socketConnected = false;
    public string buzzerRoomCode;
    public string buzzerRoomAdminPassword;

    private string quizSocketIOUsername = "pokemonquizprogramm";

    private bool showTextFields = false;
    private bool lockTextFields = false;
    private bool quizOver = false;

    [SerializeField] private AudioEffectsController audioEffects;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        fileDataHandler = new FileDataHandler();
        FileDataHandler.QuizSettingsData settingsData = fileDataHandler.Load();
        if (settingsData != null)
        {
            settings.selectedQuiz = settingsData.selectedQuiz;
            settings.quizzes = settingsData.quizzes;
            Debug.Log("Overwrite data from loaded settings.");
        }
        else
        {
            Debug.Log("Dont overwrite data. Take Editor data instead.");
        }

        NetworkManager.Singleton.StartServer();

        questionsDict = new()
        {
            { QuestionType.Feature, PrepareQuestionList(QuestionType.Feature, featureQuestionDB) },
            { QuestionType.Shiny, PrepareQuestionList(QuestionType.Shiny, shinyQuestionDB) },
            { QuestionType.Blur, PrepareQuestionList(QuestionType.Blur, blurQuestionDB) },
            { QuestionType.Anagram, PrepareQuestionList(QuestionType.Anagram, anagramQuestionDB) },
            { QuestionType.Draw, PrepareQuestionList(QuestionType.Draw, drawQuestionDB) },
            { QuestionType.Footprint, PrepareQuestionList(QuestionType.Footprint, footprintQuestionDB) },
            { QuestionType.Team, PrepareQuestionList(QuestionType.Team, teamQuestionDB) },
        };

        foreach (QuestionType _ in Enum.GetValues(typeof(QuestionType))) {
            explainedAlready.Add(!settings.quiz.explainRules);
        }

        for (int i = 0; i < settings.quiz.players.Count; i++)
        {
            Player player = settings.quiz.players[i];
            GameObject playerPanel = Instantiate(playerPanelPrefab, playerPanels.transform);
            PlayerPanelController playerPanelontroller = playerPanel.GetComponent<PlayerPanelController>();
            playerPanelontroller.SetPlayer(player);

            playerPanelControllers.Add(playerPanelontroller);
            playerPoints.Add(0);
            quizPlayerNames.Add(player.name.ToLower());
        }
        playerPanels.GetComponent<HorizontalLayoutGroup>().spacing = SpacingFromPlayerCount(settings.quiz.players.Count);
        ShowTextFields(showTextFields);

        if (settings.quiz.enableBuzzerServer)
        {
            DisplayBuzzerServerSetupPrompt();
        } else
        {
            DisplayStatus();
        }

        PrepareQuiz();
    }

    public void DisplayBuzzerServerSetupPrompt(string errorMessage = null)
    {
        ClearQuestionContainer();
        GameObject buzzerSetupPrompt = Instantiate(buzzerServerSetupPromptPrefab, questionContainer.transform);
        if (errorMessage != null)
            buzzerSetupPrompt.GetComponent<BuzzerServerSetupPrompt>().DisplayInfoMessage(errorMessage);
    }

    public void DisplayStatus()
    {
        GameObject statusObject = Instantiate(statusPrefab, questionContainer.transform);
        statusText = statusObject.transform.GetChild(0).GetComponent<TMP_Text>();
        statusLoopActive = true;
        StartCoroutine(StatusUpdateLoop(0.2f));
    }

    public IEnumerator StatusUpdateLoop(float delayBetweenUpdates)
    {
        UpdateStatus();
        yield return new WaitForSeconds(delayBetweenUpdates);
        if (statusLoopActive && !quizReady)
        {
            StartCoroutine(StatusUpdateLoop(delayBetweenUpdates));
        }
    }

    void OnApplicationQuit()
    {
        if (socket != null)
        {
            socket.Options.Reconnection = false;
            socket.Disconnect();
        }
    }

    private List<IQuestion> PrepareQuestionList(QuestionType questionType, QuestionDB questionDB)
    {
        return questionDB.questions.ToList().Where(
                question => !settings.quiz.questionTypeSettingsList.Where(
                    questionTypeSetting => questionTypeSetting.type == questionType).ToList()[0]
                .excludedGens.Contains(question.pokemonGen))
            .OrderBy(x => UnityEngine.Random.value).ToList();
    }

    private void Update()
    {
        if (Input.GetButtonDown("NextQuestion"))
        {
            //NextQuestion();
            StartCountdownTimer();
        }
        if (Input.GetButtonDown("NextQuestionStep"))
        {
            NextQuestionStep();
        }
        if (Input.GetButtonDown("ShowSolution"))
        {
            ShowSolution();
        }
    }

    public void UpdateStatus()
    {
        if (settings.quiz.enableBuzzerServer && !socketConnected)
        {
            statusText.text = "Connecting to buzzer server...";
            return;
        }

        if (settings.quiz.enableBuzzerServer && !AllPlayersConnected())
        {
            statusText.text = $"Waiting for players to join buzzer room ({buzzerRoomCode}) \nConnected players: {ConnectedPlayerStrings()}";
            return;
        } 

        if (!adminPanelConnected)
        {
            statusText.text = "Waiting for admin panel to connect...";
            return;
        }

        statusText.text = "Quiz ready";
        quizReady = true;
    }

    public string ConnectedPlayerStrings()
    {
        return string.Join(", ", playerNamesBuzzerRoom.Where(name => name != quizSocketIOUsername));
    }

    public bool AllPlayersConnected()
    {
        foreach (string name in quizPlayerNames)
        {
            if (!playerNamesBuzzerRoom.Contains(name))
            {
                return false;
            }
        }
        return true;
    }

    public void AdminPanelConnected()
    {
        adminPanelConnected = true;
    }

    public void SetUpSocketIOConnection(string roomCode, string password)
    {
        buzzerRoomCode = roomCode.ToUpper();
        buzzerRoomAdminPassword = password;

        var uri = new Uri("https://buzzter-server.weekofcharity.de");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string> { {"token", "UNITY"} },
            EIO = 4,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        ///// reserved socketio events
        socket.OnConnected += (sender, e) =>
        {
            socketConnected = true;
            socket.Emit("connect_to_room", new { room = buzzerRoomCode, password = buzzerRoomAdminPassword, username = quizSocketIOUsername });
            Debug.Log("Connected.");
        };
        /*socket.OnPing += (sender, e) =>
        {
            Debug.Log("Ping");
        };
        socket.OnPong += (sender, e) =>
        {
            Debug.Log("Pong: " + e.TotalMilliseconds);
        };*/
        socket.OnDisconnected += (sender, e) =>
        {
            socketConnected = false;
            Debug.Log("disconnect: " + e);
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };

        socket.OnUnityThread("room_update", response =>
        {
            RoomUpdateResponse roomUpdate = response.GetValue<RoomUpdateResponse>();
            if (!roomUpdate.users.Find(user => user.name == quizSocketIOUsername).isAdmin)
            {
                statusLoopActive = false;
                socket.Disconnect();
                DisplayBuzzerServerSetupPrompt("Authenticating as admin in buzzer room failed. Change room number or admin password.");
                return;
            }
            playerNamesBuzzerRoom = roomUpdate.users.Select(user => user.name).ToList();
            if (roomUpdate.pressed)
            {
                BuzzerFromPlayerWithName(roomUpdate.pressed_by, false);
            }
        });
        socket.OnUnityThread("buzzer_was_pressed", response =>
        {
            string name = response.GetValue<string>();
            BuzzerFromPlayerWithName(name, true);
        });
        socket.OnUnityThread("textfield_update_from", response =>
        {
            TextfieldUpdateResponse textfieldUpdate = response.GetValue<TextfieldUpdateResponse>();
            UpdateTextfield(textfieldUpdate);
        });
        socket.OnUnityThread("buzzer_was_freed", response =>
        {
            OnBuzzerFreed();
        });
        Debug.Log("Connecting...");
        socket.Connect();
        DisplayStatus();
    }

    public void FreeBuzzer()
    {
        if (socketConnected) {
            socket.Emit("free_buzzer");
        }
    }

    public void ResetAllBuzzerHighlights()
    {
        foreach (PlayerPanelController playerPanelController in playerPanelControllers)
        {
            playerPanelController.ResetBuzzerHighlight();
        }
    }

    public void OnBuzzerFreed()
    {
        if (quizOver) return;
        ResetAllBuzzerHighlights();
        Type controllerType = currentQuestionController?.GetType();
        if (controllerType == typeof(DrawQuestionController))
        {
            (currentQuestionController as DrawQuestionController).Play();
        }
        else if (controllerType == typeof(BlurQuestionController))
        {
            (currentQuestionController as BlurQuestionController).Play();
        }
    }

    public void BuzzerFromPlayerWithName(string name, bool playSound)
    {
        Debug.Log($"The buzzer was hit by {name}.");
        int index = quizPlayerNames.IndexOf(name);
        if (index == -1)
        {
            Debug.Log($"{name} is not part of the quiz. Freeing buzzer.");
            FreeBuzzer();
            return;
        }

        Type controllerType = currentQuestionController?.GetType();

        if (quizOver || controllerType == typeof(ShinyQuestionController) || controllerType == typeof(FootprintQuestionController))
        {
            Debug.Log("Buzzer currently not enabled. Freeing buzzer.");
            FreeBuzzer();
            return;
        }
        else
        {
            playerPanelControllers[index]?.SetBuzzerHighlight();
            if (playSound) audioEffects.PlayBuzzerSound();
            if (controllerType == typeof(DrawQuestionController))
            {
                (currentQuestionController as DrawQuestionController).Pause();
            }
            else if (controllerType == typeof(BlurQuestionController))
            {
                (currentQuestionController as BlurQuestionController).Pause();
            }
        }
    }

    public void LockTextFields(bool enableLock)
    {
        lockTextFields = enableLock;
        if (!enableLock) {
            foreach (PlayerPanelController ppc in playerPanelControllers)
            {
                ppc.ReloadTextFieldTexts();
            }
        }
    }

    public void UpdateTextfield(TextfieldUpdateResponse response)
    {
        int playerIndex = quizPlayerNames.IndexOf(response.name);
        if (playerIndex != -1) {
            playerPanelControllers[playerIndex].SetTextFieldText(response.text, !lockTextFields);
        }
    }

    public void StartCountdownTimer(float time=5f)
    {
        if (countdownTimerActive) return;
        countdownTimerActive = true;
        audioEffects.Play(SoundEffect.Ticking);
        GameObject countdownTimerObject = Instantiate(countdownTimerPrefab, canvas.transform);
        countdownTimerObject.GetComponent<CountdownTimerController>().StartCountdown(time);
    }

    public void CountdownTimerOver()
    {
        countdownTimerActive = false;
        PlayTimerOverSound();
    }

    public void PlayTimerOverSound()
    {
        audioEffects.StopTicking();
        audioEffects.Play(SoundEffect.TimerOver);
    }

    public void PrepareQuiz()
    {
        singleQuestionList = !settings.quiz.infiniteMode || settings.quiz.shuffleCategories;

        var questionTypes = Enum.GetValues(typeof(QuestionType));
        numberOfQuestionTypes = questionTypes.Length;

        if (!singleQuestionList)
        {
            foreach (QuestionType questionType in questionTypes)
            {
                currentQuestionIndices.Add(questionType, -1);
            }
        }
        else
        {
            foreach (QuestionType questionType in questionTypes)
            {
                currentQuestionIndex = -1;

                int questionTypeAmount = questionsDict[questionType].Count;

                if (!settings.quiz.infiniteMode)
                    questionTypeAmount = Mathf.Min(questionTypeAmount, settings.quiz.questionTypeSettingsList.Find(x => x.type == questionType).questionAmount);

                foreach (IQuestion question in questionsDict[questionType].Take(questionTypeAmount))
                {
                    allQuestions.Add(question);
                }
            }

            for (int i = 0; i < allQuestions.Count; i++)
            {
                questionOrder.Add(questionOrder.Count);
            }

            if (settings.quiz.shuffleCategories)
            {
                questionOrder = questionOrder.OrderBy(x => UnityEngine.Random.value).ToList();
            }
        }

        if (singleQuestionList)
        {
            Debug.Log($"Prepared Quiz with {allQuestions.Count} questions in queue.");
        } else
        {
            Debug.Log("Prepared Quiz with category switch controls enabled.");
        }
    }

    public int NextQuestion()
    {
        if (!quizReady) return -3;
        IQuestion question = null;

        if (singleQuestionList)
        {
            if (currentQuestionIndex >= allQuestions.Count - 1)
            {
                OnAllQuestionsPlayed();
                return -1;
            }
            currentQuestionIndex++;
            int index = questionOrder[currentQuestionIndex];
            question = allQuestions[index];
        }
        else
        {
            int counter = 0;
            while (currentQuestionIndices[selectedCategory] >= questionsDict[selectedCategory].Count - 1)
            {
                int currentCategoryInt = (int)(selectedCategory + 1) % numberOfQuestionTypes;
                selectedCategory = (QuestionType)currentCategoryInt;
                if (counter > numberOfQuestionTypes)
                {
                    OnAllQuestionsPlayed();
                    return -1;
                }
                counter++;
            }
            currentQuestionIndices[selectedCategory]++;
            int index = currentQuestionIndices[selectedCategory];
            question = questionsDict[selectedCategory][index];
        }

        QuestionType type = GetTypeFromIQuestion(question);
        if (!explainedAlready[(int)type])
        {
            ExplainCategory(type);
            ResetCurrentQuestionIndexToPrevious();
            return -2;
        } else
        {
            DisplayQuestion(question);
            DisplayAdminSolution(question);
            return (int)currentQuestionType;
        }
    }

    public void ShowTextFields(bool show)
    {
        showTextFields = show;
        LockTextFields(show);
        foreach (PlayerPanelController ppc in playerPanelControllers)
        {
            ppc.ShowTextField(show);
        }
    }

    public void ToggleShowTextFields()
    {
        showTextFields = !showTextFields;
        ShowTextFields(showTextFields);
    }

    public void ExplainCategory(QuestionType type)
    {
        ClearQuestionContainer();
        explainedAlready[(int)type] = true;
        Instantiate(explainScreenPrefab, questionContainer.transform)
            .GetComponent<ExplainScreenController>().ExplainQuestionType(type);
    }

    public QuestionType GetTypeFromIQuestion(IQuestion question)
    {
        if (question.GetType() == typeof(FeatureQuestion))
        {
            return QuestionType.Feature;
        }
        else if (question.GetType() == typeof(ShinyQuestion))
        {
            return QuestionType.Shiny;
        }   
        else if (question.GetType() == typeof(BlurQuestion))
        {   
            return QuestionType.Blur;
        }
        else if (question.GetType() == typeof(AnagramQuestion))
        {
            return QuestionType.Anagram;
        }
        else if (question.GetType() == typeof(DrawQuestion))
        {
            return QuestionType.Draw;
        }
        else if (question.GetType() == typeof(FootprintQuestion))
        {
            return QuestionType.Footprint;
        }
        else if (question.GetType() == typeof(TeamQuestion))
        {
            return QuestionType.Team;
        }
        return QuestionType.Feature;
    }

    public void ResetCurrentQuestionIndexToPrevious()
    {
        if (singleQuestionList)
            currentQuestionIndex--;
        else
            currentQuestionIndices[selectedCategory]--;
    }

    private void OnAllQuestionsPlayed()
    {
        Debug.Log("Every question has been played!");
        EndQuiz();
    }

    public void EndQuiz()
    {
        Debug.Log("Quiz is over!");
        quizOver = true;
        ClearQuestionContainer();
        if (winningBackgroundSprite != null)
        {
            foreach (Image bgImage in backgroundImages)
            {
                bgImage.sprite = winningBackgroundSprite;
            }
        }
        GameObject endScreenObject = Instantiate(endScreenPrefab, questionContainer.transform);
        List<Result> results = new List<Result>();
        for (int i = 0; i < playerPoints.Count; i++)
        {
            Result result = new()
            {
                playerId = i,
                playerName = settings.quiz.players[i].name,
                playerPoints = playerPoints[i],
                color = playerPanelControllers[i].GetPlayerPanelColor(),
            };
            results.Add(result);
        }
        results = results.OrderByDescending(result => result.playerPoints).ToList();
        ResetAllBuzzerHighlights();
        playerPanelControllers[results[0].playerId].SetBuzzerHighlight();
        StartCoroutine(playerPanelControllers[results[0].playerId].PlayWinAnimation());
        endScreenObject.GetComponent<EndScreenController>().DisplayPlayerRankings(results);
    }

    public void SwitchToQuestionType(int questionIndex)
    {
        Debug.Log("Switch To: " + (QuestionType)questionIndex);
        selectedCategory = (QuestionType)questionIndex;
    }

    private void DisplayQuestion(IQuestion question)
    {
        FreeBuzzer();
        ClearQuestionContainer();
        ShowTextFields(false);
        currentQuestionType = GetTypeFromIQuestion(question);
        if (question.GetType() == typeof(FeatureQuestion))
        {
            DisplayFeatureQuestion(question as FeatureQuestion);
        }
        else if (question.GetType() == typeof(ShinyQuestion))
        {
            DisplayShinyQuestion(question as ShinyQuestion);
        }
        else if (question.GetType() == typeof(BlurQuestion))
        {
            DisplayBlurQuestion(question as BlurQuestion);
        }
        else if (question.GetType() == typeof(AnagramQuestion))
        {
            DisplayAnagramQuestion(question as AnagramQuestion);
        }
        else if (question.GetType() == typeof(DrawQuestion))
        {
            DisplayDrawQuestion(question as DrawQuestion);
        }
        else if (question.GetType() == typeof(FootprintQuestion))
        {
            DisplayFootprintQuestion(question as FootprintQuestion);
        }
        else if (question.GetType() == typeof(TeamQuestion))
        {
            DisplayTeamQuestion(question as TeamQuestion);
        }
    }

    public void DisplayFeatureQuestion(FeatureQuestion questionData)
    {
        GameObject questionObject = Instantiate(featureQuestionPrefab, questionContainer.transform);
        FeatureQuestionController questionController = questionObject.GetComponent<FeatureQuestionController>();
        StartQuestionController(questionData, questionController);
    }

    public void DisplayShinyQuestion(ShinyQuestion questionData)
    {
        GameObject questionObject = Instantiate(shinyQuestionPrefab, questionContainer.transform);
        ShinyQuestionController questionController = questionObject.GetComponent<ShinyQuestionController>();
        StartQuestionController(questionData, questionController);
    }

    public void DisplayBlurQuestion(BlurQuestion questionData)
    {
        GameObject questionObject = Instantiate(blurQuestionPrefab, questionContainer.transform);
        BlurQuestionController questionController = questionObject.GetComponent<BlurQuestionController>();
        StartQuestionController(questionData, questionController);
    }

    public void DisplayAnagramQuestion(AnagramQuestion questionData)
    {
        GameObject questionObject = Instantiate(anagramQuestionPrefab, questionContainer.transform);
        AnagramQuestionController questionController = questionObject.GetComponent<AnagramQuestionController>();
        StartQuestionController(questionData, questionController);
    }

    public void DisplayDrawQuestion(DrawQuestion questionData)
    {
        GameObject questionObject = Instantiate(drawQuestionPrefab, questionContainer.transform);
        DrawQuestionController questionController = questionObject.GetComponent<DrawQuestionController>();
        StartQuestionController(questionData, questionController);
    }

    public void DisplayFootprintQuestion(FootprintQuestion questionData)
    {
        GameObject questionObject = Instantiate(footprintQuestionPrefab, questionContainer.transform);
        FootprintQuestionController questionController = questionObject.GetComponent<FootprintQuestionController>();
        StartQuestionController(questionData, questionController);
    }

    public void DisplayTeamQuestion(TeamQuestion questionData)
    {
        GameObject questionObject = Instantiate(teamQuestionPrefab, questionContainer.transform);
        TeamQuestionController questionController = questionObject.GetComponent<TeamQuestionController>();
        StartQuestionController(questionData, questionController);
    }

    public void StartQuestionController(IQuestion questionData, IQuestionController questionController)
    {
        questionController.SetData(questionData);
        questionController.StartQuestion();
        currentQuestionController = questionController;
    }

    public void NextQuestionStep()
    {
        if (currentQuestionController == null || currentQuestionController.ToString() == "null") return;
        currentQuestionController.NextQuestionStep();
    }

    public void ShowSolution()
    {
        if (currentQuestionController == null || currentQuestionController.ToString() == "null") return;
        currentQuestionController.ResetDisplay();
        currentQuestionController.ShowSolution();
    }

    public void ToggleFeatureBackground()
    {
        if (currentQuestionController == null || currentQuestionController.ToString() == "null") return;
        if (currentQuestionController.GetType() == typeof(FeatureQuestionController))
        {
            (currentQuestionController as FeatureQuestionController).ToggleBackground();
        }
    }

    public void ClearQuestionContainer()
    {
        foreach (Transform transform in questionContainer.transform)
        {
            Destroy(transform.gameObject);
        }
    }

    public static int SpacingFromPlayerCount(int playerCount)
    {
        return Mathf.FloorToInt((3.32f * playerCount * playerCount) - (77.2f * playerCount) + 364);
    }

    public void RegisterAdminPanelUser(AdminPanelUser user)
    {
        adminPanelUser = user;
    }

    public void CorrectAnswerFrom(int playerID)
    {
        QuestionTypeSettings questionTypeSettings = settings.quiz.questionTypeSettingsList.Find(x => x.type == currentQuestionType);
        AddPointsToPlayer(playerID, questionTypeSettings.correctPoints);
        AddPointsToEveryoneExcept(playerID, questionTypeSettings.correctPointsOther);
        StartCoroutine(playerPanelControllers[playerID].PlayCorrectAnimation());
        audioEffects.PlayCorrectSound();
        ShowSolution();
    }

    public void WrongAnswerFrom(int playerID)
    {
        QuestionTypeSettings questionTypeSettings = settings.quiz.questionTypeSettingsList.Find(x => x.type == currentQuestionType);
        AddPointsToPlayer(playerID, questionTypeSettings.wrongPoints);
        AddPointsToEveryoneExcept(playerID, questionTypeSettings.wrongPointsOther);
        audioEffects.PlayWrongSound();
        FreeBuzzer();
    }

    public void AddPointsToEveryoneExcept(int playerID, int points)
    {
        for (int i = 0; i < playerPoints.Count; i++)
        {
            if (i == playerID) continue;
            AddPointsToPlayer(i, points);
        }
    }

    public void AddPointsToPlayer(int playerID, int points)
    {
        playerPoints[playerID] += points;
        playerPanelControllers[playerID].SetPlayerPointsText(playerPoints[playerID].ToString());
    }

    public void DisplayAdminSolution(IQuestion question)
    {
        if (adminPanelUser != null && question != null)
        {
            string solutionString;
            if (currentQuestionType == QuestionType.Shiny)
            {
                int shinySolutionIndex = (currentQuestionController as ShinyQuestionController).solutionIndex;
                string[] shinySolutionStrings = new string[] { "A,", "B,", "C," };
                solutionString = shinySolutionStrings[shinySolutionIndex];
            }
            else if (currentQuestionType == QuestionType.Footprint)
            {
                int footprintSolutionIndex = (currentQuestionController as FootprintQuestionController).solutionIndex;
                string[] footprintSolutionStrings = new string[] { "A,", "B,", "C,", "D," };
                solutionString = footprintSolutionStrings[footprintSolutionIndex];
            }
            else if (currentQuestionType == QuestionType.Team)
            {
                solutionString = question.pokemonName + ",";
            }
            else
            {
                solutionString = question.pokemonName + ",GEN " + ((int)question.pokemonGen + 1);
            }
            adminPanelUser.ShowSolutionClientRpc(solutionString);
        }
    }

    public void SetNextStepButtonTextId(NextStepButtonState state)
    {
        if (adminPanelUser != null)
        {
            adminPanelUser.SetNextStepButtonTextClientRpc((int)state);
        }
    }

    private void OnSceneUnloaded(Scene currentScene)
    {
        Debug.Log("Scene " + currentScene.name + " has been unloaded.");
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        if (socket != null)
            socket.Disconnect();
        NetworkManager.Singleton.Shutdown();
    }
}
