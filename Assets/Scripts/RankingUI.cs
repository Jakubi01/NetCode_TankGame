using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingUI : MonoBehaviour
{
    public TMP_Text rankingText;

    // 서버가 ClientRpc로 호출하거나 직접 호출
    public void ShowRanking((ulong clientId, int score)[] rankings, Dictionary<ulong, string> clientIdToUserName)
    {
        string text = "Ranking\n";

        for (int i = 0; i < rankings.Length; i++)
        {
            var rank = rankings[i];
            string value = clientIdToUserName.GetValueOrDefault(rank.clientId, "Player");
            text += $"{i + 1}. {value} - {rank.score} points\n";
        }

        rankingText.text = text;
    }
}
