using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerReadyState : NetworkBehaviour
{
    public static Dictionary<ulong, bool> ReadyStates = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SubmitReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        ReadyStates[clientId] = true;
  
        if (AllPlayersReady())
        {
            InGameManager.Instance.StartGame();
        }
    }
    
    private bool AllPlayersReady()
    {
        if (ReadyStates.Count < NetworkManager.Singleton.ConnectedClients.Count)
        {
            return false;
        }

        if (InGameManager.Instance.ConnectedUserNum <= 1)
        {
            return false;
        }
 
        // Lambda
        return ReadyStates.Values.All(isReady => isReady);
    }
    
    public override void OnNetworkSpawn()
    {
        ReadyStates[OwnerClientId] = true;
    }

    public override void OnNetworkDespawn()
    {
        ReadyStates.Remove(OwnerClientId);
    }
}
