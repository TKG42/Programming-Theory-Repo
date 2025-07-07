using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class HighScoreEntry
{
    public string playerName;
    public int score;

    public HighScoreEntry(string name, int score)
    {
        playerName = name;
        this.score = score;
    }
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScoreEntry> scores = new List<HighScoreEntry>();
}

