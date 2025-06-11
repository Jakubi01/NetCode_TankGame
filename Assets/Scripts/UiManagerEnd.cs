using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManagerEnd : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlaySound(SoundType.ThemeEnd, true);
    }
    
    public void OnMainMenuBtnClicked()
    {
        SoundManager.Instance.StopPlaySound(SoundType.ThemeEnd);
        StartCoroutine(nameof(ReturnToMainMenu));
    }

    public void OnExitBtnClicked()
    {
        SoundManager.Instance.StopPlaySound(SoundType.ThemeEnd);
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private IEnumerator ReturnToMainMenu()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        yield return new WaitUntil(() => NetworkManager.Singleton.IsListening == false);
        
        SceneManager.LoadScene("BeginScene");
    }
}
