using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AdminPanelUser : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            AdminPanelController.instance.RegisterAdminPanelUser(this);
        } else if (IsServer)
        {
            QuizSession.instance.RegisterAdminPanelUser(this);
        }
    }

    [ServerRpc]
    public void CorrectAnswerServerRpc(int playerId)
    {
        QuizSession.instance.CorrectAnswerFrom(playerId);
    }

    [ServerRpc]
    public void WrongAnswerServerRpc(int playerId)
    {
        QuizSession.instance.WrongAnswerFrom(playerId);
    }

    [ServerRpc]
    public void AddPointServerRpc(int playerId)
    {
        QuizSession.instance.AddPointsToPlayer(playerId, 1);
    }

    [ServerRpc]
    public void SubtractPointServerRpc(int playerId)
    {
        QuizSession.instance.AddPointsToPlayer(playerId, -1);
    }

    [ClientRpc]
    public void ShowSolutionClientRpc()
    {
        AdminPanelController.instance.SetSolutionImageTest();
    }
}
