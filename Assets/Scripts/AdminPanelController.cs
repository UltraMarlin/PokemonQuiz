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

    [SerializeField] private Image solutionImage;
    [SerializeField] private TextMeshProUGUI nextButtonStepText;

    [SerializeField] private CategorySelector categorySelector;

    public Sprite testSprite;

    private QuestionType currentQuestionType;
    private bool lastQuestionReached = false;
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

    public void SetSolutionImage(Sprite sprite)
    {
        if (sprite != null)
        {
            solutionImage.color = Color.white;
        } else
        {
            solutionImage.color = Color.clear;
        }
        solutionImage.sprite = sprite;
    }

    public void SetSolutionImageTest()
    {
        SetSolutionImage(testSprite);
    }

    public void NextQuestion()
    {
        adminPanelUser.NextQuestionServerRpc();
    }

    public void SetNextStepButtonText(int textid)
    {
        string[] textStrings = {"Next Step", "Pausieren", "Abspielen", ""};
        nextButtonStepText.text = textStrings[textid];
    }

    public void NextQuestionStep()
    {
        adminPanelUser.NextQuestionStepServerRpc();
    }

    public void ShowSolution()
    {
        adminPanelUser.ShowSolutionServerRpc();
    }

    public void SetCurrentQuestionType(int questionType)
    {
        if (questionType == -1)
        {
            lastQuestionReached = true;
        } else
        {
            currentQuestionType = (QuestionType)questionType;
        }
        categorySelector.SetActiveCategoryLabel(questionType);
        Debug.Log("AdminControlPanel: " + currentQuestionType);
    }
}
