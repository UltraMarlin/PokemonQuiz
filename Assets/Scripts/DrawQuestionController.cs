using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private GameObject imageContainer;
    [SerializeField] private GameObject stretchImagePrefab;
    [SerializeField] private TextMeshProUGUI solutionText;

    private Image drawImageComponent;

    public DrawQuestion drawQuestionData;

    public void SetData(IQuestion questionData)
    {
        drawQuestionData = questionData as DrawQuestion;
    }

    public void StartQuestion()
    {
        if (drawQuestionData == null) return;
    }

    public void NextQuestionStep()
    {
        return;
    }

    public void ShowSolution()
    {
        return;
    }

    public void ResetDisplay()
    {
        return;
    }
}
