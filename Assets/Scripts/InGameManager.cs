using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class InGameManager : NetworkBehaviour 
{
    public Transform[] spawnPoints;
    
    public static InGameManager Instance { get; private set; }
    
    public int ConnectedUserNum { get; set; }

    public float PlayTime { get; private set; }
    
    public bool CanMove { get; private set; }

    public Dictionary<ulong, (FixedString64Bytes userName, int score)> FinalScore = new();

    public Dictionary<ulong, (string userName, int score)> ScoreCache = new();
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayTime = 30;
        CanMove = false;
    }

    [ClientRpc]
    private void StartGameCountDownClientRpc()
    {
        CanMove = true;
        UiManagerTank.Instance.StartTimer();
    }

    [ClientRpc]
    private void StartScoreboardCountDownClientRpc()
    {
        UiManagerTank.Instance.StartScoreboardTimer();
    }

    public void StartGame()
    {
        StartGameCountDownClientRpc();
    }

    public void StartScoreboardTimer()
    {
        StartScoreboardCountDownClientRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void CollectScoresServerRpc()
    {
        if (!IsServer)
        {
            return;
        }
            
        FinalScore.Clear();
        
        var player = NetworkManager.Singleton.ConnectedClients.Values;
        foreach (var nc in player)
        {
            if (!nc.PlayerObject)
            {
                continue;
            }

            if (!ScoreCache.ContainsKey(nc.ClientId))
            {
                ScoreCache.Add(
                    nc.ClientId
                    , (nc.PlayerObject.GetComponent<PlayerScoreManager>().userId.Value.Value
                        , nc.PlayerObject.GetComponent<PlayerScoreManager>().score.Value)
                );
            }
        }
        
        foreach (var kvp in ScoreCache)
        {
            FinalScore[kvp.Key] = kvp.Value;
        }
        
        var scoreList = new List<ScoreData>();
        foreach (var entry in FinalScore.Values)
        {
            scoreList.Add(new ScoreData
            {
                UserName = entry.userName,
                Score = entry.score
            });
        }
        
        // TODO : List 정렬
        UpdateScoreBoardClientRpc(scoreList.ToArray());
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SubmitScoreServerRpc(ulong clientId, string userName, int score)
    {
        if (!ScoreCache.ContainsKey(clientId))
        {
            ScoreCache.Add(clientId, (userName, score));
        }
    }
    
    [ClientRpc]
    private void UpdateScoreBoardClientRpc(ScoreData[] scoreData)
    {
        UiManagerTank.Instance.UpdateScoreUI(scoreData);
    }
}
