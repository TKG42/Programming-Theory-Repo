using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class HighScoreDisplay : MonoBehaviour
{
    public TMP_Text easyText;
    public TMP_Text mediumText;
    public TMP_Text hardText;

    private void OnEnable()
    {
        UpdateScores();
    }

    public void UpdateScores()
    {
        easyText.text = FormatScores(HighScoreManager.Instance.LoadScores(HighScoreManager.Difficulty.Easy), "Easy");
        mediumText.text = FormatScores(HighScoreManager.Instance.LoadScores(HighScoreManager.Difficulty.Medium), "Medium");
        hardText.text = FormatScores(HighScoreManager.Instance.LoadScores(HighScoreManager.Difficulty.Hard), "Hard");
    }

    private string FormatScores(List<HighScoreEntry> scores, string header)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(header);
        foreach (var e in scores)
        {
            sb.AppendLine($"{e.playerName} - {e.score}");
        }
        return sb.ToString();
    }
}
