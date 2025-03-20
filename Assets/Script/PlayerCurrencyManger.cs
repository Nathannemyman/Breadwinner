using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCurrencyManager : MonoBehaviour
{
    public int playerCurrency = 100; // Starting currency
    public int itemCost = 20; // Cost of the item
    public TMP_Text currencyText; // TextMeshPro UI element
    public GameObject buyButton; // Reference to the Buy Button
    public GameObject denyButton; // Reference to the Deny Button
    public GameObject heartUI; //  Reference to Heart UI
    public GameObject shopUI; //  Reference to Shop UI

    void Start()
    {
        UpdateCurrencyUI();
    }
    public void OpenShop()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(true);  // Show the shop UI
        }

        if (heartUI != null)
        {
            heartUI.SetActive(false);  // Hide the heart UI
        }
    }

    public void BuyItem()
    {
        if (playerCurrency >= itemCost)
        {
            playerCurrency -= itemCost;
            UpdateCurrencyUI();
            Debug.Log("Purchase successful! Remaining Currency: " + playerCurrency);

            if (heartUI != null)
            {
                heartUI.SetActive(true);
            }
            if (shopUI != null)
            {
                shopUI.SetActive(false);
            }
        }
        else
        {
            ShowDenyButton();
        }
    }

    void UpdateCurrencyUI()
    {
        if (currencyText != null)
        {
            currencyText.text = "Currency: " + playerCurrency;
        }

        // Show/hide buttons based on currency
        if (playerCurrency >= itemCost)
        {
            ShowBuyButton();
            HideDenyButton();
        }
        else
        {
            ShowDenyButton();
            HideBuyButton();
        }
    }

    public void ShowDenyButton()
    {
        if (denyButton != null)
        {
            denyButton.SetActive(true);
        }
        Debug.Log("Not enough currency!");
    }

    public void HideDenyButton()
    {
        if (denyButton != null)
        {
            denyButton.SetActive(false);
        }
    }

    public void ShowBuyButton()
    {
        if (buyButton != null)
        {
            buyButton.SetActive(true);
        }
    }

    public void HideBuyButton()
    {
        if (buyButton != null)
        {
            buyButton.SetActive(false);
        }
    }

    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        UpdateCurrencyUI();
    }
}
