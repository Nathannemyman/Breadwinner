using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 300f; // 5 minutes (300 seconds)
    public TextMeshProUGUI timerText; // Reference to UI text
    public Color[] timerColors = { Color.white, Color.yellow, new Color(1f, 0.5f, 0f), Color.red, new Color(0.5f, 0f, 0f) }; // White -> Yellow -> Orange -> Red -> Dark Red
    private float totalTime = 300f; // Store total time for color transitions

    void Start()
    {
        if (timerText != null)
        {
            timerText.font = Resources.Load<TMP_FontAsset>("Jersey 15"); // Load Jersey 15 font
            timerText.color = Color.white; // Start with white color
            timerText.alignment = TextAlignmentOptions.Center; // Center text
        }
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0;
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

            // Update text color based on time left
            float percentage = timeRemaining / totalTime;
            if (percentage > 0.75f)
                timerText.color = timerColors[0]; // White
            else if (percentage > 0.5f)
                timerText.color = timerColors[1]; // Yellow
            else if (percentage > 0.25f)
                timerText.color = timerColors[2]; // Orange
            else if (percentage > 0.1f)
                timerText.color = timerColors[3]; // Red
            else
                timerText.color = timerColors[4]; // Dark Red
        }
    }
}
