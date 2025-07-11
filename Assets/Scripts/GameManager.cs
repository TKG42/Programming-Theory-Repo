using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public NewHighScoreUI newHighScoreUI; 

    public BaseSnake currentSnake;
    public GameObject gameOverUI;
    public GameObject pauseUI;
    private bool isPaused = false;
    private bool isHighScoreUIUp = false;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isHighScoreUIUp)
        {
            TogglePause();
        }
    }

    public void GameOver()
    {
        int finalScore = ScoreManager.Instance.GetScore();
        var diff = (HighScoreManager.Difficulty)GameSessionData.Instance.selectedDifficulty;

        if (finalScore <= 0)
        {
            Debug.Log("No score — skipping high score UI.");
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
            AudioManager.Instance?.PlayRecordScratch();
            return;
        }

        if (HighScoreManager.Instance.IsHighScore(finalScore, diff))
        {
            if (newHighScoreUI != null)
            {
                newHighScoreUI.Show(finalScore, diff);
                isHighScoreUIUp = true;
            }
            else
            {
                Debug.LogError("[GameManager] newHighScoreUI is not assigned — check inspector!");
                Time.timeScale = 0f;
                gameOverUI.SetActive(true);
            }
        }
        else
        {
            Debug.Log("No high score.");
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
            AudioManager.Instance?.PlayRecordScratch();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        StartCoroutine(RestartAndPlayMusic());
    }

    private IEnumerator RestartAndPlayMusic()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);

        yield return null; // wait one frame

        AudioManager.Instance?.PlayMusicForScene(sceneName);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScreen");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
}
