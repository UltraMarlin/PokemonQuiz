using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;

public class AnagramQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private TextMeshProUGUI anagramText;
    [SerializeField] private TextMeshProUGUI solutionText;

    [SerializeField] private GameObject imageContainer;
    [SerializeField] private GameObject stretchImagePrefab;

    public AnagramQuestion anagramQuestionData;

    public void SetData(IQuestion questionData)
    {
        anagramQuestionData = questionData as AnagramQuestion;
    }

    public void StartQuestion()
    {
        if (anagramQuestionData == null) return;
        anagramText.text = anagramQuestionData.anagramString;
    }

    public void NextQuestionStep()
    {
        return;
    }

    public void ShowSolution()
    {
        GameObject solutionImage = Instantiate(stretchImagePrefab, imageContainer.transform);
        solutionImage.GetComponent<Image>().sprite = anagramQuestionData.sprite;
        solutionText.text = anagramQuestionData.pokemonName;
    }

    public void ResetDisplay()
    {
        return;
    }
}
