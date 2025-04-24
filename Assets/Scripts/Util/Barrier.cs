using UnityEngine;

public class Barrier : MonoBehaviour
{
    public PogoStickMovement player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PogoStickMovement _))
        {
            if (GameManager.Instance.HasBread)
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }
}
