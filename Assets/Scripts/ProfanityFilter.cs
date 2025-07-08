using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class ProfanityFilter
{
    // Max name length
    private const int MaxNameLength = 12;

    // Add to list as needed
    private static readonly List<string> bannedWords = new List<string>
    {
        "fuck", "shit", "bitch", "asshole", "dick", "cunt", "nigger", "faggot", "slut", "whore",
        "rape", "hitler", "kkk", "piss", "nazi", "retard", "cock", "bollocks", "twat", "cum",
        "nagger", "nogger", "nignog", "niglet", "dike", "wetback", "spick", "beaner", "penis",
        "titties", "tits", "cocksucker", "kike", "nigga", "africoon", "anal", "chink"
    };

    public static string Sanitize(string input)
    {
        string profaneReplacement = "Venomous Snake";
        string defaultname = "Snake";

        if (string.IsNullOrWhiteSpace(input))
            return defaultname;

        // Normalize: trim, lowercase, remove surrounding spaces
        string name = input.Trim();

        // Step 1: Leetspeak normalization
        string normalized = LeetToAlpha(name.ToLower());

        // Step 2: Ban check
        foreach (string word in bannedWords)
        {
            if (normalized.Contains(word))
                return profaneReplacement;
        }

        // Step 3: Strip all non-alphanumeric characters (except space)
        name = Regex.Replace(name, @"[^a-zA-Z0-9 ]", "");

        // Step 4: Clamp to max length
        if (name.Length > MaxNameLength)
            name = name.Substring(0, MaxNameLength);

        // Step 5: Final fallback check
        if (string.IsNullOrWhiteSpace(name))
            return defaultname;

        return name;
    }

    private static string LeetToAlpha(string input)
    {
        return input
            .Replace("1", "i")
            .Replace("!", "i")
            .Replace("|", "i")
            .Replace("3", "e")
            .Replace("4", "a")
            .Replace("@", "a")
            .Replace("5", "s")
            .Replace("$", "s")
            .Replace("0", "o")
            .Replace("9", "g")
            .Replace("7", "t");
    }
}
