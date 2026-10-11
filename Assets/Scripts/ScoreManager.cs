using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TMP_Text scoreText;
    public TMP_Text feedbackText;

    private int totalScore = 0;

    void Awake() { Instance = this; }

    public void AddScore(int points, string label)
    {
        totalScore += points;
        scoreText.text = "SCORE: " + totalScore;
        CancelInvoke();
        feedbackText.text = label;
        Invoke("ClearFeedback", 1.5f);
    }

    void ClearFeedback() { feedbackText.text = ""; }
}
