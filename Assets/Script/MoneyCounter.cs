using UnityEngine;
using TMPro;

public class MoneyCounter : MonoBehaviour
{
    public static MoneyCounter instance;

    public TMP_Text moneyText;
    public int currentMoney = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Start()
    {
        moneyText.text = "    : " + currentMoney.ToString();
    }

    public void IncreaseMoney(int v)
    {
        currentMoney += v;
        moneyText.text = "    : " + currentMoney.ToString();

    }
}
