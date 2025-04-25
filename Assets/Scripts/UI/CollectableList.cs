using UnityEngine;

public class CollectableList : MonoBehaviour
{
    private CollectableSlot[] slots;

    private void OnEnable()
    {
        slots = GetComponentsInChildren<CollectableSlot>();

        if (GameData.Instance != null)
        {
            for (int i = 0; i < GameData.Instance.Collectables.Count; i++)
            {
                if (i < slots.Length)
                {
                    slots[i].Initiate(GameData.Instance.Collectables[i]);
                }
            }
        }
    }
}
