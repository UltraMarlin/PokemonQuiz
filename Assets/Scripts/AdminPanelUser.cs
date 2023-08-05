using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AdminPanelUser : NetworkBehaviour
{
    private NetworkVariable<int> nwv_CurrentQuestionType = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            AdminPanelController.instance.RegisterAdminPanelUser(this);
            nwv_CurrentQuestionType.OnValueChanged += OnQuestionTypeChange;
        } else if (IsServer)
        {
            QuizSession.instance.RegisterAdminPanelUser(this);
        }
    }

    [ServerRpc]
    public void NextQuestionServerRpc()
    {
        nwv_CurrentQuestionType.Value = QuizSession.instance.NextQuestion();
    }

    [ServerRpc]
    public void NextQuestionStepServerRpc()
    {
        QuizSession.instance.NextQuestionStep();
    }

    [ServerRpc]
    public void ShowSolutionServerRpc()
    {
        QuizSession.instance.ShowSolution();
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
    public void ShowSolutionClientRpc(string solutionString)
    {
        AdminPanelController.instance.SetSolutionString(solutionString);
    }

    [ServerRpc]
    public void SendCategorySwitchServerRpc(int categoryType)
    {
        QuizSession.instance.SwitchToQuestionType(categoryType);
    }

    public void OnQuestionTypeChange(int previous, int current)
    {
        if (IsClient)
        {
            AdminPanelController.instance.SetCurrentQuestionType(current);
        }
    }

    [ClientRpc]
    public void SetNextStepButtonTextClientRpc(int id)
    {
        AdminPanelController.instance.SetNextStepButtonText(id);
    }
}
