using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void PlayAgain()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
