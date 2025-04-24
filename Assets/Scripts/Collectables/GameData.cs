using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameData : MonoBehaviour
{
    public static GameData Instance {  get; private set; }

    public int Money { get; private set; }
    public int BankAccountMoney { get; private set; }
    public int PoliceKilled { get; private set; }
    public float TimeItTookToFinish {  get; private set; }
    public float StartingTime {  get; private set; }
    public int Hearts {  get; private set; }
    public int StartingHealth { get; private set; }

    private List<CollectableSO> collectables;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            collectables = new();
        }
    }

    public void AddMoney(int value)
    {
        Money += value;
    }

    public void SendMoneyToBank()
    {
        BankAccountMoney += Money;
        Money = 0;
    }

    public void AddBankMoney(int value)
    {
        BankAccountMoney += value;
    }

    public void SpendMoney(int value)
    {
        Money = Mathf.Max(Money - value, 0);
    }

    public void SpendBankMoney(int value)
    {
        BankAccountMoney = Mathf.Max(BankAccountMoney - value, 0);
    }

    public void AddCollectable(CollectableSO collectable)
    {
        collectables.Add(collectable);
    }

    public void SetTimeItTookToWin(float value)
    {
        TimeItTookToFinish = value;
    }

    public void SetStartingTime(float value)
    {
        StartingTime = value;
    }

    public void PoliceOfficerKilled()
    {
        PoliceKilled++;
    }

    public void SetHearts(int value)
    {
        Hearts = value;
    }

    public void SetStartingHealth(int value)
    {
        StartingHealth = value;
        Hearts = StartingHealth;
    }

    public void ResetRuntimeData()
    {
        TimeItTookToFinish = 0;
        PoliceKilled = 0;
    }

    public void ResetGameData()
    {
        Money = 0;
        BankAccountMoney = 0;
        collectables.Clear();
    }

    public bool HasItem(CollectableType type)
    {
        return collectables.Any(c => c.Type == type);
    }
}