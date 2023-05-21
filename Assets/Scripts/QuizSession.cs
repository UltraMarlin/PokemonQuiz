using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class QuizSession : MonoBehaviour
{
    public static QuizSession instance;
    private AdminPanelUser adminPanelUser;

    [SerializeField] private GameObject playerPanel;
    private int playerPoints = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        NetworkManager.Singleton.StartServer();
    }

    void Update()
    {
        
    }

    public void RegisterAdminPanelUser(AdminPanelUser user)
    {
        adminPanelUser = user;
    }

    public void AddPoint()
    {
        playerPoints++;
        playerPanel.transform.GetChild(0).GetComponent<PlayerPanelController>().SetPlayerPointsText(playerPoints.ToString());
    }

    public void DisplayAdminSolution()
    {
        adminPanelUser.ShowSolutionClientRpc();
    }
}
