using Unity.Collections;
using Unity.Netcode;

public class PlayerScoreManager : NetworkBehaviour
{
    public NetworkVariable<int> score 
        = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public NetworkVariable<FixedString64Bytes> userId 
        = new("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        { 
            score.Value = 0;
            userId.Value = BeginGameManager.Instance.UserId;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int amount)
    { 
        score.Value += amount;
    }
}
