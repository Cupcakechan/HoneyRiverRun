using UnityEngine;
using TMPro;

/// Wires the Main Menu buttons. Lives on a MenuController object in the MainMenu scene.
public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject highScorePanel;

    [Header("High Score Display")]
    [SerializeField] private TMP_Text highScoreValueText;

    public void OnPlayPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartRun();   // resets lives to 3 + enters Playing
        SceneLoader.LoadGameplay();
    }

    public void OpenHowToPlay()  => howToPlayPanel.SetActive(true);
    public void CloseHowToPlay() => howToPlayPanel.SetActive(false);

    public void OpenHighScores()
    {
        if (highScoreValueText != null && GameManager.Instance != null)
            highScoreValueText.text = GameManager.Instance.HighScore.ToString();
        highScorePanel.SetActive(true);
    }
    public void CloseHighScores() => highScorePanel.SetActive(false);
}