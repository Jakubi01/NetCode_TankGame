using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerReadyState : NetworkBehaviour
{
    private static Dictionary<ulong, bool> _readyStates = new();
    
    public static PlayerReadyState Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SubmitReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        _readyStates[clientId] = true;

        if (AllPlayersReady())
        {
            Debug.Log("모든 플레이어가 Ready. 게임 시작");
            // InGameManager.Instance.StartGameCountDownRpc();
        }
    }
    
    private bool AllPlayersReady()
    {
        if (_readyStates.Count < NetworkManager.Singleton.ConnectedClients.Count)
        {
            return false;
        }

        // if (InGameManager.Instance.ConnectedUserNum <= 1)
        // {
        //     return false;
        // }
        
        // UiManagerTank.Instance.holdOnBox.SetActive(false);
        
        // Lambda
        return _readyStates.Values.All(isReady => isReady);
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _readyStates[OwnerClientId] = false;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            _readyStates.Remove(OwnerClientId);
        }
    }
}
