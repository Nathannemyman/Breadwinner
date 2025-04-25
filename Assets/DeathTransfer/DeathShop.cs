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
                if(GameData.Instance.Collectables.Contains(collectables[i])) {
                    Debug.Log("Has This " + collectables[i].ToString());
                }
    
            }

        }

        List<CollectableSO> availableCollectables = collectables.Where(x => !GameData.Instance.HasItem(x.Type) || x.Type == CollectableType.ToddsMysteryCheck).ToList();
        if (availableCollectables.Count <= 0) return;

        for (int i = 0; i < buttons.Length; i++)
        {
            CollectableSO chosenCollectible = availableCollectables[Random.Range(0, availableCollectables.Count)];
            buttons[i].Initiate(chosenCollectible);
            availableCollectables.Remove(chosenCollectible);
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

    public void Reset()
    {
        if (GameData.Instance != null) GameData.Instance.ResetRuntimeData();
        if (GameManager.Instance != null) GameManager.Instance.DestroyThis();
    }
}
