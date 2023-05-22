using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerControlPanel : MonoBehaviour
{
    public int playerID;

    [SerializeField] private TextMeshProUGUI playerNameText;

    public void CorrectAnswer()
    {
        if (AdminPanelController.instance.adminPanelUser == null) return;
        AdminPanelController.instance.adminPanelUser.CorrectAnswerServerRpc(playerID);
    }

    public void WrongAnswer()
    {
        if (AdminPanelController.instance.adminPanelUser == null) return;
        AdminPanelController.instance.adminPanelUser.WrongAnswerServerRpc(playerID);
    }

    public void AddPoint()
    {
        if (AdminPanelController.instance.adminPanelUser == null) return;
        AdminPanelController.instance.adminPanelUser.AddPointServerRpc(playerID);
    }

    public void SubtractPoint()
    {
        if (AdminPanelController.instance.adminPanelUser == null) return;
        AdminPanelController.instance.adminPanelUser.SubtractPointServerRpc(playerID);
    }

    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
    }
}
