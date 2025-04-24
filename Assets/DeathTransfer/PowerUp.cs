using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PowerUp : MonoBehaviour
{

    private PogoStickMovement PogoStick;
    public List<string> Items = new List<string>();
    public string theItem;
    private static bool Thisexists;
    public string sceneToLoad;
    public float money;
    public bool hasCringeSpeaker = false;
    //   public PogoStickMovement Pogo;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

        // Start is called before the first frame update
        void Start()
    {
        PogoStick = FindObjectOfType<PogoStickMovement>();

        if (!Thisexists)
        {
            Thisexists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
           // 	DestroyObject(gameObject);
        }

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
                        break;

                    case "Energy Drink":
                        Debug.Log("Energy Drink have");
                        
                        break;

                    case "Goopiter Battery":
                        Debug.Log("Battery have");
                        
                        break;

                    case "Lethal Face Card":

                        break;

                    case "Traffic Light":

                        break;

                    case "Rocket Boosters":
                        Debug.Log("Rockets have");
                        
                        break;
                }
            }
        } catch {  }
    }

    // Update is called once per frame
    void Update()
    {

        if(money < 0)
        {
            money = 0;
        }
    }

    public void UpInQuestion(string power)
    {
        Items.Add(power);
    }
}
