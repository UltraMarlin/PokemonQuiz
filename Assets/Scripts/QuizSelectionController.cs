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
    [SerializeField] private QuizSettings quizSettings;

    private List<QuizSelectCard> quizSelectCards;

    // Start is called before the first frame update
    void Start()
    {
        if (quizSettings == null) return;
        int previousLength = quizSettings.quizzes.Count;
        quizSettings.quizzes = quizSettings.quizzes.Where(quiz => quiz.presetName.Length > 0).ToList();
        int lengthDifference = previousLength - quizSettings.quizzes.Count;
        quizSettings.selectedQuiz -= lengthDifference;

        int quizIndex = 0;
        quizSelectCards = new List<QuizSelectCard>();
        foreach (Quiz quiz in quizSettings.quizzes)
        {
            GameObject go = Instantiate(quizSelectCardPrefab, quizGrid.transform);
            QuizSelectCard quizSelectCard = go.GetComponent<QuizSelectCard>();
            quizSelectCard.SetQuizName(quiz.presetName);
            quizSelectCard.SetPlayerNames($"Spieler:\n{string.Join(", ", quiz.GetPlayerNames())}");
            int copyQuizIndex = quizIndex;
            go.GetComponent<Button>().onClick.AddListener(delegate {
                Debug.Log("Pressed Quiz number: " + copyQuizIndex);
                quizSettings.selectedQuiz = copyQuizIndex;
                UpdateQuizSelectCardHighlights();
            });
            quizSelectCards.Add(quizSelectCard);
            quizIndex++;
        }

        if (quizGrid.transform.childCount > 0)
        {
            if (quizSettings.selectedQuiz > quizGrid.transform.childCount - 1 || quizSettings.selectedQuiz < 0)
            {
                quizSettings.selectedQuiz = 0;
            }
            quizGrid.transform.GetChild(quizSettings.selectedQuiz).GetComponent<QuizSelectCard>().EnableHighlight();
        }
    }

    public void UpdateQuizSelectCardHighlights()
    {
        for (int i = 0; i < quizSelectCards.Count; i++)
        {
            if (quizSettings.selectedQuiz == i)
                quizSelectCards[i].EnableHighlight();
            else
                quizSelectCards[i].DisableHighlight();
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void CreateNewQuiz()
    {
        quizSettings.CreateNewQuiz();
        SceneManager.LoadScene("Settings");
    }

    public void EditSelectedQuiz()
    {
        if (quizSettings.quizzes.Count == 0) return;
        SceneManager.LoadScene("Settings");
    }
}
