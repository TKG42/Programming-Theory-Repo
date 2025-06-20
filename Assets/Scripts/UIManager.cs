using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public TMPro.TMP_Text scoreText;
    public Image multiplier2xImage;
    public Image multiplier3xImage;
    public Image multiplier5xImage;
    public Image shieldIcon;
    public Image speedIcon;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public void UpdateMultiplierVisuals(bool has2x, bool has3x)
    {
        // Show only one multiplier icon based on combo status
        bool show5x = has2x && has3x;
        bool show2x = has2x && !has3x;
        bool show3x = has3x && !has2x;

        multiplier5xImage?.gameObject.SetActive(show5x);
        multiplier2xImage?.gameObject.SetActive(show2x);
        multiplier3xImage?.gameObject.SetActive(show3x);
    }

    public void ShowShieldIcon(bool show)
    {
        shieldIcon?.gameObject.SetActive(show);
    }

    public void ShowSpeedIcon(bool show)
    {
        speedIcon?.gameObject.SetActive(show);
    }
}
