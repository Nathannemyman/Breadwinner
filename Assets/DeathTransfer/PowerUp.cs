using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PowerUp : MonoBehaviour
{
    public List<string> Items = new List<string>();
    public string theItem;
    private static bool Thisexists;
    public string sceneToLoad;
    public float money;
 //   public PogoStickMovement Pogo;

    // Start is called before the first frame update
    void Start()
    {
      //  Pogo = FindObjectOfType<PogoStickMovement>();

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
