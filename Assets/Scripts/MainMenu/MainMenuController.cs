using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gmaeSceneName = "InGameScene";

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
