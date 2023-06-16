using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlurQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private GameObject imageContainer;
    [SerializeField] private GameObject stretchImagePrefab;
    [SerializeField] private TextMeshProUGUI solutionText;

    private Image blurImageComponent;

    public BlurQuestion blurQuestionData;

    public void SetData(IQuestion questionData)
    {
        blurQuestionData = questionData as BlurQuestion;
    }

    public void StartQuestion()
    {
        if (blurQuestionData == null) return;
        GameObject blurImage = Instantiate(stretchImagePrefab, imageContainer.transform);
        blurImageComponent = blurImage.GetComponent<Image>();
        blurImageComponent.sprite = blurQuestionData.sprite;
        blurImageComponent.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
    }

    public void NextQuestionStep()
    {
        return;
    }

    public void ShowSolution()
    {
        blurImageComponent.color = Color.white;
        solutionText.text = blurQuestionData.pokemonName;
    }

    public void ResetDisplay()
    {
        return;
    }
}
