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
                AudioManager.Instance?.ResetMultiplierCycle();
                UpdateUI();
            }
        }
    }

    public void AddPoints(int basePoints)
    {
        // This method demonstrates ENCAPSULATION.
        // This public method updates the private variable baseScore.
        int total = basePoints * (currentMultiplier + bonusMultiplier);
        baseScore += total;
        UpdateUI();
    }

    public void ActivateMultiplier(int value, float duration)
    {
        int prevTotal = currentMultiplier + bonusMultiplier;
        bool wasInactive = bonusMultiplier <= 0;

        bonusMultiplier += value;
        multiplierTimer = duration;

        int newTotal = currentMultiplier + bonusMultiplier;

        if (!AudioManager.Instance) return;

        // Only play once per cycle
        if (!AudioManager.Instance.HasPlayedMultiplierThisCycle())
        {
            if (newTotal >= 4)
            {
                // Play Level2 only if we just reached 5x
                if (prevTotal < 5)
                    AudioManager.Instance.TryPlayMultiplierSFX(5);
            }
            else if (wasInactive)
            {
                // Otherwise play Level1 for new 2x or 3x
                AudioManager.Instance.TryPlayMultiplierSFX(newTotal);
            }
        }

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
