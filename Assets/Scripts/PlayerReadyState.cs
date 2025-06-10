using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

public class PlayerReadyState : NetworkBehaviour
{
    private static Dictionary<ulong, bool> _readyStates = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
    
    public override void OnNetworkSpawn()
    {
        _readyStates[OwnerClientId] = true;
    }

    public override void OnNetworkDespawn()
    {
        _readyStates.Remove(OwnerClientId);
    }
}
