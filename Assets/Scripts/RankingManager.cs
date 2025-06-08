using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public GameObject scoreEntryPrefab;
    public Transform scoreListParent;

    public static RankingManager Instance{ get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    [ClientRpc]
    private void UpdateScoreBoardClientRpc()
    {
        foreach (Transform child in scoreListParent)
            Destroy(child.gameObject);

        var rankings = InGameManager.Instance.FinalScore;

        GameObject entry = Instantiate(scoreEntryPrefab, scoreListParent);
        TMP_Text text = entry.GetComponent<TMP_Text>();

        foreach (var player in rankings)
        {
            Debug.Log(player.Value.userName);
            text.text += $"{player.Value.userName} : {player.Value.score}" + "\n";
        }

    }

    public void UpdateScoreBoard()
    {
        UpdateScoreBoardClientRpc();
    }
}
