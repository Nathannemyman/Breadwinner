using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ConditionalButton : MonoBehaviour
{
    private bool condition;
    private Button button;

    private void Awake()
    {
        TryGetComponent(out button);
    }

    public void TurnOnInteractable()
    {
        if (condition) button.interactable = true;
    }
}
