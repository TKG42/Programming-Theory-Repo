using UnityEngine;

public class TitleUIScreenManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject titleScreenCanvas;
    [SerializeField] private GameObject howToPlayCanvas;
    [SerializeField] private GameObject highScoreCanvas;

    private void Start()
    {
        ShowTitleScreen(); // ensure default state
    }

    public void ShowTitleScreen()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSFX);

        titleScreenCanvas.SetActive(true);
        howToPlayCanvas.SetActive(false);
        highScoreCanvas.SetActive(false);
    }

    public void ShowHowToPlay()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSFX);

        titleScreenCanvas.SetActive(false);
        howToPlayCanvas.SetActive(true);
        highScoreCanvas.SetActive(false);
    }

    public void ShowHighScores()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSFX);

        titleScreenCanvas.SetActive(false);
        howToPlayCanvas.SetActive(false);
        highScoreCanvas.SetActive(true);
    }
}
