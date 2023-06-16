using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

[System.Serializable]
public enum QuestionType
{
    Feature,
    Shiny
}

public interface IQuestion { }

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
    private QuestionType selectedCategory = 0;

    private AdminPanelUser adminPanelUser;

    [SerializeField] private QuizSettings settings;

    [SerializeField] private GameObject playerPanels;
    [SerializeField] private GameObject playerPanelPrefab;

    [SerializeField] private GameObject questionContainer;
    [SerializeField] private GameObject featureQuestionPrefab;
    [SerializeField] private GameObject shinyQuestionPrefab;

    [SerializeField] private FeatureQuestionDB featureQuestionDB;
    [SerializeField] private ShinyQuestionDB shinyQuestionDB;

    private int numberOfQuestionTypes;

    private List<PlayerPanelController> playerPanelControllers = new();
    private List<int> playerPoints = new();

    private bool singleQuestionList;
    private List<IQuestion> allQuestions = new();
    private int currentQuestionIndex;
    private List<int> questionOrder = new();

    private QuestionType currentQuestionType;
    private IQuestionController currentQuestionController;

    private bool quizDone = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        NetworkManager.Singleton.StartServer();

        //TODO: Randomize here instead of later
        questionsDict = new()
        {
            { QuestionType.Feature, featureQuestionDB.questions.ToList().ConvertAll(x => (IQuestion)x) },
            { QuestionType.Shiny, shinyQuestionDB.questions.ToList().ConvertAll(x => (IQuestion)x) }
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

                foreach (IQuestion question in questionsDict[questionType].OrderBy(x => UnityEngine.Random.value).Take(questionTypeAmount))
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
        NextQuestion();
    }

    public void NextQuestion()
    {
        IQuestion question = null;

        if (singleQuestionList)
        {
            if (currentQuestionIndex >= allQuestions.Count - 1)
            {
                OnQuizFinished();
                return;
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
                    return;
                }
                counter++;
            }
            currentQuestionIndices[selectedCategory]++;
            int index = currentQuestionIndices[selectedCategory];
            question = questionsDict[selectedCategory][index];
        }

        DisplayQuestion(question);
    }

    private void OnQuizFinished()
    {
        Debug.Log("Every question has been played!");
        quizDone = true;
    }

    private void DisplayQuestion(IQuestion question)
    {
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
    }

    public void DisplayFeatureQuestion(FeatureQuestion questionData)
    {
        Debug.Log("Displaying Feature Question.");
        ClearQuestionContainer();
        GameObject questionObject = Instantiate(featureQuestionPrefab, questionContainer.transform);
        FeatureQuestionController questionController = questionObject.GetComponent<FeatureQuestionController>();
        StartQuestionController(questionData, questionController);
    }

    public void DisplayShinyQuestion(ShinyQuestion questionData)
    {
        Debug.Log("Displaying Shiny Question.");
        ClearQuestionContainer();
        GameObject questionObject = Instantiate(shinyQuestionPrefab, questionContainer.transform);
        ShinyQuestionController questionController = questionObject.GetComponent<ShinyQuestionController>();
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
        currentQuestionController.NextQuestionStep();
    }

    public void ShowSolution()
    {
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
        return Mathf.FloorToInt((3.63095f * playerCount * playerCount) - (77.2024f * playerCount) + 420);
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

    public void DisplayAdminSolution()
    {
        adminPanelUser.ShowSolutionClientRpc();
    }
}
