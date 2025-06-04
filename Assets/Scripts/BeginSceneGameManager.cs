using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class BeginSceneGameManager : MonoBehaviour
{
    public enum NodeType
    {
        Null = 0,
        Host,
        Client,
        Server
    }

    public NodeType UserNodeType
    {
        get; 
        set;
    }

    public string UserId
    {
        get; 
        set;
    }

    public int CountId
    {
        get;
        private set;
    }

    public float SpeedTank
    {
        get;
        private set;
    }

    public float RotSpeedTank
    {
        get;
        private set;
    }

    public float BulletSpeed
    {
        get;
        private set;
    }

    public static BeginSceneGameManager Instance { get; private set; }

    private void Start()
    {
        InitParam();
    }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void InitParam()
    {
        UserNodeType = NodeType.Null;
        SpeedTank = 6.0f;
        RotSpeedTank = 50.0f;
        BulletSpeed = 10000f;
    }

    private int GetNumClients()
    {
        if (!NetworkManager.Singleton)
        {
            return 0;
        }
        
        System.Collections.Generic.IReadOnlyList<NetworkClient> connectedClients = NetworkManager.Singleton.ConnectedClientsList;
        return connectedClients.Count;
    }

    public void SetConnection(string ipAddress, ushort portNum)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, portNum);
    }

    private void StartHost()
    {
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartHost();
        }
    }

    private void StartServer()
    {
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartServer();
        }
    }

    private void StartClient()
    {
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    public void StartNode()
    {
        if (UserNodeType == NodeType.Host)
        {
            StartHost();
        }
        else if (UserNodeType == NodeType.Client)
        {
            StartClient();
        }
        else if (UserNodeType == NodeType.Server)
        {
            StartServer();
        }
    }

    public void UpdateCountId()
    {
        CountId = GetNumClients();
    }
}
