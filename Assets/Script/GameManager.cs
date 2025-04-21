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

    private List<TextMeshProUGUI> _MoneyTexts = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> MoneyTexts
    {
        get
        {
            if (_MoneyTexts.Count == 0)
            {
                GameObject[] moneyObjects = GameObject.FindObjectsOfType<GameObject>()
                    .Where(obj => obj.name == "MoneyVal")
                    .ToArray();

                foreach (GameObject obj in moneyObjects)
                {
                    TextMeshProUGUI textComponent = obj.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        _MoneyTexts.Add(textComponent);
                    }
                }
            }
            return _MoneyTexts;
        }
    }

    public int Money { get; set; }
    public int playerHearts = 3;
    public bool HasBread { get; set; }
    public bool ShopOpen { get; set; }
    public bool policeSpawning { get; set; } // Determines whether police should spawn or not

    void Update()
    {
        foreach (TextMeshProUGUI moneyText in MoneyTexts)
        {
            if (moneyText != null)
            {
                moneyText.text = $"{Money}";
            }
        }
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
}