using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public void TransitionToScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
