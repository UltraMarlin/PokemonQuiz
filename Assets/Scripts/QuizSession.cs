using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
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
    void StartQuestion();
    void NextQuestionStep();
    void ShowSolution();
    void ResetDisplay();
}

public class QuizSession : MonoBehaviour
{
    public static QuizSession instance;
    private AdminPanelUser adminPanelUser;

    [SerializeField] private QuizSettings settings;

    [SerializeField] private GameObject playerPanels;
    [SerializeField] private GameObject playerPanelPrefab;

    [SerializeField] private GameObject questionContainer;
    [SerializeField] private GameObject featureQuestionPrefab;
    [SerializeField] private GameObject shinyQuestionPrefab;

    [SerializeField] private FeatureQuestionDB featureQuestionDB;
    [SerializeField] private ShinyQuestionDB shinyQuestionDB;

    private List<PlayerPanelController> playerPanelControllers = new();
    private List<int> playerPoints = new();

    private QuestionType currentQuestionType;
    private int currentQuestionIndex;
    private List<int> questionOrder = new();
    private List<IQuestion> questions = new();
    private IQuestionController currentQuestionController;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        NetworkManager.Singleton.StartServer();

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

    public void PrepareQuiz()
    {
        int totalQuestionAmount = 0;

        List<FeatureQuestion> featureQuestionList = featureQuestionDB.questions.ToList();
        int featureQuestionAmount = Mathf.Min(featureQuestionList.Count, settings.quiz.questionTypeSettingsList.Find(x => x.type == QuestionType.Feature).questionAmount);
        foreach (FeatureQuestion featureQuestion in featureQuestionList.OrderBy(x => Random.value).Take(featureQuestionAmount))
        {
            questions.Add(featureQuestion);
        }
        totalQuestionAmount += featureQuestionAmount;


        for (int i = 0; i < totalQuestionAmount; i++)
        {
            questionOrder.Add(questionOrder.Count);
        }
        
        if (settings.quiz.shuffleCategories)
        {
            questionOrder.OrderBy(x => Random.value);
        }

        currentQuestionIndex = -1;
        NextQuestion();
    }

    public void NextQuestion()
    {
        if (currentQuestionIndex + 1 >= questionOrder.Count) return;

        currentQuestionIndex++;
        int realIndex = questionOrder[currentQuestionIndex];
        IQuestion question = questions[realIndex];
        if (question.GetType() == typeof(FeatureQuestion))
        {
            DisplayFeatureQuestion(question as FeatureQuestion);
        }
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

    public void DisplayFeatureQuestion(FeatureQuestion featureQuestion)
    {
        ClearQuestionContainer();
        GameObject featureQuestionObject = Instantiate(featureQuestionPrefab, questionContainer.transform);
        FeatureQuestionController fqc = featureQuestionObject.GetComponent<FeatureQuestionController>();
        fqc.SetData(featureQuestion);
        fqc.StartQuestion();
        currentQuestionController = fqc;
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
