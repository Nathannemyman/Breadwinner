using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public GameObject creditsPanel; // Assign CreditsPanel in Inspector

    void Start()
    {
        creditsPanel.SetActive(false); // Hide credits at start
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }
}
