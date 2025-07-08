using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    private const int maxEntries = 3;
    private string[] keys = { "HighScores_Easy", "HighScores_Medium", "HighScores_Hard" };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public enum Difficulty { Easy, Medium, Hard }

    private string GetKey(Difficulty diff) => keys[(int)diff];

    public List<HighScoreEntry> LoadScores(Difficulty diff)
    {
        string json = PlayerPrefs.GetString(GetKey(diff), "");
        if (string.IsNullOrEmpty(json)) return new List<HighScoreEntry>();
        return JsonUtility.FromJson<HighScoreList>(json)?.scores ?? new List<HighScoreEntry>();
    }

    public void SaveScores(Difficulty diff, List<HighScoreEntry> entries)
    {
        HighScoreList list = new HighScoreList { scores = entries };
        string json = JsonUtility.ToJson(list);
        PlayerPrefs.SetString(GetKey(diff), json);
        PlayerPrefs.Save();
    }

    public bool IsHighScore(int score, Difficulty diff)
    {
        var scores = LoadScores(diff);
        if (scores.Count < maxEntries) return true;
        return scores.Any(e => score > e.score);
    }

    public void AddScore(int score, string playerName, Difficulty diff)
    {
        var scores = LoadScores(diff);
        scores.Add(new HighScoreEntry(playerName, score));
        scores = scores.OrderByDescending(e => e.score).Take(maxEntries).ToList();
        SaveScores(diff, scores);
    }
}
