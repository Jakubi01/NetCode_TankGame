using UnityEngine;
using Unity.Netcode;

public class InGameManager : NetworkBehaviour 
{
    public Transform[] spawnPoints;
    
    public static InGameManager Instance { get; private set; }
    public int ConnectedUserNum { get; set; }

    public float PlayTime { get; private set; }
    
    public bool CanMove { get; set; }
    
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
}
