using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    public enum ThisItem { None, CringeSpeaker, EnergyDrink, GoopiterBattery, GrandmasCookies, Infinite, LethalFC, TrafficLight, ZaarianRocketBoosters, }
    public ThisItem TheItem;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    }

}
