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
    
    private List<IQuestion> allQuestions = new();
    private Dictionary<QuestionType, int> NOINF_currentQuestionIndices = new();
    private Dictionary<QuestionType, int> lastQuestionIndices = new();
    private List<int> questionOrder = new();

    private QuestionType selectedCategory = 0;
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
        int totalQuestionAmount = 0;

        foreach (QuestionType questionType in Enum.GetValues(typeof(QuestionType)))
        {
            numberOfQuestionTypes++;
            NOINF_currentQuestionIndices.Add(questionType, -1);

            int questionTypeAmount = questionsDict[questionType].Count;
            if (!settings.quiz.infiniteMode)
            {
                questionTypeAmount = Mathf.Min(questionTypeAmount, settings.quiz.questionTypeSettingsList.Find(x => x.type == questionType).questionAmount);
            }

            foreach (IQuestion featureQuestion in questionsDict[questionType].OrderBy(x => UnityEngine.Random.value).Take(questionTypeAmount))
            {
                allQuestions.Add(featureQuestion);
            }

            totalQuestionAmount += questionTypeAmount;

            lastQuestionIndices.Add(questionType, totalQuestionAmount - 1);
        }

        for (int i = 0; i < totalQuestionAmount; i++)
        {
            questionOrder.Add(questionOrder.Count);
        }

        if (settings.quiz.shuffleCategories)
        {
            questionOrder.OrderBy(x => UnityEngine.Random.value);
        }
        NextQuestion();
    }

    public void NextQuestion()
    {
        if (!settings.quiz.shuffleCategories)
        {
            int counter = 0;
            while (NOINF_currentQuestionIndices[selectedCategory] == lastQuestionIndices[selectedCategory])
            {
                int currentCategoryInt = (int)(selectedCategory + 1) % numberOfQuestionTypes;
                selectedCategory = (QuestionType)currentCategoryInt;
                if (counter > numberOfQuestionTypes)
                {
                    Debug.Log("Every question has been played!");
                    quizDone = true;
                    return;
                }
                counter++;
            }
        }

        NOINF_currentQuestionIndices[selectedCategory]++;
        int realIndex = questionOrder[NOINF_currentQuestionIndices[0]];
        IQuestion question = allQuestions[realIndex];
        DisplayQuestion(question);
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
        ClearQuestionContainer();
        GameObject questionObject = Instantiate(featureQuestionPrefab, questionContainer.transform);
        FeatureQuestionController questionController = questionObject.GetComponent<FeatureQuestionController>();
        StartQuestionController(questionData, questionController);
    }

    public void DisplayShinyQuestion(ShinyQuestion questionData)
    {
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
