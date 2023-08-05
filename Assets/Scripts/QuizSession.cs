using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum QuestionType
{
    Feature,
    Shiny,
    Blur,
    Anagram,
    Draw
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

public class QuizUtils
{
    //
    public static bool validateQuestionObjects = true;
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

public class QuizSession : MonoBehaviour
{
    public static QuizSession instance;

    public Dictionary<QuestionType, List<IQuestion>> questionsDict;
    private Dictionary<QuestionType, int> currentQuestionIndices = new();
    private QuestionType selectedCategory = QuestionType.Feature;

    private AdminPanelUser adminPanelUser;

    [SerializeField] private QuizSettings settings;

    [SerializeField] private GameObject playerPanels;
    [SerializeField] private GameObject playerPanelPrefab;

    [SerializeField] private GameObject questionContainer;

    [SerializeField] private GameObject featureQuestionPrefab;
    [SerializeField] private GameObject shinyQuestionPrefab;
    [SerializeField] private GameObject blurQuestionPrefab;
    [SerializeField] private GameObject anagramQuestionPrefab;
    [SerializeField] private GameObject drawQuestionPrefab;

    [SerializeField] private QuestionDB featureQuestionDB;
    [SerializeField] private QuestionDB shinyQuestionDB;
    [SerializeField] private QuestionDB blurQuestionDB;
    [SerializeField] private QuestionDB anagramQuestionDB;
    [SerializeField] private QuestionDB drawQuestionDB;

    private int numberOfQuestionTypes;

    private List<PlayerPanelController> playerPanelControllers = new();
    private List<int> playerPoints = new();

    private bool singleQuestionList;
    private List<IQuestion> allQuestions = new();
    private int currentQuestionIndex;
    private List<int> questionOrder = new();

    private QuestionType currentQuestionType;
    private IQuestionController currentQuestionController;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        NetworkManager.Singleton.StartServer();

        questionsDict = new()
        {
            { QuestionType.Feature, prepareQuestionList(QuestionType.Feature, featureQuestionDB) },
            { QuestionType.Shiny, prepareQuestionList(QuestionType.Shiny, shinyQuestionDB) },
            { QuestionType.Blur, prepareQuestionList(QuestionType.Blur, blurQuestionDB) },
            { QuestionType.Anagram, prepareQuestionList(QuestionType.Anagram, anagramQuestionDB) },
            { QuestionType.Draw, prepareQuestionList(QuestionType.Draw, drawQuestionDB) },
        };

        for (int i = 0; i < settings.quiz.players.Count; i++)
        {
            Player player = settings.quiz.players[i];
            GameObject playerPanel = Instantiate(playerPanelPrefab, playerPanels.transform);
            PlayerPanelController playerPanelontroller = playerPanel.GetComponent<PlayerPanelController>();
            playerPanelontroller.SetPlayer(player);

            playerPanelControllers.Add(playerPanelontroller);
            playerPoints.Add(0);
        }
        playerPanels.GetComponent<HorizontalLayoutGroup>().spacing = SpacingFromPlayerCount(settings.quiz.players.Count);

        PrepareQuiz();
    }

    private List<IQuestion> prepareQuestionList(QuestionType questionType, QuestionDB questionDB)
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
            NextQuestion();
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
        IQuestion question = null;
        SetNextStepButtonTextId(0);

        if (singleQuestionList)
        {
            if (currentQuestionIndex >= allQuestions.Count - 1)
            {
                OnQuizFinished();
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
                    OnQuizFinished();
                    return -1;
                }
                counter++;
            }
            currentQuestionIndices[selectedCategory]++;
            int index = currentQuestionIndices[selectedCategory];
            question = questionsDict[selectedCategory][index];
        }

        DisplayQuestion(question);
        DisplayAdminSolution(question);
        return (int)currentQuestionType;
    }

    private void OnQuizFinished()
    {
        Debug.Log("Every question has been played!");
    }

    public void SwitchToQuestionType(int questionIndex)
    {
        Debug.Log("Switch To: " + (QuestionType)questionIndex);
        selectedCategory = (QuestionType)questionIndex;
    }

    private void DisplayQuestion(IQuestion question)
    {
        ClearQuestionContainer();
        if (question.GetType() == typeof(FeatureQuestion))
        {
            currentQuestionType = QuestionType.Feature;
            DisplayFeatureQuestion(question as FeatureQuestion);
        }
        else if (question.GetType() == typeof(ShinyQuestion))
        {
            currentQuestionType = QuestionType.Shiny;
            DisplayShinyQuestion(question as ShinyQuestion);
        }
        else if (question.GetType() == typeof(BlurQuestion))
        {
            currentQuestionType = QuestionType.Blur;
            DisplayBlurQuestion(question as BlurQuestion);
        }
        else if (question.GetType() == typeof(AnagramQuestion))
        {
            currentQuestionType = QuestionType.Anagram;
            DisplayAnagramQuestion(question as AnagramQuestion);
        }
        else if (question.GetType() == typeof(DrawQuestion))
        {
            currentQuestionType = QuestionType.Draw;
            DisplayDrawQuestion(question as DrawQuestion);
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

    public void StartQuestionController(IQuestion questionData, IQuestionController questionController)
    {
        questionController.SetData(questionData);
        questionController.StartQuestion();
        currentQuestionController = questionController;
    }

    public void NextQuestionFromCategory(QuestionType category)
    {
        selectedCategory = category;
        NextQuestion();
    }

    public void NextQuestionStep()
    {
        if (currentQuestionController == null) return;
        currentQuestionController.NextQuestionStep();
    }

    public void ShowSolution()
    {
        if (currentQuestionController == null) return;
        currentQuestionController.ResetDisplay();
        currentQuestionController.ShowSolution();
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
        int points = settings.quiz.questionTypeSettingsList.Find(x => x.type == currentQuestionType).correctPoints;
        AddPointsToPlayer(playerID, points);
    }

    public void WrongAnswerFrom(int playerID)
    {
        int points = settings.quiz.questionTypeSettingsList.Find(x => x.type == currentQuestionType).wrongPointsOther;
        AddPointsToEveryoneExcept(playerID, points);
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
                string[] shinySolutionStrings = new string[] { "Left,", "Middle,", "Right," };
                solutionString = shinySolutionStrings[shinySolutionIndex];
            } else {
                solutionString = question.pokemonName + ",GEN " + ((int)question.pokemonGen + 1);
            }
            adminPanelUser.ShowSolutionClientRpc(solutionString);
        }
    }

    public void SetNextStepButtonTextId(int id)
    {
        if (adminPanelUser != null)
        {
            adminPanelUser.SetNextStepButtonTextClientRpc(id);
        }
    }
}
