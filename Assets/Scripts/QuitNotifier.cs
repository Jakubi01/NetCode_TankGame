using Unity.Netcode;

public class QuitNotifier : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void NotifyQuitServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            var playerObj = client.PlayerObject;
            if (playerObj)
            {
                playerObj.Despawn();
            }
        }
    }
}
