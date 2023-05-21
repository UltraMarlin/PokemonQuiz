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
    public void AddPointServerRpc()
    {
        QuizSession.instance.AddPoint();
    }

    [ClientRpc]
    public void ShowSolutionClientRpc()
    {
        AdminPanelController.instance.SetSolutionImageTest();
    }
}
