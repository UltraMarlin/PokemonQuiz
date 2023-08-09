using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizSelectCard : MonoBehaviour
{
    [SerializeField] private Image selectedBgImage;
    [SerializeField] private TMP_Text quizNameText;
    [SerializeField] private TMP_Text playerNamesText;

    public void SetQuizName(string quizName)
    {
        quizNameText.text = quizName;
    }

    public void SetPlayerNames(string playerNames)
    {
        playerNamesText.text = playerNames;
    }

    public void EnableHighlight()
    {
        selectedBgImage.color = new Color(1f, 1f, 1f, 0.09f);
    }

    public void DisableHighlight()
    {
        selectedBgImage.color = new Color(1f, 1f, 1f, 0f);
    }
}
