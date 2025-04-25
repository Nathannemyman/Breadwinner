using UnityEngine;
using System.Collections;

public class BoxCollider2DDetector : MonoBehaviour
{
    [SerializeField] private GameObject nodeToEnable;

    // Audio sources - made public static so they can be accessed from MainMenu
    public static AudioSource gameplayTheme;        // Default gameplay music
    public static AudioSource bakeryTheme;          // Plays only in the shop
    public static AudioSource gameplayBreadTheme;   // Replaces default after bread purchase

    // Reference to the same audio sources for inspector assignment
    [SerializeField] private AudioSource _gameplayTheme;
    [SerializeField] private AudioSource _bakeryTheme;
    [SerializeField] private AudioSource _gameplayBreadTheme;

    // Static flag to track if bread was ever bought
    public static bool breadEverBought = false;

    private bool canTrigger = true;
    private float cooldownTime = 5f;
    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        // Initialize static references
        gameplayTheme = _gameplayTheme;
        bakeryTheme = _bakeryTheme;
        gameplayBreadTheme = _gameplayBreadTheme;

        // Check if we should already be in the bread-purchased state
        if (GameManager.Instance.HasBread)
        {
            breadEverBought = true;
        }
    }

    private void Start()
    {
        // Play the appropriate theme on start
        if (breadEverBought)
        {
            if (gameplayBreadTheme != null && !gameplayBreadTheme.isPlaying)
            {
                if (gameplayTheme != null && gameplayTheme.isPlaying)
                    gameplayTheme.Stop();

                gameplayBreadTheme.Play();
            }
        }
        else
        {
            if (gameplayTheme != null && !gameplayTheme.isPlaying)
            {
                gameplayTheme.Play();
            }
        }
    }

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

        // Audio changes when entering the shop
        if (breadEverBought)
        {
            // If bread was ever bought, stop the bread gameplay theme
            if (gameplayBreadTheme != null && gameplayBreadTheme.isPlaying)
            {
                gameplayBreadTheme.Stop();
            }
        }
        else
        {
            // Otherwise stop the normal gameplay theme
            if (gameplayTheme != null && gameplayTheme.isPlaying)
            {
                gameplayTheme.Stop();
            }
        }

        // Play bakery theme
        if (bakeryTheme != null && !bakeryTheme.isPlaying)
        {
            bakeryTheme.Play();
        }

        // Set ShopOpen flag
        GameManager.Instance.ShopOpen = true;

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