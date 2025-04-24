using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public static GameManager Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<GameManager>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    private GameObject _PlayeSceneObjects;
    public GameObject PlayeSceneObjects
    {
        get
        {
            if (_PlayeSceneObjects == null)
            {
                _PlayeSceneObjects = GameObject.Find("PlaySceneObjects");
            }

            return _PlayeSceneObjects;
        }
    }

    private int _Money;
    public int Money
    {
        get
        {
            return _Money;
        }
        set
        {
            _Money = value;
            UpdateMoneyCount();
        }
    }
    public int playerHearts = 3;
    public bool HasBread { get; set; }
    public bool ShopOpen { get; set; }
    public bool policeSpawning { get; set; } // Determines whether police should spawn or not

    public void UpdateMoneyCount()
    {
        List<TextMeshProUGUI> moneyTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None)
                    .Where(obj => obj.name == "MoneyVal")
                    .ToList();

        foreach (TextMeshProUGUI textBox in moneyTexts)
        {
            textBox.text = $"{Money}";
        }
    }

    private void Start()
    {
        Money = 0;
    }

    public void OpenShop()
    {
        ShopOpen = true;
        PlayeSceneObjects.SetActive(false);
        SceneHelper.LoadScene("BreadBuyingMenu", true);
    }

    public void CloseShop()
    {
        ShopOpen = false;
        SceneHelper.UnloadScene("BreadBuyingMenu");
        CallAfterDelay.Create(1.0f, () => {
            PlayeSceneObjects.SetActive(true);
        });
    }

    public UnityAction onPlayerDamage;
    public void DamagePlayer()
    {
        playerHearts--;
        onPlayerDamage.Invoke();
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}