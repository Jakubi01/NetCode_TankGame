using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManagerBegin : MonoBehaviour
{
    public TMP_InputField inputIp;
    public TMP_InputField inputPort;
    public TMP_InputField inputUserId;

    public GameObject startPanel;
    public GameObject startBtn;
    public GameObject exitBtn;
    
    private const string DefaultIpAddress = "127.0.0.1";
    private const string DefaultUserName = "player";
    private const string DefaultPort = "7777";

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
        startPanel.SetActive(false);
        SoundManager.Instance.PlaySound(SoundType.ThemeBegin, true);
    }

    public void StartHost()
    {
        BeginGameManager.Instance.UserNodeType = BeginGameManager.NodeType.Host;
        UpdateConnection();
        LoadTankScene();
    }

    public void StartClient()
    {
        BeginGameManager.Instance.UserNodeType = BeginGameManager.NodeType.Client;
        UpdateConnection();
        LoadTankScene();
    }

    public void StartServer()
    {
        BeginGameManager.Instance.UserNodeType = BeginGameManager.NodeType.Server;
        UpdateConnection();
        LoadTankScene();
    }

    private void LoadTankScene()
    {
        SoundManager.Instance.StopPlaySound(SoundType.ThemeBegin);
        
        SceneManager.LoadScene("TankScene");
    }

    private void UpdateConnection()
    {
        if (string.IsNullOrEmpty(inputIp.text))
        {
            inputIp.text = DefaultIpAddress;
        }
        
        if (string.IsNullOrEmpty(inputPort.text))
        {
            inputPort.text = DefaultPort;
        }

        if (string.IsNullOrEmpty(inputUserId.text))
        {
            inputUserId.text = DefaultUserName;
        }

        ushort portNum = ushort.Parse(inputPort.text);
        BeginGameManager.Instance.SetConnection(inputIp.text, portNum);
        BeginGameManager.Instance.UserId = inputUserId.text;
    }

    public void OnStartBtnClicked()
    {
        startPanel.SetActive(true);

        startBtn.GetComponent<Button>().interactable = false;
        exitBtn.GetComponent<Button>().interactable = false;
    }

    public void OnExitBtnClicked()
    {
        SoundManager.Instance.StopPlaySound(SoundType.ThemeBegin);
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif
    }

    public void OnHidePanelBtnClicked()
    {
        startPanel.SetActive(false);
        
        startBtn.GetComponent<Button>().interactable = true;
        exitBtn.GetComponent<Button>().interactable = true;
    }
}
