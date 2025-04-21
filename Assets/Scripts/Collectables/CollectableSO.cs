using UnityEngine;

[CreateAssetMenu(fileName = "Collectable", menuName = "Scriptable Objects/Collectable")]
public class CollectableSO : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int cost;
    [TextArea(1, 3), SerializeField] private string description;
    [SerializeField] private CollectableType type;

    public string Title => title;
    public Sprite Sprite => sprite;
    public int Cost => cost;
    public string Description => description;
    public CollectableType Type => type;
}

public enum CollectableType
{
    ZaarianRocketBooster,
    GrandmasCookies,
    EnergyDrink,
    GoopiterBatteryCharge,
    CringeSpeaker,
    ToddsMysteryCheck,
    LethalFaceCard,
    TrafficLightRemote,
    KreysFishingRod
}
