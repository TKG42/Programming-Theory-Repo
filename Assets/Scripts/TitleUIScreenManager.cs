using UnityEngine;

public class TitleUIScreenManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject titleScreenCanvas;
    [SerializeField] private GameObject howToPlayCanvas;
    [SerializeField] private GameObject highScoreCanvas;
    [SerializeField] private GameObject creditsCanvas;

    private void Start()
    {
        ShowTitleScreen(); // ensure default state
    }

    public void ShowTitleScreen()
    {      
        titleScreenCanvas.SetActive(true);
        howToPlayCanvas.SetActive(false);
        highScoreCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
    }

    public void ShowHowToPlay()
    {      
        titleScreenCanvas.SetActive(false);
        howToPlayCanvas.SetActive(true);
        highScoreCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
    }

    public void ShowHighScores()
    {       
        titleScreenCanvas.SetActive(false);
        howToPlayCanvas.SetActive(false);
        highScoreCanvas.SetActive(true);
        creditsCanvas.SetActive(false);
    }

    public void ShowCreditsScreen()
    {
        titleScreenCanvas.SetActive(false);
        howToPlayCanvas.SetActive(false);
        highScoreCanvas.SetActive(false);
        creditsCanvas.SetActive(true);

    }
}
