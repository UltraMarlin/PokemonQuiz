using System.Collections.Generic;
using UnityEngine;

public struct Result
{
    public int playerId;
    public string playerName;
    public int playerPoints;
    public Color color;
}

public class PlayerRankingController : MonoBehaviour
{
    [SerializeField] private GameObject rankingEntryPrefab;

    public void AddPlayerResults(List<Result> results)
    {
        int rank = 0;
        results.ForEach(result =>
        {
            rank++;
            GameObject go = Instantiate(rankingEntryPrefab, transform);
            RankingEntryController rankingEntryController = go.GetComponent<RankingEntryController>();
            if (rank == 1)
            {
                rankingEntryController.HighlightWinner(result.color);
            }
            rankingEntryController.SetRankText($"{rank}.");
            rankingEntryController.SetPlayerNameText(result.playerName);
            rankingEntryController.SetPlayerPointsText(result.playerPoints.ToString());
        });
    }
}
