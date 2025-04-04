using UnityEngine;

public class BoxCollider2DDetector : MonoBehaviour
{
    [SerializeField] private GameObject nodeToEnable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OpenShop();
        }
    }

    private void OpenShop()
    {
        // Freeze time by setting timeScale to 0
        Time.timeScale = 0f;

        // Enable the node
        if (nodeToEnable != null)
        {
            nodeToEnable.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Node to enable is not assigned in the inspector!");
        }
    }
}