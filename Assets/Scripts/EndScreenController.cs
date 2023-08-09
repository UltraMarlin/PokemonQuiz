using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour
{
    [SerializeField] private GameObject playerRankingList;

    public void DisplayPlayerRankings(List<Result> results)
    {
        playerRankingList.GetComponent<PlayerRankingController>().AddPlayerResults(results);
    }
}
