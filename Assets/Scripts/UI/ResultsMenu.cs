using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class ResultsMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    [Header("Time Stats")]
    [SerializeField] private float timeToStart = 2;
    [SerializeField] private float timeToDisplayLine = 1;
    [SerializeField] private float timeToDisplayScore = 2;

    [Header("Audio")]
    [SerializeField] private AudioClip sectionAppearSFX;

    [Header("Scroll Values")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform scrollContentRect;
    [SerializeField] private float phoneViewportHeight = 686;

    [Header("Header Stuff")]
    [SerializeField] private RectTransform headerRect;

    [Header("Timer Values")]
    [SerializeField] private RectTransform timeRect;
    [SerializeField] private TextMeshProUGUI timeHeaderTextBox;
    [SerializeField] private TextMeshProUGUI timeTakenTextBox;
    [SerializeField] private TextMeshProUGUI timePointsTextBox;

    [Header("Money Values")]
    [SerializeField] private RectTransform moneyRect;
    [SerializeField] private TextMeshProUGUI moneyHeaderTextBox;
    [SerializeField] private TextMeshProUGUI moneyPointsTextBox;

    [Header("Hearts Values")]
    [SerializeField] private RectTransform heartsRect;
    [SerializeField] private TextMeshProUGUI heartsHeaderTextBox;
    [SerializeField] private TextMeshProUGUI heartsPointsTextBox;

    [Header("Police Values")]
    [SerializeField] private RectTransform policeRect;
    [SerializeField] private TextMeshProUGUI policeHeaderTextBox;
    [SerializeField] private TextMeshProUGUI policePointsTextBox;

    [Header("Total Score Values")]
    [SerializeField] private RectTransform totalScoreRect;
    [SerializeField] private TextMeshProUGUI totalPointsTextBox;

    [Header("Bank Account Values")]
    [SerializeField] private RectTransform bankAllowanceRect;
    [SerializeField] private TextMeshProUGUI bankAllowanceTextBox;

    [Header("Ranking Stuff")]
    [SerializeField] private RectTransform rankingRect;
    [SerializeField] private Image rankingImage;
    [SerializeField] private Sprite sRankImage;
    [SerializeField] private Sprite aRankImage;
    [SerializeField] private Sprite bRankImage;
    [SerializeField] private Sprite cRankImage;
    [SerializeField] private Sprite dRankImage;
    [SerializeField] private Sprite fRankImage;

    [Header("Continue Stuff")]
    [SerializeField] private RectTransform continueRect;
    [SerializeField] private Button continueButton;

    private bool menuOpen = false;
    private bool canSkip = false;
    private bool resultsEnded = false;
    private Coroutine resultsShowCoroutine;
    private Animator anim;
    private AudioSource audioSource;

    private int cashAtEndOfRun;
    private float timeItTookToFinish;
    private float remainingTime;
    private int heartsAtEndOfRun;
    private int startingHealth;
    private int policeKillsAtEndOfRun;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out audioSource);

        timeItTookToFinish = GameData.Instance.TimeItTookToFinish;
        if (timeItTookToFinish != 0) remainingTime = GameData.Instance.StartingTime - timeItTookToFinish;
        else remainingTime = 0;
        cashAtEndOfRun = GameData.Instance.Money;
        heartsAtEndOfRun = GameData.Instance.Hearts;
        startingHealth = GameData.Instance.StartingHealth;
        policeKillsAtEndOfRun = GameData.Instance.PoliceKilled;

        if (GameManager.Instance != null) GameManager.Instance.DestroyThis();

        ToggleElements(false);
    }

    private void ToggleElements(bool toggleValue)
    {
        if (toggleValue)
        {
            headerRect.GetComponent<CanvasGroup>().alpha = 1;
            timeRect.GetComponent<CanvasGroup>().alpha = 1;
            timeHeaderTextBox.GetComponent<CanvasGroup>().alpha = 1;
            timeTakenTextBox.GetComponent<CanvasGroup>().alpha = 1;
            timePointsTextBox.GetComponent<CanvasGroup>().alpha = 1;
            moneyRect.GetComponent<CanvasGroup>().alpha = 1;
            moneyHeaderTextBox.GetComponent<CanvasGroup>().alpha = 1;
            moneyPointsTextBox.GetComponent<CanvasGroup>().alpha = 1;
            heartsRect.GetComponent<CanvasGroup>().alpha = 1;
            heartsHeaderTextBox.GetComponent<CanvasGroup>().alpha = 1;
            heartsPointsTextBox.GetComponent<CanvasGroup>().alpha = 1;
            policeRect.GetComponent<CanvasGroup>().alpha = 1;
            policeHeaderTextBox.GetComponent<CanvasGroup>().alpha = 1;
            policePointsTextBox.GetComponent<CanvasGroup>().alpha = 1;
            totalScoreRect.GetComponent<CanvasGroup>().alpha = 1;
            totalPointsTextBox.GetComponent<CanvasGroup>().alpha = 1;
            bankAllowanceRect.GetComponent<CanvasGroup>().alpha = 1;
            bankAllowanceTextBox.GetComponent<CanvasGroup>().alpha = 1;
            rankingRect.GetComponent<CanvasGroup>().alpha = 1;
            rankingImage.gameObject.SetActive(true);
            continueRect.GetComponent<CanvasGroup>().alpha = 1;
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            headerRect.GetComponent<CanvasGroup>().alpha = 0;
            timeRect.GetComponent<CanvasGroup>().alpha = 0;
            timeHeaderTextBox.GetComponent<CanvasGroup>().alpha = 0;
            timeTakenTextBox.GetComponent<CanvasGroup>().alpha = 0;
            timePointsTextBox.GetComponent<CanvasGroup>().alpha = 0;
            moneyRect.GetComponent<CanvasGroup>().alpha = 0;
            moneyHeaderTextBox.GetComponent<CanvasGroup>().alpha = 0;
            moneyPointsTextBox.GetComponent<CanvasGroup>().alpha = 0;
            heartsRect.GetComponent<CanvasGroup>().alpha = 0;
            heartsHeaderTextBox.GetComponent<CanvasGroup>().alpha = 0;
            heartsPointsTextBox.GetComponent<CanvasGroup>().alpha = 0;
            policeRect.GetComponent<CanvasGroup>().alpha = 0;
            policeHeaderTextBox.GetComponent<CanvasGroup>().alpha = 0;
            policePointsTextBox.GetComponent<CanvasGroup>().alpha = 0;
            totalScoreRect.GetComponent<CanvasGroup>().alpha = 0;
            totalPointsTextBox.GetComponent<CanvasGroup>().alpha = 0;
            bankAllowanceRect.GetComponent<CanvasGroup>().alpha = 0;
            bankAllowanceTextBox.GetComponent<CanvasGroup>().alpha = 0;
            rankingRect.GetComponent<CanvasGroup>().alpha = 0;
            rankingImage.gameObject.SetActive(false);
            continueRect.GetComponent<CanvasGroup>().alpha = 0;
            continueButton.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !menuOpen)
        {
            PauseGame();
        }

        if (menuOpen && Input.GetMouseButton(0) && canSkip && !resultsEnded)
        {
            SkipResults();
        }
    }

    public void PauseGame()
    {
        menuOpen = true;
        menu.SetActive(true);
        anim.SetTrigger("ToggleMenu");
        ShowResults();
    }

    private void SkipResults()
    {
        float finalContentSize =
            headerRect.sizeDelta.y +
            scrollContentRect.sizeDelta.y +
            moneyRect.sizeDelta.y +
            heartsRect.sizeDelta.y +
            policeRect.sizeDelta.y +
            totalScoreRect.sizeDelta.y +
            rankingRect.sizeDelta.y +
            continueRect.sizeDelta.y;

        Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
        tempAnchorPos.y = Mathf.Max(0, finalContentSize - phoneViewportHeight);
        scrollContentRect.anchoredPosition = tempAnchorPos;

        int minutes = Mathf.FloorToInt(timeItTookToFinish / 60f);
        int seconds = Mathf.FloorToInt(timeItTookToFinish % 60f);
        if (timeItTookToFinish != 0) timeTakenTextBox.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        else timeTakenTextBox.text = "<color=red>FAIL<color=red>";

        if (timeItTookToFinish != 0) timePointsTextBox.text = $"{Mathf.Round(remainingTime)} secs remaining = <color=green>{Mathf.Round(remainingTime)}p</color>";
        else timePointsTextBox.text = $"{Mathf.Round(remainingTime)} secs remaining = {Mathf.Round(remainingTime)}p";

        if (cashAtEndOfRun != 0) moneyPointsTextBox.text = $"${cashAtEndOfRun} = <color=green>{cashAtEndOfRun}p</color>";
        else moneyPointsTextBox.text = $"${cashAtEndOfRun} = {cashAtEndOfRun}p";

        heartsPointsTextBox.text = $"({startingHealth - heartsAtEndOfRun})x30 = <color=red>{0 - ((startingHealth - heartsAtEndOfRun) * 30)}p</color>";

        policePointsTextBox.text = $"({policeKillsAtEndOfRun})x50 = <color=red>{0 - (policeKillsAtEndOfRun * 50)}p</color>";

        int totalScore = Mathf.FloorToInt(Mathf.Round(remainingTime) + cashAtEndOfRun - ((startingHealth - heartsAtEndOfRun) * 30) - (policeKillsAtEndOfRun * 50));
        totalPointsTextBox.text = $"Point Total: {totalScore}p";

        int bankMoneyAdded;
        if (totalScore < 0) bankMoneyAdded = 10;
        else if (totalScore < 50) bankMoneyAdded = 20;
        else if (totalScore < 150) bankMoneyAdded = 50;
        else if (totalScore < 350) bankMoneyAdded = 150;
        else if (totalScore < 500) bankMoneyAdded = 500;
        else if (totalScore < 750) bankMoneyAdded = 1000;
        else bankMoneyAdded = 1000;
        bankAllowanceTextBox.text = $"Allowance Added: ${bankMoneyAdded}";
        GameData.Instance.AddBankMoney(bankMoneyAdded);

        if (totalScore < 0) rankingImage.sprite = fRankImage;
        else if (totalScore < 50) rankingImage.sprite = dRankImage;
        else if (totalScore < 150) rankingImage.sprite = cRankImage;
        else if (totalScore < 350) rankingImage.sprite = bRankImage;
        else if (totalScore < 500) rankingImage.sprite = aRankImage;
        else if (totalScore < 750) rankingImage.sprite = sRankImage;
        else rankingImage.sprite = sRankImage;
        rankingImage.gameObject.SetActive(true);

        ToggleElements(true);
        StopCoroutine(resultsShowCoroutine);
        resultsEnded = true;
        scrollRect.vertical = true;
    }

    private void ShowResults()
    {
        if (resultsShowCoroutine != null) StopCoroutine(resultsShowCoroutine);
        resultsShowCoroutine = StartCoroutine(ShowTheResults());
    }


    private IEnumerator ShowTheResults()
    {
        float timer = 0;
        float totalContentSize = 0;
        scrollRect.vertical = false;

        while (timer < timeToStart) { timer += Time.fixedDeltaTime; yield return null; }
        timer = 0;

        canSkip = true;

        if (headerRect != null)
        {
            totalContentSize += headerRect.sizeDelta.y;
            Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
            tempAnchorPos.y = Mathf.Max(0, totalContentSize - phoneViewportHeight);
            scrollContentRect.anchoredPosition = tempAnchorPos;
            headerRect.GetComponent<CanvasGroup>().alpha = 1;
            audioSource.PlayOneShot(sectionAppearSFX);

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
        }

        if (timeRect != null)
        {
            Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
            tempAnchorPos.y = Mathf.Max(0, totalContentSize - phoneViewportHeight);
            scrollContentRect.anchoredPosition = tempAnchorPos;
            timeRect.GetComponent<CanvasGroup>().alpha = 1;
            totalContentSize += timeRect.sizeDelta.y;

            timeHeaderTextBox.GetComponent<CanvasGroup>().alpha = 1;
            audioSource.PlayOneShot(sectionAppearSFX);

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
            int minutes = Mathf.FloorToInt(timeItTookToFinish / 60f);
            int seconds = Mathf.FloorToInt(timeItTookToFinish % 60f);
            if (timeItTookToFinish != 0) timeTakenTextBox.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            else timeTakenTextBox.text = "<color=red>FAIL<color=red>";
            timeTakenTextBox.GetComponent<CanvasGroup>().alpha = 1;


            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
            timePointsTextBox.GetComponent<CanvasGroup>().alpha = 1;

            if (timeItTookToFinish != 0)
            {
                while (timer < timeToDisplayScore)
                {
                    timer += Time.fixedDeltaTime;
                    timePointsTextBox.text = $"{Mathf.Round(remainingTime)} secs remaining = <color=green>{Mathf.Round(remainingTime * (timer / timeToDisplayScore))}p</color>";
                    yield return null;
                }
                timePointsTextBox.text = $"{Mathf.Round(remainingTime)} secs remaining = <color=green>{Mathf.Round(remainingTime)}p</color>";
            }
            else timePointsTextBox.text = $"{Mathf.Round(remainingTime)} secs remaining = {Mathf.Round(remainingTime)}p";
            timer = 0;

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
        }

        if (moneyRect != null)
        {
            totalContentSize += moneyRect.sizeDelta.y;
            Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
            tempAnchorPos.y = Mathf.Max(0, totalContentSize - phoneViewportHeight);
            scrollContentRect.anchoredPosition = tempAnchorPos;
            moneyRect.GetComponent<CanvasGroup>().alpha = 1;

            moneyHeaderTextBox.GetComponent<CanvasGroup>().alpha = 1;
            audioSource.PlayOneShot(sectionAppearSFX);

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
            moneyPointsTextBox.GetComponent<CanvasGroup>().alpha = 1;

            if (cashAtEndOfRun != 0)
            {
                while (timer < timeToDisplayScore)
                {
                    timer += Time.fixedDeltaTime;
                    moneyPointsTextBox.text = $"${cashAtEndOfRun} = <color=green>{Mathf.Round(cashAtEndOfRun * (timer / timeToDisplayScore))}p</color>";
                    yield return null;
                }
                moneyPointsTextBox.text = $"${cashAtEndOfRun} = <color=green>{cashAtEndOfRun}p</color>";
            }
            else moneyPointsTextBox.text = $"${cashAtEndOfRun} = {cashAtEndOfRun}p";
            timer = 0;

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
        }

        if (heartsRect != null)
        {
            totalContentSize += heartsRect.sizeDelta.y;
            Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
            tempAnchorPos.y = Mathf.Max(0, totalContentSize - phoneViewportHeight);
            scrollContentRect.anchoredPosition = tempAnchorPos;
            heartsRect.GetComponent<CanvasGroup>().alpha = 1;

            heartsHeaderTextBox.GetComponent<CanvasGroup>().alpha = 1;
            audioSource.PlayOneShot(sectionAppearSFX);

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
            heartsPointsTextBox.GetComponent<CanvasGroup>().alpha = 1;

            if (startingHealth - heartsAtEndOfRun != 0)
            {
                while (timer < timeToDisplayScore)
                {
                    timer += Time.fixedDeltaTime;
                    heartsPointsTextBox.text = $"({startingHealth - heartsAtEndOfRun})x30 = <color=red>{Mathf.Round(0-(((startingHealth - heartsAtEndOfRun) * 30) * (timer / timeToDisplayScore)))}p</color>";
                    yield return null;
                }
            }
            timer = 0;
            heartsPointsTextBox.text = $"({startingHealth - heartsAtEndOfRun})x30 = <color=red>{0 - ((startingHealth - heartsAtEndOfRun) * 30)}p</color>";

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
        }

        if (policeRect != null)
        {
            totalContentSize += policeRect.sizeDelta.y;
            Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
            tempAnchorPos.y = Mathf.Max(0, totalContentSize - phoneViewportHeight);
            scrollContentRect.anchoredPosition = tempAnchorPos;
            policeRect.GetComponent<CanvasGroup>().alpha = 1;

            policeHeaderTextBox.GetComponent<CanvasGroup>().alpha = 1;
            audioSource.PlayOneShot(sectionAppearSFX);

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
            policePointsTextBox.GetComponent<CanvasGroup>().alpha = 1;

            if (policeKillsAtEndOfRun != 0)
            {
                while (timer < timeToDisplayScore)
                {
                    timer += Time.fixedDeltaTime;
                    policePointsTextBox.text = $"({policeKillsAtEndOfRun})x50 = <color=red>{Mathf.Round(0 - ((policeKillsAtEndOfRun * 50) * (timer / timeToDisplayScore)))}p</color>";
                    yield return null;
                }
            }
            timer = 0;
            policePointsTextBox.text = $"({policeKillsAtEndOfRun})x50 = <color=red>{0 - (policeKillsAtEndOfRun * 50) }p</color>";

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
        }

        int totalScore = Mathf.FloorToInt(Mathf.Round(remainingTime) + cashAtEndOfRun - ((startingHealth - heartsAtEndOfRun) * 30) - (policeKillsAtEndOfRun * 50));
        if (totalScoreRect != null)
        {
            totalContentSize += totalScoreRect.sizeDelta.y;
            Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
            tempAnchorPos.y = Mathf.Max(0, totalContentSize - phoneViewportHeight);
            scrollContentRect.anchoredPosition = tempAnchorPos;
            totalScoreRect.GetComponent<CanvasGroup>().alpha = 1;

            totalPointsTextBox.GetComponent<CanvasGroup>().alpha = 1;
            audioSource.PlayOneShot(sectionAppearSFX);

            while (timer < timeToDisplayScore)
            {
                timer += Time.fixedDeltaTime;
                totalPointsTextBox.text = $"Point Total: {Mathf.Round(totalScore * (timer / timeToDisplayScore))}p";
                yield return null;
            }
            timer = 0;
            totalPointsTextBox.text = $"Point Total: {totalScore}p";

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
        }

        int bankMoneyAdded;
        if (totalScore < 0) bankMoneyAdded = 10;
        else if (totalScore < 50) bankMoneyAdded = 20;
        else if (totalScore < 150) bankMoneyAdded = 50;
        else if (totalScore < 350) bankMoneyAdded = 150;
        else if (totalScore < 500) bankMoneyAdded = 500;
        else if (totalScore < 750) bankMoneyAdded = 1000;
        else bankMoneyAdded = 1000;
        if (bankAllowanceRect != null)
        {
            totalContentSize += bankAllowanceRect.sizeDelta.y;
            Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
            tempAnchorPos.y = Mathf.Max(0, totalContentSize - phoneViewportHeight);
            scrollContentRect.anchoredPosition = tempAnchorPos;
            bankAllowanceRect.GetComponent<CanvasGroup>().alpha = 1;

            bankAllowanceTextBox.GetComponent<CanvasGroup>().alpha = 1;
            audioSource.PlayOneShot(sectionAppearSFX);

            while (timer < timeToDisplayScore)
            {
                timer += Time.fixedDeltaTime;
                bankAllowanceTextBox.text = $"Allowance Added: ${Mathf.Round(bankMoneyAdded * (timer / timeToDisplayScore))}";
                yield return null;
            }
            timer = 0;
            bankAllowanceTextBox.text = $"Allowance Added: ${bankMoneyAdded}";
            GameData.Instance.AddBankMoney(bankMoneyAdded);

            while (timer < timeToDisplayLine) { timer += Time.fixedDeltaTime; yield return null; }
            timer = 0;
        }

        if (rankingRect != null)
        {
            totalContentSize += rankingRect.sizeDelta.y;
            Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
            tempAnchorPos.y = Mathf.Max(0, totalContentSize - phoneViewportHeight);
            scrollContentRect.anchoredPosition = tempAnchorPos;
            rankingRect.GetComponent<CanvasGroup>().alpha = 1;
            audioSource.PlayOneShot(sectionAppearSFX);

            if (totalScore < 0) rankingImage.sprite = fRankImage;
            else if (totalScore < 50) rankingImage.sprite = dRankImage;
            else if (totalScore < 150) rankingImage.sprite = cRankImage;
            else if (totalScore < 350) rankingImage.sprite = bRankImage;
            else if (totalScore < 500) rankingImage.sprite = aRankImage;
            else if (totalScore < 750) rankingImage.sprite = sRankImage;
            else rankingImage.sprite = sRankImage;
            rankingImage.gameObject.SetActive(true);

            while (timer < 2) { timer += Time.fixedDeltaTime; yield return null; }
        }

        if (continueRect != null)
        {
            totalContentSize += continueRect.sizeDelta.y;
            Vector2 tempAnchorPos = scrollContentRect.anchoredPosition;
            tempAnchorPos.y = Mathf.Max(0, totalContentSize - phoneViewportHeight);
            scrollContentRect.anchoredPosition = tempAnchorPos;
            continueRect.GetComponent<CanvasGroup>().alpha = 1;
            audioSource.PlayOneShot(sectionAppearSFX);

            continueButton.gameObject.SetActive(true);
        }

        scrollRect.vertical = true;
        resultsEnded = true;
        yield break;
    }
}
