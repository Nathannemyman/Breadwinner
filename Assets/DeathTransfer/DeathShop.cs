using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.UI.VirtualMouseInput;

public class DeathShop : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private Texture2D cursorTexture;

    [Header("Menu Objects")]
    [SerializeField] private TextMeshProUGUI moneyTextBox;
    [SerializeField] private DeathShopButton[] buttons;

    [Header("Collectables")]
    [SerializeField] private CollectableSO[] collectables;


    void Start()
    {
        List<CollectableSO> collectList = collectables.ToList();

        buttons[0].BuyButton.onClick.AddListener(() => buttons[0].Buy());
        buttons[1].BuyButton.onClick.AddListener(() => buttons[1].Buy());
        buttons[2].BuyButton.onClick.AddListener(() => buttons[2].Buy());

        if (GameData.Instance != null) {

            for (int i = 0; i < buttons.Length; i++)
            {
                if(GameData.Instance.collectables.Contains(collectables[i])) {
                    Debug.Log("Has This " + collectables[i].ToString());
                }
    
            }

        }

        for (int i = 0; i < buttons.Length; i++)
        {
            CollectableSO chosenCollectible = collectList[Random.Range(0, collectList.Count)];
            buttons[i].Initiate(chosenCollectible);
            collectList.Remove(chosenCollectible);
            buttons[i].BuyButton.onClick.AddListener(() => SetMoneyText());
        }

        if (GameData.Instance != null) GameData.Instance.SendMoneyToBank();
        SetMoneyText();

        //Cursor.SetCursor(cursorTexture, Vector2.zero, UnityEngine.CursorMode.Auto);
    }

    

    private void SetMoneyText()
    {
        if (GameData.Instance != null)
        {
            moneyTextBox.text = GameData.Instance.BankAccountMoney.ToString();
        }
    }

    public void Continue()
    {
        Cursor.SetCursor(null, Vector2.zero, UnityEngine.CursorMode.Auto);
    }
}
