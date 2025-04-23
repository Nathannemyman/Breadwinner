using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WinCon : MonoBehaviour
{
    public UnityEvent onGameWin = default;

    public PogoStickMovement player;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PogoStickMovement _))
        {
            if (GameManager.Instance.HasBread)
            {
                Timer gameTimer = GameObject.FindAnyObjectByType<Timer>();
                if (gameTimer != null) gameTimer.SetTimeOnGameWin();
                onGameWin?.Invoke();
            }
        }
    }
}
