using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class BuyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public DeathShop DS;
    public int ID;
    public bool inZone;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     //   inZone = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            DS.Description.text = DS.ItemsOut[ID].description.ToString();
        } catch { }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DS.Description.text = null;
    }
}
