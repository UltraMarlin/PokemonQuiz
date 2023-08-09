using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingEntryController : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerPointsText;
    
    public void SetFontsize(float fontSize)
    {
        rankText.fontSize = fontSize;
        playerNameText.fontSize = fontSize;
        playerPointsText.fontSize = fontSize;
    }

    public void SetRankText(string text)
    {
        rankText.text = text;
    }

    public void SetPlayerNameText(string text)
    {
        playerNameText.text = text;
    }

    public void SetPlayerPointsText(string text)
    {
        playerPointsText.text = text;
    }
}
