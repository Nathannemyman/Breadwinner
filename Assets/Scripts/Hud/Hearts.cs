using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hearts : MonoBehaviour
{
    public GameObject Heart_1;
    public GameObject Heart_2;
    public GameObject Heart_3;

    void Start()
    {
        GameManager.Instance.playerHearts = 3;
        UpdateHeartDisplay();
    }

    void Update()
    {
        // Update hearts constnatly
        UpdateHeartDisplay();
    }

    void UpdateHeartDisplay()
    {
        GameManager.Instance.playerHearts = Mathf.Clamp(GameManager.Instance.playerHearts, 0, 3);

        switch (GameManager.Instance.playerHearts)
        {
            case 0:
                {
                    Heart_1.gameObject.SetActive(false);
                    Heart_2.gameObject.SetActive(false);
                    Heart_3.gameObject.SetActive(false);
                    SceneManager.LoadScene(3);
                    break;
                }
            case 1:
                {
                    Heart_1.gameObject.SetActive(true);
                    Heart_2.gameObject.SetActive(false);
                    Heart_3.gameObject.SetActive(false);
                    break;
                }
            case 2:
                {
                    Heart_1.gameObject.SetActive(true);
                    Heart_2.gameObject.SetActive(true);
                    Heart_3.gameObject.SetActive(false);
                    break;
                }
            case 3:
                {
                    Heart_1.gameObject.SetActive(true);
                    Heart_2.gameObject.SetActive(true);
                    Heart_3.gameObject.SetActive(true);
                    break;
                }
        }
    }
}