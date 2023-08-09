using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Result
{
    public string playerName;
    public int playerPoints;
}

public class PlayerRankingController : MonoBehaviour
{
    [SerializeField] private GameObject rankingEntryPrefab;

    public void AddPlayerResults(List<Result> results)
    {
        int rank = 0;
        results.OrderByDescending(result => result.playerPoints).ToList().ForEach(result =>
        {
            rank++;
            GameObject go = Instantiate(rankingEntryPrefab, transform);
            RankingEntryController rankingEntryController = go.GetComponent<RankingEntryController>();
            if (rank == 1)
            {
                rankingEntryController.SetFontsize(62);
            }
            rankingEntryController.SetRankText($"{rank}.");
            rankingEntryController.SetPlayerNameText(result.playerName);
            rankingEntryController.SetPlayerPointsText(result.playerPoints.ToString());
        });
    }
}
