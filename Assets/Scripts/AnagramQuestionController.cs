using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AnagramQuestionController : MonoBehaviour, IQuestionController
{
    [SerializeField] private TextMeshProUGUI anagramText;
    [SerializeField] private TextMeshProUGUI solutionText;

    [SerializeField] private GameObject imageContainer;
    [SerializeField] private GameObject stretchImagePrefab;

    public AnagramQuestion anagramQuestionData;
    private int currentStep = 0;
    private List<bool> markedLetters = new List<bool>();

    public void SetData(IQuestion questionData)
    {
        anagramQuestionData = questionData as AnagramQuestion;
    }

    public void StartQuestion()
    {
        if (anagramQuestionData == null) return;
        anagramText.text = anagramQuestionData.anagramString;
        for (int i = 0; i < anagramQuestionData.anagramString.Length; i++)
        {
            markedLetters.Add(false);
        }
    }

    public void NextQuestionStep()
    {
        if (currentStep >= anagramQuestionData.pokemonName.Length)
        {
            Debug.Log("All letters revealed");
            return;
        }
        char newlyRevealedChar = char.ToUpper(anagramQuestionData.pokemonName[currentStep]);
        currentStep++;
        string newAnagramText = "";
        string newHintText = "";

        for (int i = 0; i < anagramQuestionData.pokemonName.Length; i++) {
            if (i < currentStep)
            {
                newHintText += MDUnderline(anagramQuestionData.pokemonName[i]) + " ";
            }
            else
            {
                newHintText += "_ ";
            }
        }

        List<int> indices = IndicesOf(anagramQuestionData.anagramString.ToUpper(), newlyRevealedChar);
        foreach (int index in indices)
        {
            if(!markedLetters[index])
            {
                markedLetters[index] = true;
                break;
            }
        }

        for (int i = 0; i < markedLetters.Count; i++)
        {
            newAnagramText += markedLetters[i] ? MDGrey(anagramQuestionData.anagramString[i]) : anagramQuestionData.anagramString[i];
        }

        anagramText.text = newAnagramText.Trim();
        solutionText.text = newHintText.Trim();
    }

    public void ShowSolution()
    {
        currentStep = 99999;
        GameObject solutionImage = Instantiate(stretchImagePrefab, imageContainer.transform);
        solutionImage.GetComponent<Image>().sprite = anagramQuestionData.sprite;
        solutionText.characterSpacing = 0;
        solutionText.text = anagramQuestionData.pokemonName;
        anagramText.text = anagramQuestionData.anagramString;
    }

    public void ResetDisplay()
    {
        return;
    }

    public string MDGrey(char s)
    {
        return $"<color=#FFFFFF12>{s}</color>";
    }

    public string MDUnderline(char s)
    {
        return s.ToString();
        //return $"<u>{s}</u>";
    }

    public List<int> IndicesOf(string str, char c)
    {
        var indices = new List<int>();
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == c)
            {
                indices.Add(i);
            }
        }
        return indices;
    }
}
