using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;    

    private bool gamePaused = false;
    private Animator anim;

    private void Awake()
    {
        TryGetComponent(out anim);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gamePaused && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0 : 1;
        menu.SetActive(gamePaused);
        anim.SetTrigger("ToggleMenu");
    }
}
