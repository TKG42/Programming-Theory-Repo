using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text multiplierText;
    public Image shieldIcon;
    public Image speedIcon;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateMultiplierText(int multiplier)
    {
        multiplierText.text = multiplier > 1 ? $"x{multiplier}" : "";
    }

    public void ShowShieldIcon(bool show)
    {
        shieldIcon.gameObject.SetActive(show);
    }

    public void ShowSpeedIcon(bool show)
    {
        speedIcon.gameObject.SetActive(show);
    }
}
