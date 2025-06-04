using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManagerBegin : MonoBehaviour
{
    public TMP_InputField inputIp;
    public TMP_InputField inputPort;
    public TMP_InputField inputUserId;

    public GameObject StartPanel;
    public GameObject StartBtn;
    public GameObject ExitBtn;

    public static UiManagerBegin Instance { get; private set; }

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
    }

    private void Start()
    {
        StartPanel.SetActive(false);
    }

    public void StartHost()
    {
        BeginSceneGameManager.Instance.UserNodeType = BeginSceneGameManager.NodeType.Host;
        UpdateConnection();
        SceneManager.LoadScene("TankScene");
    }

    public void StartClient()
    {
        BeginSceneGameManager.Instance.UserNodeType = BeginSceneGameManager.NodeType.Client;
        UpdateConnection();
        SceneManager.LoadScene("TankScene");
    }

    public void StartServer()
    {
        BeginSceneGameManager.Instance.UserNodeType = BeginSceneGameManager.NodeType.Server;
        UpdateConnection();
        SceneManager.LoadScene("TankScene");
    }

    private void UpdateConnection()
    {
        string ipAddress = inputIp.text;
        
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = "127.0.0.1";
            
            string port = inputPort.text;
            if (string.IsNullOrEmpty(port))
            {
                port = "7777";
            }

            string userId = inputUserId.text;
            if (string.IsNullOrEmpty(userId))
            {
                userId = "player";
            }

            ushort portNum = ushort.Parse(port);
            BeginSceneGameManager.Instance.SetConnection(ipAddress, portNum);
            BeginSceneGameManager.Instance.UserId = userId;
        }
    }

    public void OnStartBtnClicked()
    {
        StartPanel.SetActive(true);

        StartBtn.GetComponent<Button>().interactable = false;
        ExitBtn.GetComponent<Button>().interactable = false;
    }

    public void OnExitBtnClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif
    }

    public void OnHidePanelBtnClicked()
    {
        StartPanel.SetActive(false);
        
        StartBtn.GetComponent<Button>().interactable = true;
        ExitBtn.GetComponent<Button>().interactable = true;
    }
}
