using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AdminPanelController : MonoBehaviour
{
    public static AdminPanelController instance;
    public AdminPanelUser adminPanelUser;

    [SerializeField] private QuizSettings settings;

    [SerializeField] private GameObject playerControlPanels;
    [SerializeField] private GameObject playerControlPanelPrefab;

    [SerializeField] private TextMeshProUGUI solutionText;
    [SerializeField] private TextMeshProUGUI solutionGenText;
    [SerializeField] private TextMeshProUGUI nextButtonStepText;

    [SerializeField] private CategorySelector categorySelector;

    [SerializeField] private Button featureBackgroundButton;

    private QuestionType currentQuestionType;
    public bool quizOver = false;
    private int nextButtonStepTextId;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        Screen.SetResolution(1920, 300, FullScreenMode.Windowed);
        NetworkManager.Singleton.StartClient();

        for (int i = 0; i < settings.quiz.players.Count; i++)
        {
            Player player = settings.quiz.players[i];
            GameObject playerControlPanel = Instantiate(playerControlPanelPrefab, playerControlPanels.transform);
            PlayerControlPanel pcpComponent = playerControlPanel.GetComponent<PlayerControlPanel>();
            pcpComponent.playerID = i;
            pcpComponent.SetPlayerName(player.name);
        }

        playerControlPanels.GetComponent<HorizontalLayoutGroup>().spacing = QuizSession.SpacingFromPlayerCount(settings.quiz.players.Count);
    }

    public void RegisterAdminPanelUser(AdminPanelUser user)
    {
        adminPanelUser = user;
    }

    public void SetSolutionString(string solutionString)
    {
        var splitString = solutionString.Split(',');
        solutionText.text = splitString[0];
        solutionGenText.text = splitString[1];
    }

    public void SetNextStepButtonText(int textid)
    {
        string[] textStrings = { "Next Step", "Pausieren", "Abspielen", "" };
        nextButtonStepText.text = textStrings[textid];
    }

    public void NextQuestion()
    {
        if (quizOver) return;
        adminPanelUser.NextQuestionServerRpc();
    }

    public void NextQuestionStep()
    {
        if (quizOver) return;
        adminPanelUser.NextQuestionStepServerRpc();
    }

    public void ShowSolution()
    {
        if (quizOver) return;
        adminPanelUser.ShowSolutionServerRpc();
    }

    public void ToggleFeatureBackground()
    {
        if (quizOver) return;
        adminPanelUser.ToggleFeatureBackgroundServerRpc();
    }

    public void EndQuiz()
    {
        if (quizOver) return;
        quizOver = true;
        adminPanelUser.EndQuizServerRpc();
    }

    public void SetCurrentQuestionType(int questionType)
    {
        if (questionType == -1)
        {
            quizOver = true;
        } else
        {
            currentQuestionType = (QuestionType)questionType;
        }
        categorySelector.SetActiveCategoryLabel(questionType);
        Debug.Log("AdminControlPanel: " + currentQuestionType);
        featureBackgroundButton.interactable = currentQuestionType == QuestionType.Feature;
    }
}
