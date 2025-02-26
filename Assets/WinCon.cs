using UnityEngine;

public class WinCon : MonoBehaviour
{
    public bool GameWon = false;
    public PogoStickMovement player;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && player.hasBread)
        {
            GameWon = true;
        }
    }
}
