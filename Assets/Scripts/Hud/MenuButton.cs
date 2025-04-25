using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject buttonToDisable;

    // Optional serialized audio references in case you want to assign them directly in this script
    [SerializeField] private AudioSource gameplayTheme;
    [SerializeField] private AudioSource bakeryTheme;
    [SerializeField] private AudioSource gameplayBreadTheme;

    public void Awake()
    {
        //Time.timeScale = 1f; // Normal time 
    }

    public void BuyBread()
    {
        Debug.Log("BUYING BREAD!!!!!!!!!");

        // Check if player has enough money
        if (GameManager.Instance.Money >= 100)
        {
            GameManager.Instance.Money -= 100;
            GameManager.Instance.HasBread = true;

            if (GameData.Instance != null) GameData.Instance.SpendMoney(100);

            // Set the global bread bought flag
            BoxCollider2DDetector.breadEverBought = true;

            // Disable the referenced GameObject if money was over 100
            if (buttonToDisable != null)
            {
                buttonToDisable.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Not enough money to buy bread!");
        }
    }

    public void CloseShop()
    {
        Time.timeScale = 1f; // Resume time

        // Stop the bakery theme
        if (BoxCollider2DDetector.bakeryTheme != null)
        {
            BoxCollider2DDetector.bakeryTheme.Stop();
        }
        else if (bakeryTheme != null) // Fallback to local reference
        {
            bakeryTheme.Stop();
        }

        // Play the appropriate gameplay theme based on bread status
        if (BoxCollider2DDetector.breadEverBought || GameManager.Instance.HasBread)
        {
            // Play bread gameplay theme
            if (BoxCollider2DDetector.gameplayBreadTheme != null)
            {
                BoxCollider2DDetector.gameplayBreadTheme.Play();
            }
            else if (gameplayBreadTheme != null) // Fallback to local reference
            {
                gameplayBreadTheme.Play();
            }
        }
        else
        {
            // Play normal gameplay theme
            if (BoxCollider2DDetector.gameplayTheme != null)
            {
                BoxCollider2DDetector.gameplayTheme.Play();
            }
            else if (gameplayTheme != null) // Fallback to local reference
            {
                gameplayTheme.Play();
            }
        }

        // Reset ShopOpen flag
        GameManager.Instance.ShopOpen = false;

        Debug.Log("Shop closed, time resumed");
    }
}