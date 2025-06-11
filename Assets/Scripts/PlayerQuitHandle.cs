using Unity.Netcode;
using UnityEngine;

public class PlayerQuitHandle : MonoBehaviour
{
    public static PlayerQuitHandle Instance { get; private set; }

    private void Awake()
    {
        if (Instance&& Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsConnectedClient)
        {
            return;
        }

        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (playerObject && playerObject.TryGetComponent(out QuitNotifier notifier))
        {
            notifier.NotifyQuitServerRpc();
        }
    }
}
