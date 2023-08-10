using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizSelectionController : MonoBehaviour
{
    [SerializeField] private GameObject quizGrid;
    [SerializeField] private GameObject quizSelectCardPrefab;
    [SerializeField] private QuizSettings settings;

    private List<QuizSelectCard> quizSelectCards;

    private FileDataHandler fileDataHandler;

    // Start is called before the first frame update
    void Start()
    {
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
        int previousLength = settings.quizzes.Count;
        settings.quizzes = settings.quizzes.Where(quiz => quiz.presetName.Length > 0).ToList();
        int lengthDifference = previousLength - settings.quizzes.Count;
        settings.selectedQuiz -= lengthDifference;

        int quizIndex = 0;
        quizSelectCards = new List<QuizSelectCard>();
        foreach (Quiz quiz in settings.quizzes)
        {
            GameObject go = Instantiate(quizSelectCardPrefab, quizGrid.transform);
            QuizSelectCard quizSelectCard = go.GetComponent<QuizSelectCard>();
            quizSelectCard.SetQuizName(quiz.presetName);
            quizSelectCard.SetPlayerNames($"Spieler:\n{string.Join(", ", quiz.GetPlayerNames())}");
            int copyQuizIndex = quizIndex;
            go.GetComponent<Button>().onClick.AddListener(delegate {
                Debug.Log("Pressed Quiz number: " + copyQuizIndex);
                settings.selectedQuiz = copyQuizIndex;
                UpdateQuizSelectCardHighlights();
            });
            quizSelectCards.Add(quizSelectCard);
            quizIndex++;
        }

        if (quizGrid.transform.childCount > 0)
        {
            if (settings.selectedQuiz > quizGrid.transform.childCount - 1 || settings.selectedQuiz < 0)
            {
                settings.selectedQuiz = 0;
            }
            quizGrid.transform.GetChild(settings.selectedQuiz).GetComponent<QuizSelectCard>().EnableHighlight();
        }
    }

    public void UpdateQuizSelectCardHighlights()
    {
        for (int i = 0; i < quizSelectCards.Count; i++)
        {
            if (settings.selectedQuiz == i)
                quizSelectCards[i].EnableHighlight();
            else
                quizSelectCards[i].DisableHighlight();
        }
    }

    public void LoadMainMenu()
    {
        fileDataHandler.Save(settings);
        SceneManager.LoadScene("MainMenu");
    }

    public void CreateNewQuiz()
    {
        fileDataHandler.Save(settings);
        settings.CreateNewQuiz();
        SceneManager.LoadScene("Settings");
    }

    public void EditSelectedQuiz()
    {
        if (settings.quizzes.Count == 0) return;
        fileDataHandler.Save(settings);
        SceneManager.LoadScene("Settings");
    }
}
