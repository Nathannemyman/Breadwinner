using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WinCon : MonoBehaviour
{
    public UnityEvent onGameWin = default;

    private bool GameWon = false;
    public PogoStickMovement player;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PogoStickMovement player))
        {
            if (GameManager.Instance.HasBread)
            {
                GameWon = true;
                onGameWin?.Invoke();
            }
        }
    }
}
