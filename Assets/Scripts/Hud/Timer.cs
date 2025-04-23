using TMPro;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {
    // need to be in seconds so if 5 min that mean 300 seconds
    [SerializeField] private float timerDuration;
    [SerializeField] TextMeshProUGUI textObject;
    [SerializeField] TextMeshProUGUI milisecObject;


    private float timeRemaining;
    public float TimeRemaining { get => timeRemaining; private set => timeRemaining = value; }

    // this will fire if the timer is at 0 or lower. have whatever you need to know this subscribe to this. MAKE SURE TOE UNSUBSCRIBE
    public event Action OnTimerComplete;
    private bool eventEvoked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (GameData.Instance != null) {
            if (GameData.Instance.HasItem(CollectableType.TrafficLightRemote)) {
                timerDuration += 180;
            }

            GameData.Instance.SetStartingTime(timerDuration);
        }
        
        timeRemaining = timerDuration;


    }

    // Update is called once per frame
    void Update() {
        if (timeRemaining > 0) {
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(timeRemaining, 0);

            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            int milisecond = Mathf.FloorToInt((timeRemaining - Mathf.Floor(timeRemaining)) * 1000);

            textObject.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            milisecObject.text = string.Format(":<size=25>{0:00}</size>", milisecond);
            // milisecObject.text = string.Format(":{0:000}", milisecond);

            if (timeRemaining < 120) {
                ChangeTextColor(Color.red);
            } else if (timeRemaining < 180) {
                ChangeTextColor(Color.yellow);
            } else {
                ChangeTextColor(Color.white);
            } 
        } else {
            textObject.text = "00:00:<size=20>000</size>";
            if (!eventEvoked) {
                OnTimerComplete?.Invoke();
                eventEvoked = true;
                SceneManager.LoadScene(4); // Loads loss screen
            }
        }
    }

    public void ChangeRemaingingTime(float amount) {
        timeRemaining += amount;
    }

    public void ChangeTextColor(Color color) {
        textObject.color = color;
    }

    public void SetTimeOnGameWin() {
            if (GameData.Instance != null) GameData.Instance.SetTimeItTookToWin(timerDuration - timeRemaining);
    }
}
