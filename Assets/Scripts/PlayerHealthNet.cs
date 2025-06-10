using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealthNet : NetworkBehaviour
{
    private const int MaxHealth = 100;
    private const int TakenDamage = 50;
    
    public NetworkVariable<int> health = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString64Bytes> userId = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private Camera _mainCamera;
    
    private void Start()
    {
        _mainCamera = Camera.main;
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            health.Value = MaxHealth;
            userId.Value = BeginGameManager.Instance.UserId;
        }
        
        base.OnNetworkSpawn();
    }

    private void DecHealth()
    {
        int curValue = health.Value - TakenDamage;
        if (curValue <= 0)
        {
            RequestRespawnServerRpc(OwnerClientId);
            DestroyEventRpc();
            return;
        }
        
        UiManagerTank.Instance.UpdateUIInfo(curValue);
        health.Value = curValue;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRespawnServerRpc(ulong clientId)
    {
        InGameManager.Instance.HandleRespawn(clientId);
    }

    [Rpc(SendTo.Server)]
    private void DestroyEventRpc()
    {
        NetworkObject.Despawn();
    }

    [Rpc(SendTo.Owner)]
    public void DecHealthRpc()
    {
        DecHealth();
    }
    
    private void OnGUI()
    {
        if (!_mainCamera)
        {
            return;
        }
        
        Vector3 offset = new Vector3(0f, 2f, 0f);
        Vector3 pos = _mainCamera.WorldToScreenPoint(transform.position + offset);
        Rect rect = new Rect(0, 0, 100, 50)
        {
            x = pos.x,
            y = Screen.height - pos.y
        };
        
        string text = $"{userId.Value} : {health.Value}";
        GUI.Label(rect, text);
    }
}
