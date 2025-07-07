using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public BaseSnake currentSnake;
    public GameObject gameOverUI;
    public GameObject pauseUI;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void GameOver()
    {
        int finalScore = ScoreManager.Instance.GetScore();
        var diff = (HighScoreManager.Difficulty)GameSessionData.Instance.selectedDifficulty;

        if (HighScoreManager.Instance.IsHighScore(finalScore, diff))
        {
            Debug.Log("High score detected!");
            NewHighScoreUI.Instance.Show(finalScore, diff);
        }
        else
        {
            Debug.Log("No high score. LOSER!");
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
