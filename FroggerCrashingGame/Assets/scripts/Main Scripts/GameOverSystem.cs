using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverSystem : MonoBehaviour
{
    public Button restartButton;
    public Button quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RestartGame()
    {
        // Load the main game scene (assuming "SampleScene" is the gameplay scene)
        SceneManager.LoadScene("MapGenerate");
    }

    private void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}
