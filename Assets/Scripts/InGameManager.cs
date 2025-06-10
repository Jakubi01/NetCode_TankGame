using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class InGameManager : NetworkBehaviour 
{
    public Transform[] spawnPoints;

    public GameObject playerPrefab;
    
    public static InGameManager Instance { get; private set; }
    
    public int ConnectedUserNum { get; set; }

    public float PlayTime { get; private set; }
    
    public bool CanMove { get; private set; }
    
    private Dictionary<ulong, (string userId, int Score)> _playerCache = new();
    
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
        
        if (_playerCache.Count > 0)
        {
            _playerCache.Clear();
        }
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
        
        var scoreList = new List<ScoreData>();
        foreach (var entry in _playerCache.Values)
        {
            scoreList.Add(new ScoreData
            {
                UserName = entry.userId,
                Score = entry.Score
            });
        }
        
        scoreList.Sort((a, b) => b.Score.CompareTo(a.Score));
        UpdateScoreBoardClientRpc(scoreList.ToArray());
    }

    public void CachePlayerScore(ulong clientId, string userName, int score)
    {
        if (_playerCache.TryGetValue(clientId, out var oldData))
        {
            _playerCache[clientId] = (userName, oldData.Score += score);
        }
        else
        {
            _playerCache[clientId] = (userName, score);
        }
    }
    
    [ClientRpc]
    private void UpdateScoreBoardClientRpc(ScoreData[] scoreData)
    {
        UiManagerTank.Instance.UpdateScoreUI(scoreData);
    }

    public void HandleRespawn(ulong clientId)
    {
        StartCoroutine(RespawnCoroutine(clientId));
    }

    private IEnumerator RespawnCoroutine(ulong clientId)
    {
        yield return new WaitForSeconds(3f);

        var spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        var player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}
