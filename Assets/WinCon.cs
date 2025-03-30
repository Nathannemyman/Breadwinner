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
                onGameWin?.Invoke();
            }
        }
    }
}
