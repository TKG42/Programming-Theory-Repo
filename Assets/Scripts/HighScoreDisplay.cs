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
        sb.AppendLine(); // <- Add space between header and scores

        if (scores.Count == 0)
        {
            sb.AppendLine("—");
            sb.AppendLine("—");
            sb.AppendLine("—");
        }
        else
        {
            foreach (var e in scores)
            {
                string name = string.IsNullOrEmpty(e.playerName) ? "-" : e.playerName;
                sb.AppendLine($"{name} - {e.score}");
            }

            // Fill remaining with dashes if fewer than 3 scores
            for (int i = scores.Count; i < 3; i++)
            {
                sb.AppendLine("—");
            }
        }
        return sb.ToString();
    }
}
