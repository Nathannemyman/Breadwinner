using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void NewGame()
    {
        // Replace "GameScene" with your actual game scene name
        if (GameData.Instance != null) GameData.Instance.ResetGameData();
        if (GameManager.Instance != null) GameManager.Instance.DestroyThis();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        if (GameManager.Instance != null) GameManager.Instance.DestroyThis();
        SceneManager.LoadScene(1);
    }

    public void OpenSettings()
    {
        Debug.Log("Settings clicked (show settings UI here)");
        // Show your settings panel
    }

    public void OpenCredits()
    {
        Debug.Log("Credits clicked (show credits UI here)");
        // Show your credits panel
    }
}
