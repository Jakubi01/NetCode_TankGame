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
        // TODO :   모든 접속 유저들의 정보를 수집하는데 성공
        //          그렇지만, Host에서는 userName과 score를 불러오는데 실패하고,
        //          Client에선 UI Update가 이루어지지 않음
        
        Debug.Log("UpdateScoreBoard");
        
        var rankings = InGameManager.Instance.FinalScore;
        
        GameObject entry = Instantiate(scoreEntryPrefab, scoreListParent);
        TMP_Text text = entry.GetComponent<TMP_Text>();
        
        foreach (var player in rankings)
        {
            Debug.Log(player.Value.userName);
            text.text += $"{player.Value.userName} : {player.Value.score}" + "\n";
        }
    }
}
