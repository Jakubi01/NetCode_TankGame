using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiManagerTank : MonoBehaviour
{
    public TMP_Text textUserInfo;
    public Slider sliderHealth;

    public int health;
    public int maxHealth = 100;

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
    
    void Start()
    {
        BeginSceneGameManager.Instance.StartNode();
        BeginSceneGameManager.Instance.UpdateCountId();
        health = maxHealth;
        UpdateTextUserInfo();
        UpdateSliderHealth();
        Invoke(nameof(CheckCountId), 2.0f);
    }

    public void UpdateUIInfo(int value)
    {
        health = value;

        UpdateTextUserInfo();
        UpdateSliderHealth();
    }
    
    private void UpdateTextUserInfo()
    {
        textUserInfo.text = $"{BeginSceneGameManager.Instance.CountId}:{BeginSceneGameManager.Instance.UserId}={health}";
    }

    private void UpdateSliderHealth()
    {
        sliderHealth.value = health;
    }

    private void CheckCountId()
    {
        BeginSceneGameManager.Instance.UpdateCountId();
        UpdateTextUserInfo();
    }
}