using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerUp : MonoBehaviour
{
    private static PowerUp _Instance;
    public static PowerUp Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<PowerUp>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    public List<string> Items = new List<string>();
    public string theItem;
    public string sceneToLoad;
    public float money;
    public static bool hasCringeSpeaker = false;
    //   public PogoStickMovement Pogo;

    private void Start()
    {
        //  Pogo = FindObjectOfType<PogoStickMovement>();
        try
        {
            for (int i = 0; i < Items.Count; i++)
            {
                switch (Items[i])
                {
                    case "Cookies":

                        break;

                    case "Infinite":

                        break;

                    case "Cringe Speaker":

                        hasCringeSpeaker = true;
                        Debug.Log("CRINGE SPEAKER BOUGHT!: " + hasCringeSpeaker + " !!!!!");
                        break;

                    case "Energy Drink":

                        break;

                    case "Goopiter Battery":

                        break;

                    case "Lethal Face Card":

                        break;

                    case "Traffic Light":

                        break;

                    case "Rocket Boosters":

                        break;
                }
            }
        }
        catch { }
    }

    private void Update()
    {
        if (money < 0)
        {
            money = 0;
        }
    }

    public void UpInQuestion(string power)
    {
        Items.Add(power);
    }
}