using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


using UnityEngine.EventSystems;

public class BuyButton : MonoBehaviour/*, IPointerEnterHandler, IPointerExitHandler*/
{
    //public enum WhatDoesItDo { None, Buy, Leave};
    //public DeathShop DS;
    //public int ID;
    //public bool inZone;
    //public WhatDoesItDo ButtonCommand;
    //private Button thisButton;
    //private PowerUp PU;
    //public string sceneToLoad;
    //public bool ToLeave;


    //// Start is called before the first frame update
    //void Start()
    //{
    //    PU = FindObjectOfType<PowerUp>();
    //    thisButton = GetComponent<Button>();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //   inZone = false;
    // //   thisButton.onClick.AddListener(ButtonVoid);
    //}
    
    //public void ButtonVoid()
    //{
    //    switch(ButtonCommand)
    //    {
    //        case WhatDoesItDo.Buy:
    //            try
    //            {
    //                if (PU.money >= DS.ItemsOut[ID].cost)
    //                {
    //                    if (!PU.Items.Contains(DS.ItemsOut[ID].ToString()))
    //                    {
    //                        PU.money -= DS.ItemsOut[ID].cost;
    //                        PU.UpInQuestion(DS.ItemsOut[ID].title);
    //                        DS.ItemsOut[ID] = null;
    //                        DS.ItemsCost[ID].text = null;
    //                        DS.Description.text = null;
    //                        Destroy(DS.addedObject[ID]);
    //                        Debug.Log("Added");
    //                    }
    //                    else
    //                    {
    //                        Debug.Log("Already have");
    //                    }
    //                }
    //                else
    //                {
    //                    Debug.Log("Broke");
    //                }
    //            } catch { Debug.Log("Nothing here"); }
    //            break;

    //        case WhatDoesItDo.Leave:
    //            SceneManager.LoadScene(sceneToLoad);
    //            break;
    //    }
    //}


    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (!ToLeave)
    //    {
    //        try
    //        {
    //            DS.Description.text = DS.ItemsOut[ID].description.ToString();
    //        }
    //        catch { }
            
    //    } else
    //    {
    //        DS.Description.text = "Return to the game";
    //    }
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    DS.Description.text = null;
    //}
}
