using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void NewGame()
    {
        // Replace "GameScene" with your actual game scene name
        SceneManager.LoadScene("JohnMilestoneTestScene");
    }

    public void ContinueGame()
    {
        Debug.Log("Continue Game clicked (add load system here)");
        // Load saved game data
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
