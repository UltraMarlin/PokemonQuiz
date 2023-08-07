using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategorySettingsCard : MonoBehaviour
{
    [SerializeField] private TMP_Text categoryNameText;
    [SerializeField] private TMP_InputField questionAmountInput;
    [SerializeField] private List<Toggle> genToggles;

    [SerializeField] private TMP_InputField pointsCorrectPlayerInput;
    [SerializeField] private TMP_InputField pointsCorrectOthersInput;
    [SerializeField] private TMP_InputField pointsWrongPlayerInput;
    [SerializeField] private TMP_InputField pointsWrongOthersInput;
    
    public int GetQuestionAmount()
    {
        int.TryParse(questionAmountInput.text, out int questionAmount);
        return questionAmount;
    }

    public void SetQuestionAmount(int questionAmount)
    {
        questionAmountInput.text = questionAmount.ToString();
    }

    public List<PokemonGen> GetExcludedGens()
    {
        List<PokemonGen> excludedGens = new();
        for (int i = 0; i < genToggles.Count; i++)
        {
            if (!genToggles[i].isOn)
            {
                excludedGens.Add((PokemonGen)i);
            }
        }
        return excludedGens;
    }

    public void SetGenToggles(List<PokemonGen> excludedGens)
    {
        for (int i = 0; i < genToggles.Count; i++)
        {
            genToggles[i].isOn = !excludedGens.Contains((PokemonGen)i);
        }
    }

    public int GetPointsCorrectPlayer()
    {
        int.TryParse(pointsCorrectPlayerInput.text, out int pointsCorrectPlayer);
        return pointsCorrectPlayer;
    }

    public void SetPointsCorrectPlayer(int pointsCorrectPlayer)
    {
        pointsCorrectPlayerInput.text = pointsCorrectPlayer.ToString();
    }

    public int GetPointsCorrectOthers()
    {
        int.TryParse(pointsCorrectOthersInput.text, out int pointsCorrectOthers);
        return pointsCorrectOthers;
    }

    public void SetPointsCorrectOthers(int pointsCorrectOthers)
    {
        pointsCorrectOthersInput.text = pointsCorrectOthers.ToString();
    }

    public int GetPointsWrongPlayer()
    {
        int.TryParse(pointsWrongPlayerInput.text, out int pointsWrongPlayer);
        return pointsWrongPlayer;
    }

    public void SetPointsWrongPlayer(int pointsWrongPlayer)
    {
        pointsWrongPlayerInput.text = pointsWrongPlayer.ToString();
    }

    public int GetPointsWrongOthers()
    {
        int.TryParse(pointsWrongOthersInput.text, out int pointsWrongOthers);
        return pointsWrongOthers;
    }

    public void SetPointsWrongOthers(int pointsWrongOthers)
    {
        pointsWrongOthersInput.text = pointsWrongOthers.ToString();
    }
}
