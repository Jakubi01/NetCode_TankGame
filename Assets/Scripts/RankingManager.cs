using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public GameObject scoreEntryPrefab;
    public Transform scoreListParent;

    private void Start()
    {
        Invoke(nameof(UpdateScoreBoard), 1.0f);
    }

    void UpdateScoreBoard()
    {
        List<PlayerScoreManager> allPlayers = FindObjectsByType<PlayerScoreManager>(FindObjectsSortMode.InstanceID).ToList();

        var sorted = allPlayers.OrderByDescending(p => p.score.Value).ToList();

        // TODO : 텍스트가 하나만 생성되고 모든 플레이어 점수가 통합되어 보임
        foreach (var player in sorted)
        {
            GameObject entry = Instantiate(scoreEntryPrefab, scoreListParent);
            TMP_Text text = entry.GetComponent<TMP_Text>();
            text.text = $"{player.userId.Value} : {player.score.Value}";
        }
    }
}
