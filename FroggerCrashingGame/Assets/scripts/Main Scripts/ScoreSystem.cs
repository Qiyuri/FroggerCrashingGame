using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public TMP_Text scoreText; // Assign your TMP Text component in the Inspector
    private int score = 0;

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score added: " + points + ". Total score: " + score);
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
            Debug.Log("Score text updated to: " + scoreText.text);
        }
        else
        {
            Debug.LogError("ScoreText is null! Make sure to assign the TMP_Text component in the Inspector.");
        }
    }
}
