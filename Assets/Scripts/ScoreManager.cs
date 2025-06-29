using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int baseScore = 0;
    private int currentMultiplier = 1;

    private float multiplierTimer = 0f;
    private int bonusMultiplier = 0;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        if (bonusMultiplier > 0)
        {
            multiplierTimer -= Time.deltaTime;
            if (multiplierTimer <= 0)
            {
                bonusMultiplier = 0;
                UpdateUI();
            }
        }
    }

    public void AddPoints(int basePoints)
    {
        int total = basePoints * (currentMultiplier + bonusMultiplier);
        baseScore += total;
        UpdateUI();
    }

    public void ActivateMultiplier(int value, float duration)
    {
        bonusMultiplier += value;
        multiplierTimer = duration;
        UpdateUI();
    }

    private void UpdateUI()
    {
        UIManager.Instance.UpdateScoreText(baseScore);

        // Determine multiplier breakdown
        bool has2x = bonusMultiplier >= 2;
        bool has3x = bonusMultiplier >= 3;

        // 2 + 3 = 5 only if both are active independently
        bool show5x = (bonusMultiplier == 5); // optional override if you want exact logic

        UIManager.Instance.UpdateMultiplierVisuals(has2x, has3x);
    }

    public void SetBaseMultiplier(int value)
    {
        currentMultiplier = Mathf.Max(1, value);
        UpdateUI();
    }

    public int GetScore() => baseScore;
}
