using Unity.Collections;
using Unity.Netcode;

public class PlayerScoreManager : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> userId = new();
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        { 
            SetUserIdServerRpc(BeginGameManager.Instance.UserId);
        }
        
        base.OnNetworkSpawn();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(ulong ownerClientId, int amount)
    { 
        InGameManager.Instance.CachePlayerScore(ownerClientId, userId.Value.Value, amount);
    } 

    [ServerRpc(RequireOwnership = false)]
    private void SetUserIdServerRpc(string userName)
    {
        userId.Value = userName;
    }
}
