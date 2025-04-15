using UnityEngine;
using System.Collections;

public class BoxCollider2DDetector : MonoBehaviour
{
    [SerializeField] private GameObject nodeToEnable;
    private bool canTrigger = true;
    private float cooldownTime = 5f;
    private Coroutine cooldownCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canTrigger)
        {
            OpenShop();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCooldown();
        }
    }

    private void StartCooldown()
    {
        canTrigger = false;

        // Cancel any existing cooldown coroutine
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        cooldownCoroutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSecondsRealtime(cooldownTime);
        canTrigger = true;
        cooldownCoroutine = null;
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