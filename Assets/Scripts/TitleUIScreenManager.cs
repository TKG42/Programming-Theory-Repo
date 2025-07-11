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
        titleScreenCanvas.SetActive(true);
        howToPlayCanvas.SetActive(false);
        highScoreCanvas.SetActive(false);
    }

    public void ShowHowToPlay()
    {      
        titleScreenCanvas.SetActive(false);
        howToPlayCanvas.SetActive(true);
        highScoreCanvas.SetActive(false);
    }

    public void ShowHighScores()
    {       
        titleScreenCanvas.SetActive(false);
        howToPlayCanvas.SetActive(false);
        highScoreCanvas.SetActive(true);
    }
}
