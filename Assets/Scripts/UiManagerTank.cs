using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct ScoreData : INetworkSerializable
{
    public FixedString64Bytes UserName;
    public int Score;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref UserName);
        serializer.SerializeValue(ref Score);
    }
}

public class UiManagerTank : MonoBehaviour
{
    public TMP_Text textUserInfo;
    public TMP_Text textScore;
    public TMP_Text textTimer;
    public TMP_Text textScoreboardTimer;
    public Slider sliderHealth;
    public GameObject rankingPanel;
    public GameObject fadeInImage;
    private Color _fadeInImageColor;

    public int health;
    public int maxHealth = 100;
    private float _playTime;
    private float _showScoreboardTime;
    
    private bool ShouldStartCountDown { get; set; }
    private bool ShouldStartScoreboardTimer { get; set; }
    
    public static UiManagerTank Instance { get; private set; }

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
        BeginGameManager.Instance.StartNode();
        BeginGameManager.Instance.UpdateCountId();

        InitUI();
    }

    private void InitUI()
    {
        health = maxHealth;
        
        UpdateTextUserInfo();
        UpdateSliderHealth();
        Invoke(nameof(CheckCountId), 2.0f);

        ShouldStartCountDown = false;
        ShouldStartScoreboardTimer = false;

        _playTime = 5f;// InGameManager.Instance.PlayTime;
        _showScoreboardTime = 10;
        
        textTimer.text = InGameManager.Instance.PlayTime.ToString("F1");
        textScoreboardTimer.text = InGameManager.Instance.PlayTime.ToString("F1");
        textScore.text = string.Empty;
        
        rankingPanel.SetActive(false);
        fadeInImage.SetActive(false);
        _fadeInImageColor = fadeInImage.GetComponent<Image>().color;
    }

    private void Update()
    {
        if (ShouldStartCountDown)
        {
            UpdateCountDownTimer();
        }

        if (ShouldStartScoreboardTimer)
        {
            UpdateScoreBoardTimer();
        }
    }

    public void StartTimer()
    {
        ShouldStartCountDown = true;
    }

    public void StartScoreboardTimer()
    {
        ShouldStartScoreboardTimer = true;
    }

    private void UpdateCountDownTimer()
    {
        if (!textTimer)
        {
            return;
        }
            
        _playTime -= Time.deltaTime;
        textTimer.text = _playTime.ToString("F1");

        if (!(_playTime <= 0))
        {
            return;
        }
        
        ShouldStartCountDown = false;
        InGameManager.Instance.StartScoreboardTimer();
        
        rankingPanel.SetActive(true);
        InGameManager.Instance.CollectScoresServerRpc();
    }

    private void UpdateScoreBoardTimer()
    {
        if (!textScoreboardTimer)
        {
            return;
        }

        InGameManager.Instance.CanMove = false;
        _showScoreboardTime -= Time.deltaTime;
        textScoreboardTimer.text = _showScoreboardTime.ToString("F1");

        if (!(_showScoreboardTime <= 0f))
        {
            return;
        }

        ShouldStartScoreboardTimer = false;
        StartFadeIn();
    }
    
    public void UpdateUIInfo(int value)
    {
        health = value;

        UpdateTextUserInfo();
        UpdateSliderHealth();
    }
    
    private void UpdateTextUserInfo()
    {
        textUserInfo.text = $"{BeginGameManager.Instance.CountId}:{BeginGameManager.Instance.UserId}={health}";
    }

    public void UpdateScoreUI(ScoreData[] scoreData)
    {
        textScore.text = "";
        foreach (var data in scoreData)
        {
            textScore.text += $"{data.UserName} : {data.Score}\n";
        }
    }
    
    private void UpdateSliderHealth()
    {
        sliderHealth.value = health;
    }

    private void CheckCountId()
    {
        BeginGameManager.Instance.UpdateCountId();
        UpdateTextUserInfo();
    }

    private IEnumerator FadeIn()
    {
        float alphaValue = 0f;
        fadeInImage.GetComponent<Image>().color = new Color(_fadeInImageColor.r, _fadeInImageColor.g, _fadeInImageColor.b ,alphaValue);
        fadeInImage.SetActive(true);

        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            alphaValue += 0.1f;
            fadeInImage.GetComponent<Image>().color = new Color(_fadeInImageColor.r, _fadeInImageColor.g, _fadeInImageColor.b ,alphaValue);

            if (alphaValue >= 1f)
            {
                break;
            }
        }
        
        LoadEndScene();
    }

    private void StartFadeIn()
    {
        StartCoroutine(nameof(FadeIn));
    }

    private void LoadEndScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
    }
}