using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdminPanelController : MonoBehaviour
{
    public static AdminPanelController instance;
    public AdminPanelUser adminPanelUser;

    private FileDataHandler fileDataHandler;

    [SerializeField] private QuizSettings settings;

    [SerializeField] private GameObject playerControlPanels;
    [SerializeField] private GameObject playerControlPanelPrefab;

    [SerializeField] private TextMeshProUGUI solutionText;
    [SerializeField] private TextMeshProUGUI solutionGenText;

    [SerializeField] private CategorySelector categorySelector;

    [SerializeField] private Button featureBackgroundButton;
    [SerializeField] private Button showSolutionButton;
    [SerializeField] private Button nextStepButton;
    [SerializeField] private TextMeshProUGUI showSolutionButtonText;
    private string showSolutionButtonOriginalText;
    [SerializeField] private TextMeshProUGUI nextButtonStepText;
    [SerializeField] private TextMeshProUGUI featureBackgroundButtonText;
    private string featureBackgroundButtonOriginalText;

    private QuestionType currentQuestionType;
    public bool quizOver = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        Screen.SetResolution(1920, 300, FullScreenMode.Windowed);
        Application.targetFrameRate = 30;

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

        NetworkManager.Singleton.StartClient();

        for (int i = 0; i < settings.quiz.players.Count; i++)
        {
            Player player = settings.quiz.players[i];
            GameObject playerControlPanel = Instantiate(playerControlPanelPrefab, playerControlPanels.transform);
            PlayerControlPanel pcpComponent = playerControlPanel.GetComponent<PlayerControlPanel>();
            pcpComponent.playerID = i;
            pcpComponent.SetPlayerName(player.name);
        }

        showSolutionButtonOriginalText = showSolutionButtonText.text;
        featureBackgroundButtonOriginalText = featureBackgroundButtonText.text;

        playerControlPanels.GetComponent<HorizontalLayoutGroup>().spacing = QuizSession.SpacingFromPlayerCount(settings.quiz.players.Count);

        ExpectNextQuestionPress();
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
        nextStepButton.interactable = (NextStepButtonState)textid != NextStepButtonState.Empty;
    }

    public void FreeBuzzer()
    {
        adminPanelUser.FreeBuzzerServerRpc();
    }

    public void StartTimer()
    {
        adminPanelUser.StartTimerServerRpc();
    }

    public void NextQuestion()
    {
        if (quizOver) return;
        adminPanelUser.NextQuestionServerRpc();
    }

    public void ExpectNextQuestionPress()
    {
        showSolutionButton.interactable = false;
        showSolutionButtonText.text = "";
        SetNextStepButtonText((int)NextStepButtonState.Empty);
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
        ExpectNextQuestionPress();
    }

    public void ToggleFeatureBackground()
    {
        if (quizOver) return;
        adminPanelUser.ToggleFeatureBackgroundServerRpc();
    }

    public void ToggleShowTextFields()
    {
        if (quizOver) return;
        adminPanelUser.ToggleShowTextFieldsServerRpc();
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
        }
        else if (questionType == -2)
        {
            ExpectNextQuestionPress();
        }
        else
        {
            showSolutionButton.interactable = true;
            nextStepButton.interactable = true;
            showSolutionButtonText.text = showSolutionButtonOriginalText;
            currentQuestionType = (QuestionType)questionType;
        }
        categorySelector.SetActiveCategoryLabel(questionType);
        Debug.Log("AdminControlPanel: " + currentQuestionType);
        featureBackgroundButton.interactable = currentQuestionType == QuestionType.Feature;
        featureBackgroundButtonText.text = currentQuestionType == QuestionType.Feature ? featureBackgroundButtonOriginalText : "";
    }

    private void OnSceneUnloaded(Scene currentScene)
    {
        Debug.Log("Scene " + currentScene.name + " has been unloaded.");
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        NetworkManager.Singleton.Shutdown();
    }
}
