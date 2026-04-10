using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuSystem : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
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

    private void StartGame()
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
