using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathShopButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI costTextBox;

    public CollectableSO Data { get; private set; }
    public Button BuyButton {  get; private set; }

    private bool bought = false;

    private void Awake()
    {
        BuyButton = GetComponentInChildren<Button>();
    }

    public void Initiate(CollectableSO dataSO)
    {
        costTextBox.text = dataSO.Cost.ToString();
        buttonImage.sprite = dataSO.Sprite;

        Data = dataSO;
    }

    public void Erase()
    {
        costTextBox.text = string.Empty;

        Color tempColor = buttonImage.color;
        tempColor.a = 0;
        buttonImage.color = tempColor;
    }

    public void SetDescriptionText(TextMeshProUGUI descriptionText)
    {
        if (!bought) descriptionText.text = Data.Description;
        else descriptionText.text = string.Empty;
        
    }

    public void Buy()
    {
        if (GameData.Instance == null) return;
        if (GameData.Instance.BankAccountMoney < Data.Cost) return;
        if (bought) return;

        if (GameData.Instance.HasItem(Data.Type)) return;
        else if (Data.Type == CollectableType.ToddsMysteryCheck)
        {
            int choice = Random.Range(0, 2);
            if (choice == 0) GameData.Instance.AddBankMoney(GameData.Instance.BankAccountMoney);
            else GameData.Instance.SpendBankMoney(GameData.Instance.BankAccountMoney/2);
        }
        else
        {
            GameData.Instance.AddCollectable(Data);
            GameData.Instance.SpendBankMoney(Data.Cost);
            Erase();
            bought = true;
        }
    }
}
