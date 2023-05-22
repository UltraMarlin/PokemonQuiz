using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerPanelController : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image smallBackground;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI pointsText;

    public void SetPlayerColor(PlayerColor playerColor)
    {
        backgroundImage.color = QuizColors.main[(int)playerColor];
        smallBackground.color = QuizColors.light[(int)playerColor];
        playerNameText.color = QuizColors.dark[(int)playerColor];
        pointsText.color = QuizColors.dark[(int)playerColor];
    }

    public void SetPlayerNameText(string text)
    {
        playerNameText.text = text;
    }

    public void SetPlayer(Player player)
    {
        SetPlayerColor(player.color);
        SetPlayerNameText(player.name);
    }

    public void SetPlayerPointsText(string text)
    {
        pointsText.text = text;
    }
}
