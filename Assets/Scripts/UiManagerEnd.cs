using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManagerEnd : MonoBehaviour
{
    private IEnumerator ReturnToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);

        yield return new WaitForSeconds(0.5f);
        
        SceneManager.LoadScene("BeginScene");
    }
    
    public void OnMainMenuBtnClicked()
    {
        StartCoroutine(nameof(ReturnToMainMenu));
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
