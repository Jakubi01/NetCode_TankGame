using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManagerEnd : MonoBehaviour
{
    public GameObject panel;

    private void Awake()
    {
        NetworkManager.Singleton.NetworkConfig.AutoSpawnPlayerPrefabClientSide = false;
    }

    private void Start()
    {
        panel.SetActive(true);
    }

    public void OnRetryBtnClicked()
    {
        SceneManager.LoadScene("TankScene");
    }

    public void OnMainMenuBtnClicked()
    {
        SceneManager.LoadScene("BeginScene");
    }

    public void OnExitBtnClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
