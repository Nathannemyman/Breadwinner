using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathShop : MonoBehaviour
{
    public GameObject Cursor;
    public Button[] spots;
    public Transform[] ItemsSpots;
    public List<BuyableItems> Items = new List<BuyableItems>();
    public List<BuyableItems> ItemsOut = new List<BuyableItems>();
    private int pick;
    private bool pressing;
    public Vector3 Offset;
    public float bankAccount;
    public Text Money;
    public Text[] ItemsCost;
    public Text Description;
    public Sprite Regular;
    public Sprite Hover;

    private Vector3 mouse;
    public float moveSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
            for (int i = 0; i < Items.Count + 1; i++)
            {
                int rnd = Random.Range(0, Items.Count);
                BuyableItems thisItem = Instantiate(Items[rnd], ItemsSpots[i].transform.position, transform.rotation);
                ItemsOut.Add(thisItem);
                Items.RemoveAt(rnd);

                ItemsCost[i].text = ItemsOut[i].cost.ToString();
            }
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.transform.position = Vector2.Lerp(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), moveSpeed);
        UnityEngine.Cursor.visible = false;

        Money.text = bankAccount.ToString();

        /*
        if (spots[pick].GetComponent<Image>().sprite != Hover)
        {
            spots[pick].GetComponent<SpriteRenderer>().sprite = Hover;
        }
        else
        {
            for (int i = 0; i < spots.Length; i++)
            {
                if (pick != i)
                {
                    spots[i].GetComponent<SpriteRenderer>().sprite = Regular;
                }
            }
        }
        */

    //    Description.text = ItemsOut[pick].description.ToString();
        
        if (pick > ItemsOut.Count - 1)
        {
            pick = 0;
        } else if(pick < 0)
        {
            pick = ItemsOut.Count - 1;
        }

       // Cursor.transform.position = spots[pick].position + Offset;


        if (Input.GetAxisRaw("Horizontal") > 0.4f || Input.GetAxisRaw("Horizontal") < -0.4f)
        {
            if (!pressing)
            {
                pick += (int)Input.GetAxisRaw("Horizontal");
            }
        }

        if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f || Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -0.5f)
        {
            pressing = true;
        }
        else
        {
            pressing = false;
        }
    }
}
