using UnityEngine;
using TMPro;

public class NewHighScoreUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public TMP_InputField nameInput;
    public GameObject gameOverPanel;

    private int pendingScore;
    private HighScoreManager.Difficulty pendingDiff;

    public void Show(int score, HighScoreManager.Difficulty difficulty)
    {
        pendingScore = score;
        pendingDiff = difficulty;
        panel.SetActive(true);
        nameInput.text = "";
    }

    public void SubmitScore()
    {
        string rawInput = nameInput.text;
        string name = ProfanityFilter.Sanitize(rawInput);

        HighScoreManager.Instance.AddScore(pendingScore, name, pendingDiff);
        panel.SetActive(false);
        gameOverPanel.SetActive(true);
        AudioManager.Instance?.PlayRecordScratch();
    }
}
