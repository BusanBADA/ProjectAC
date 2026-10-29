using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gmaeSceneName = "GameScene";

   public void StartGame() 
    {
        SceneManager.LoadScene(gmaeSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Exit");
    }
}
