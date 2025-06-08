using System.Collections;
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
            var scoreManager = GetComponent<PlayerScoreManager>();
            SubmitScoreServerRpc(OwnerClientId, scoreManager.userId.Value.Value, scoreManager.score.Value);

            // TODO : 리스폰이 안됨..
            StartCoroutine(nameof(RespawnCoroutine));
            DestroyEventRpc();
            return;
        }
        
        UiManagerTank.Instance.UpdateUIInfo(curValue);
        health.Value = curValue;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SubmitScoreServerRpc(ulong clientId, string userName, int score)
    {
        InGameManager.Instance.SubmitScoreServerRpc(clientId, userName, score);
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
    
    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(3f); // 3초 후 리스폰

        if (IsServer)
        {
            Vector3 spawnPos = GetComponent<PlayerNetworkManager>().GetRandomSpawnPoint();
            transform.position = spawnPos;
            health.Value = MaxHealth;

            gameObject.GetComponent<NetworkObject>().Spawn(true);
        }
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
