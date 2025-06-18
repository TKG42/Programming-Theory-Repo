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
        int total = basePoints * (1 + bonusMultiplier);
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
        UIManager.Instance.UpdateMultiplierText(1 + bonusMultiplier);
    }

    public int GetScore() => baseScore;
}
