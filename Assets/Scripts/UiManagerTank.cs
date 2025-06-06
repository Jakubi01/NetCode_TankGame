using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManagerTank : MonoBehaviour
{
    public TMP_Text textUserInfo;
    public TMP_Text textTimer;
    public Slider sliderHealth;

    public int health;
    public int maxHealth = 100;
    private float _playTime;
    
    private bool ShouldStartCountDown { get; set; }
    
    private const float WaitForCollectUserInfoSeconds = 1f;

    public static UiManagerTank Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        health = maxHealth;
        UpdateTextUserInfo();
        UpdateSliderHealth();
        Invoke(nameof(CheckCountId), 2.0f);
        _playTime = InGameManager.Instance.PlayTime;
        textTimer.text = InGameManager.Instance.PlayTime.ToString("F1");
    }

    private void Update()
    {
        if (ShouldStartCountDown)
        {
            UpdateCountDownTimer();
        }
    }

    public void StartTimerClient()
    {
        ShouldStartCountDown = true;
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

        StartCoroutine(nameof(WaitForCollectUserInfo));
        ShouldStartCountDown = false;
        StartFadeIn();
    }

    private IEnumerator WaitForCollectUserInfo()
    {
        Debug.Log("Collect Scores");
        InGameManager.Instance.CollectScores();

        yield return new WaitForSeconds(WaitForCollectUserInfoSeconds);
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

    private void UpdateSliderHealth()
    {
        sliderHealth.value = health;
    }

    private void CheckCountId()
    {
        BeginGameManager.Instance.UpdateCountId();
        UpdateTextUserInfo();
    }

    private void StartFadeIn()
    {
        // TODO : 페이드 인 효과 적용 

        // FadeIn Effect가 끝나면 호출
        if (true)
        {   
            LoadEndScene();
        }
    }

    private void LoadEndScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
    }
}