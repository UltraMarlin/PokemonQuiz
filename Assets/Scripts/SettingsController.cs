using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private QuizSettings settings;

    [SerializeField] private TMP_InputField quizNameInput;
    [SerializeField] private TMP_Dropdown playerCountDropdown;

    [SerializeField] private GameObject playerGrid;
    [SerializeField] private GameObject categoryGrid;

    [SerializeField] private Toggle categoryShuffleToggle;
    [SerializeField] private Toggle infiniteModeToggle;
    [SerializeField] private Toggle explainRulesToggle;

    [SerializeField] private List<PlayerSettingsCard> playerSettingsCards;
    [SerializeField] private List<CategorySettingsCard> categorySettingsCards;

    [SerializeField] private TMP_Text infoMessageText;
    [SerializeField] private Button saveQuizButton;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform playerCard in playerGrid.transform)
        {
            playerSettingsCards.Add(playerCard.GetComponent<PlayerSettingsCard>());
        }
        foreach (Transform categoryCard in categoryGrid.transform)
        {
            categorySettingsCards.Add(categoryCard.GetComponent<CategorySettingsCard>());
        }
        playerCountDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(playerCountDropdown); });
        LoadCurrentSettings();
    }

    public void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        int playerCount = dropdown.value + 1;
        for (int i = 0; i < 8; i++)
        {
            Vector3 scale = i < playerCount ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
            playerGrid.transform.GetChild(i).localScale = scale;
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCurrentSettings()
    {
        Quiz quiz = settings.quiz;

        quizNameInput.text = quiz.presetName;
        playerCountDropdown.value = Mathf.Min(Mathf.Max(0, quiz.players.Count - 1), 7);
        DropdownValueChanged(playerCountDropdown);

        for (int i = 0; i < quiz.players.Count; i++)
        {
            playerSettingsCards[i].SetPlayerName(quiz.players[i].name);
            playerSettingsCards[i].SetColor(quiz.players[i].color);
        }

        foreach (QuestionTypeSettings qts in quiz.questionTypeSettingsList)
        {
            int i = (int)qts.type;
            categorySettingsCards[i].SetQuestionAmount(qts.questionAmount);
            categorySettingsCards[i].SetPointsCorrectPlayer(qts.correctPoints);
            categorySettingsCards[i].SetPointsCorrectOthers(qts.correctPointsOther);
            categorySettingsCards[i].SetPointsWrongPlayer(qts.wrongPoints);
            categorySettingsCards[i].SetPointsWrongOthers(qts.wrongPointsOther);
            categorySettingsCards[i].SetGenToggles(qts.excludedGens);
        }

        categoryShuffleToggle.isOn = quiz.shuffleCategories;
        infiniteModeToggle.isOn = quiz.infiniteMode;
        explainRulesToggle.isOn = quiz.explainRules;
    }

    public void SaveCurrentSettings()
    {
        infoMessageText.text = "";
        Quiz quiz = new Quiz();

        quiz.presetName = quizNameInput.text;
        quiz.players = new List<Player>();
        for (int i = 0; i <= playerCountDropdown.value; i++)
        {
            Player player = new Player();
            player.name = playerSettingsCards[i].GetPlayerName();
            player.color = playerSettingsCards[i].GetColor();
            quiz.players.Add(player);
        }

        for (int i = 0; i < quiz.questionTypeSettingsList.Count; i++)
        {
            QuestionTypeSettings qts = new QuestionTypeSettings();
            qts.type = quiz.questionTypeSettingsList[i].type;
            qts.questionAmount = categorySettingsCards[i].GetQuestionAmount();
            qts.correctPoints = categorySettingsCards[i].GetPointsCorrectPlayer();
            qts.correctPointsOther = categorySettingsCards[i].GetPointsCorrectOthers();
            qts.wrongPoints = categorySettingsCards[i].GetPointsWrongPlayer();
            qts.wrongPointsOther = categorySettingsCards[i].GetPointsWrongOthers();
            qts.excludedGens = categorySettingsCards[i].GetExcludedGens();
            quiz.questionTypeSettingsList[i] = qts;
        }

        quiz.shuffleCategories = categoryShuffleToggle.isOn;
        quiz.infiniteMode = infiniteModeToggle.isOn;
        quiz.explainRules = explainRulesToggle.isOn;

        settings.quiz = quiz;

        infoMessageText.text = "Quiz Gespeichert!";
        saveQuizButton.interactable = false;
        StartCoroutine(SaveQuizCoroutine(1.0f));
    }

    IEnumerator SaveQuizCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        infoMessageText.text = "";
        saveQuizButton.interactable = true;
    }
}
