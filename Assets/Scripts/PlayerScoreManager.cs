using Unity.Collections;
using Unity.Netcode;

public class PlayerScoreManager : NetworkBehaviour
{
    public NetworkVariable<int> score 
        = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public NetworkVariable<FixedString64Bytes> userId 
        = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        { 
            score.Value = 0;
            SetUserIdServerRpc(BeginGameManager.Instance.UserId);
        }
        
        base.OnNetworkSpawn();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int amount)
    { 
        score.Value += amount;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetUserIdServerRpc(string userName)
    {
        userId.Value = userName;
    }
}
