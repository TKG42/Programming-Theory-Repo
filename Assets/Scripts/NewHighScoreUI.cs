using UnityEngine;
using TMPro;

public class NewHighScoreUI : MonoBehaviour
{
    public static NewHighScoreUI Instance;

    [Header("UI References")]
    public GameObject panel;
    public TMP_InputField nameInput;
    public GameObject gameOverPanel;

    private int pendingScore;
    private HighScoreManager.Difficulty pendingDiff;

    private void Awake()
    {
            Instance = this;
    }

    public void Show(int score, HighScoreManager.Difficulty difficulty)
    {
        pendingScore = score;
        pendingDiff = difficulty;
        panel.SetActive(true);
        nameInput.text = "";
    }

    public void SubmitScore()
    {
        string name = nameInput.text.Trim();
        if (string.IsNullOrEmpty(name)) name = "Snake";
        if (name.Length > 12) name = name.Substring(0, 12);

        HighScoreManager.Instance.AddScore(pendingScore, name, pendingDiff);
        panel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
