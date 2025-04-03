using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject buttonToDisable;

    public void Awake()
    {
        Time.timeScale = 1f; // Normal time 
    }

    public void BuyBread()
    {
        Debug.Log("BUYING BREAD!!!!!!!!!");

        // Check if player has enough money
        if (GameManager.Instance.Money >= 100)
        {
            GameManager.Instance.Money -= 100;
            GameManager.Instance.HasBread = true;

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

    public void OpenShop()
    {
        Time.timeScale = 0f; // Freeze time (Jojo's reference?!?!)
        Debug.Log("Shop opened, time frozen");
    }

    public void CloseShop()
    {
        Time.timeScale = 1f; // Resume time
        Debug.Log("Shop closed, time resumed");
    }
}