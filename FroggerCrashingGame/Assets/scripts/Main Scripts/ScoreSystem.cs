using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public TMP_Text scoreText; // Assign your TMP Text component in the Inspector
    private int score = 0;
    private int highScore = 0;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        score += points;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        Debug.Log("Score added: " + points + ". Total score: " + score);
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString() + " High: " + highScore.ToString();
            Debug.Log("Score text updated to: " + scoreText.text);
        }
        else
        {
            Debug.LogError("ScoreText is null! Make sure to assign the TMP_Text component in the Inspector.");
        }
    }
}
