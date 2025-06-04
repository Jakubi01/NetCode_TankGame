using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManagerEnd : MonoBehaviour
{
    // TODO : 이 스크립트는 5분이 지나고 게임이 종료되면 LeaderBoard를 표기하
    
    public GameObject Panel;

    private void Start()
    {
        Panel.SetActive(true);
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
