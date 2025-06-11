using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerReadyState : NetworkBehaviour
{
    private Dictionary<ulong, bool> _readyStates = new();

    public static PlayerReadyState Instance;
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        _readyStates.Clear();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SubmitReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        _readyStates[clientId] = true;
  
        if (AllPlayersReady())
        {
            InGameManager.Instance.StartGame();
        }
    }
    
    private bool AllPlayersReady()
    {
        if (_readyStates.Count < NetworkManager.Singleton.ConnectedClients.Count)
        {
            return false;
        }

        if (InGameManager.Instance.ConnectedUserNum <= 1)
        {
            return false;
        }
        
        return _readyStates.Values.All(isReady => isReady);
    }
}
