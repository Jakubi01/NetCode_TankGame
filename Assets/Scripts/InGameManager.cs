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
    
    public bool CanMove { get; set; }

    public Dictionary<ulong, (FixedString64Bytes userName, int score)> FinalScore = new();
    
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
    public void StartGameCountDownClientRpc()
    {
        CanMove = true;
        UiManagerTank.Instance.StartTimerClient();
    }

    public void StartGame()
    {
        StartGameCountDownClientRpc();
    }

    public void CollectScores()
    {
        FinalScore.Clear();
        var allPlayers = FindObjectsByType<PlayerScoreManager>(FindObjectsSortMode.InstanceID);
        foreach (var player in allPlayers)
        {
            FinalScore[player.OwnerClientId] = (player.userId.Value, player.score.Value); 
        }
    }
}
