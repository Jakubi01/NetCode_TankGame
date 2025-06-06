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
        // TODO : FinalScore를 읽어오는데 Host의 정보만 읽어오고 있음 수정
        
        Debug.Log("UpdateScoreBoard");
        
        var rankings = InGameManager.Instance.FinalScore;
        Debug.Log(rankings.Count);
        
        GameObject entry = Instantiate(scoreEntryPrefab, scoreListParent);
        TMP_Text text = entry.GetComponent<TMP_Text>();
        
        foreach (var player in rankings)
        {
            Debug.Log(player.Value.userName);
            text.text += $"{player.Value.userName} : {player.Value.score}" + "\n";
        }
    }
}
