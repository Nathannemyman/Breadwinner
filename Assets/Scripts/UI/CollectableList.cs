using UnityEngine;
using UnityEngine.Events;

public class CollectableList : MonoBehaviour
{
    [SerializeField] private UnityEvent onHasAllCollectables;
    [SerializeField] private UnityEvent onDoesntHasAllCollectables;

    private CollectableSlot[] slots;

    private void OnEnable()
    {
        slots = GetComponentsInChildren<CollectableSlot>(includeInactive: true);

        if (GameData.Instance != null)
        {
            for (int i = 0; i < GameData.Instance.Collectables.Count; i++)
            {
                if (i < slots.Length)
                {
                    slots[i].Initiate(GameData.Instance.Collectables[i]);
                }
            }

            if (GameData.Instance.Collectables.Count < 9) onDoesntHasAllCollectables?.Invoke();
            else onHasAllCollectables?.Invoke();
        }
    }
}
