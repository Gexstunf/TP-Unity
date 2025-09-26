using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    [SerializeField] private string sceneName = "Animated_Character_Navigation_Final";

    // Esta función se llama desde el botón UI
    public void RestartGame()
    {
        // Desbloquea y muestra el cursor antes de cambiar de escena
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(sceneName);
    }
}
