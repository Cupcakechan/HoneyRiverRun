using TMPro;
using UnityEngine;

/// Displays the live score on the HUD. Subscribes to ScoreManager.OnScoreChanged
/// and also self-initializes in Start, matching the FuelBarUI / LivesUI pattern.
public class ScoreUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;   // the ScoreManager in this scene
    [SerializeField] private TMP_Text scoreText;          // the HUD score label

    private void OnEnable()
    {
        if (scoreManager != null) scoreManager.OnScoreChanged += Render;
    }

    private void OnDisable()
    {
        if (scoreManager != null) scoreManager.OnScoreChanged -= Render;
    }

    private void Start()
    {
        if (scoreManager != null) Render(scoreManager.Score);   // paint current value now
    }

    private void Render(int score)
    {
        if (scoreText != null) scoreText.text = score.ToString("D7");
    }
}