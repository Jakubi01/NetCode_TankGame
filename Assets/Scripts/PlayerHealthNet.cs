using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealthNet : NetworkBehaviour
{
    private const int MaxHealth = 100;
    public NetworkVariable<int> health = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString128Bytes> userId = new("player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Camera _mainCamera;
    private const int TakenDamage = 10;
    
    private void Start()
    {
        _mainCamera = Camera.main;
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            health.Value = MaxHealth;
            userId.Value = BeginSceneGameManager.Instance.UserId;
        }
        
        base.OnNetworkSpawn();
    }

    private void DecHealth()
    {
        int curValue = health.Value - TakenDamage;
        if (curValue <= 0)
        {
            DestroyEventRpc();
        }
        
        UiManagerTank.Instance.UpdateUIInfo(curValue);
        health.Value = curValue;
    }

    [Rpc(SendTo.Server)]
    private void DestroyEventRpc()
    {
        NetworkObject.Despawn();
        BeginSceneGameManager.Instance.UpdateCountId();
        Debug.Log(BeginSceneGameManager.Instance.CountId);
    }

    [Rpc(SendTo.Owner)]
    public void DecHealthRpc()
    {
        DecHealth();
    }
    
    private void OnGUI()
    {
        Vector3 offset = new Vector3(0f, 2f, 0f);
        Vector3 pos = _mainCamera.WorldToScreenPoint(transform.position + offset);
        Rect rect = new Rect(0, 0, 100, 50)
        {
            x = pos.x,
            y = Screen.height - pos.y
        };
        
        string text = $"{userId.Value}: {health.Value}";
        GUI.Label(rect, text);
    }
}
