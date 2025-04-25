using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectableSlot : MonoBehaviour
{
    [SerializeField] private Image collectableImage;
    [SerializeField] private TextMeshProUGUI collectableTitle;

    public void Initiate(CollectableSO collectable)
    {
        collectableImage.sprite = collectable.Sprite;
        collectableTitle.text = collectable.Title;
    }
}
