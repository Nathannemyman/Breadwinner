using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PageViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pageNumberText;
    [SerializeField] private bool loopAround;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private UnityEvent onFinishViewing;

    private int currentPageIndex;
    
    private void OnEnable()
    {
        if (pages.Length <= 0)
        {
            Debug.LogError("Page Viewer has no pages");
            return;
        }
        
        currentPageIndex = 0;
        SelectPage();
    }

    public void GoLeft()
    {
        if (!loopAround && currentPageIndex == 0) return;
       
        if (loopAround) currentPageIndex = (currentPageIndex - 1 + pages.Length) % pages.Length;
        else currentPageIndex--;

        SelectPage();
    }

    public void GoRight()
    {
        if (!loopAround && currentPageIndex + 1 == pages.Length)
        {
            onFinishViewing?.Invoke();
            return;
        }
       
        if (loopAround) currentPageIndex = (currentPageIndex + 1) % pages.Length;
        else currentPageIndex++;

        SelectPage();
    }

    private void SelectPage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i].activeSelf) pages[i].SetActive(false);
        }
        pages[currentPageIndex].SetActive(true);
        if (pageNumberText != null) pageNumberText.text = (currentPageIndex + 1).ToString();
    }

    public void FreezeForRead(bool value)
    {
        Time.timeScale = value ? 0 : 1;
    }
}
