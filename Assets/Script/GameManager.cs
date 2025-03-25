using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _Instance;
    public static GameManager Instance { 
        get { 
            if (!_Instance) {
                _Instance = new GameObject().AddComponent<GameManager>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        } 
    }

    private GameObject _PlayeSceneObjects;
    public GameObject PlayeSceneObjects {
        get {
            if (_PlayeSceneObjects == null) {
                _PlayeSceneObjects = GameObject.Find("PlaySceneObjects");
            }

            return _PlayeSceneObjects;
        }
    }

    private TextMeshProUGUI _MoneyText;
    public TextMeshProUGUI MoneyText {
        get {
            if (_MoneyText== null) {
                _MoneyText= GameObject.Find("MoneyVal").GetComponent<TextMeshProUGUI>();
            }

            return _MoneyText;
        }
    }

    public int Money { get; set; }
    public bool HasBread {get; set ;}
    public bool ShopOpen {get; set; }
    public bool policeSpawning { get; set; } // Determines whether police should spawn or not

    void Update() {
        MoneyText.text = $"{Money}";
    }

    public void OpenShop() {
        ShopOpen = true;
        PlayeSceneObjects.SetActive(false);
        SceneHelper.LoadScene("BreadBuyingMenu", true);
    }

    public void CloseShop() {
        ShopOpen = false;
        SceneHelper.UnloadScene("BreadBuyingMenu");
        CallAfterDelay.Create(1.0f, () => {
            PlayeSceneObjects.SetActive(true);
        });
    }

}

