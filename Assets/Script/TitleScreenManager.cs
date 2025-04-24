using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject creditsPanel;   // Assign the Credits Panel in the inspector
    public GameObject titleCard;      // Assign the Title Card (Logo or Title UI)

    public void NewGame()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.ResetGameData();
            GameData.Instance.ResetRuntimeData();
        }
        if (GameManager.Instance != null) GameManager.Instance.DestroyThis();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        if (GameData.Instance != null) GameData.Instance.ResetRuntimeData();
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
        Debug.Log("Credits clicked (show credits UI)");
        if (creditsPanel != null)
            creditsPanel.SetActive(true);

        if (titleCard != null)
            titleCard.SetActive(false);
    }

    public void CloseCredits()
    {
        Debug.Log("Closing Credits Panel");

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (titleCard != null)
            titleCard.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
