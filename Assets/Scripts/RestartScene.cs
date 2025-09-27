using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    [SerializeField] private string sceneName = "Animated_Character_Navigation_Final";

    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Esta función se llama desde el botón UI
    public void RestartGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}
