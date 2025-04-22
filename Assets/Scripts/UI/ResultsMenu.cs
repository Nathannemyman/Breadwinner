using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ResultsMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    [Header("Stat Values")]
    [SerializeField] private TextMeshProUGUI timeScoreBox;
    [SerializeField] private TextMeshProUGUI moneyScoreBox;
    [SerializeField] private TextMeshProUGUI policeScoreBox;
    [SerializeField] private TextMeshProUGUI totalScoreBox;

    [Header("Ranking Stuff")]
    [SerializeField] private Image rankingImage;
    [SerializeField] private Sprite sRankImage;
    [SerializeField] private Sprite aRankImage;
    [SerializeField] private Sprite bRankImage;
    [SerializeField] private Sprite cRankImage;
    [SerializeField] private Sprite dRankImage;
    [SerializeField] private Sprite fRankImage;

    private bool menuOpen = false;
    private Coroutine resultsShowCoroutine;
    private Animator anim;

    private int cashAtEndOfRun;
    private float timeAtEndOfRun;
    private int policeKillsAtEndOfRun;

    private void Awake()
    {
        TryGetComponent(out anim);

        timeAtEndOfRun = GameData.Instance.Time;
        cashAtEndOfRun = GameData.Instance.Money;
        policeKillsAtEndOfRun = GameData.Instance.PoliceKilled;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !menuOpen)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        menuOpen = true;
        menu.SetActive(true);
        anim.SetTrigger("ToggleMenu");
        ShowResults();
    }

    private void ShowResults()
    {
        if (resultsShowCoroutine != null) StopCoroutine(resultsShowCoroutine);
        resultsShowCoroutine = StartCoroutine(ShowTheResults());
    }

    private IEnumerator ShowTheResults()
    {
        float timeToDisplayLine = 1;
        float timer = 0;

        if (timeScoreBox != null)
        {
            while (timer < timeToDisplayLine)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            int minutes = Mathf.FloorToInt(timeAtEndOfRun / 60f);
            int seconds = Mathf.FloorToInt(timeAtEndOfRun % 60f);
            timeScoreBox.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        timer = 0;

        if (moneyScoreBox != null)
        {
            while (timer < timeToDisplayLine)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            moneyScoreBox.text = Mathf.FloorToInt(cashAtEndOfRun).ToString();
        }

        timer = 0;

        if (policeScoreBox != null)
        {
            while (timer < timeToDisplayLine)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            policeScoreBox.text = $"(#{policeKillsAtEndOfRun}) x50 = {0 - policeKillsAtEndOfRun * 50}";
        }

        timer = 0;
        int totalScore = Mathf.FloorToInt((timeAtEndOfRun % 60f) + cashAtEndOfRun - (policeKillsAtEndOfRun * 50));

        if (totalScoreBox != null)
        {
            while (timer < timeToDisplayLine)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            totalScoreBox.text = totalScore.ToString();
        }

        if (totalScore < 0) rankingImage.sprite = fRankImage;
        else if (totalScore < 50) rankingImage.sprite = dRankImage;
        else if (totalScore < 150) rankingImage.sprite = cRankImage;
        else if (totalScore < 350) rankingImage.sprite = bRankImage;
        else if (totalScore < 500) rankingImage.sprite = aRankImage;
        else if (totalScore < 750) rankingImage.sprite = sRankImage;
        else rankingImage.sprite = sRankImage;

        rankingImage.gameObject.SetActive(true);

        yield break;
    }
}
