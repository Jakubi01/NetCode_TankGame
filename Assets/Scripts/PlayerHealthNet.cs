using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        // TODO : Leaderboard 만들기
        //        Score, Id 2개면 될 듯  
        //        .
        //        1. 어떻게 점수를 올릴것인가
        //        2. 어떻게 모든 클라이언트들을 모아올 것인가
        //           ㄴ 접속할 때 마다 BeginGameManager에서 PlayerPref로 string으로 Id 전송
        //        3. 어떻게 리스트를 표시할것인가
        //
        // PlayerPrefs.SetInt("PlayerHealth", health.Value);
        // PlayerPrefs.Save();
        //
        // Debug.Log(PlayerPrefs.GetInt("PlayerHealth"));
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

        // TODO : 인게임 씬에서 health가 0이 되면 Retry Panel.SetActive(true)
        //        5분 타이머가 끝나면 모두 EndScene으로 이동하고, Kill point 보여주기
        SceneManager.LoadScene("EndScene");
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
