using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SnakeSelector : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;

    public void SelectFloyd() => StartGame(GameSessionData.SnakeType.Floyd);
    public void SelectZach() => StartGame(GameSessionData.SnakeType.Zach);
    public void SelectDarnell() => StartGame(GameSessionData.SnakeType.Darnell);

    private void StartGame(GameSessionData.SnakeType chosenSnake)
    {
        if (GameSessionData.Instance == null) return;

        GameSessionData.Instance.selectedSnake = chosenSnake;
        GameSessionData.Instance.selectedDifficulty = (GameSessionData.Difficulty)difficultyDropdown.value;     

        string sceneToLoad = GameSessionData.Instance.selectedDifficulty.ToString();
        SceneManager.LoadScene(sceneToLoad); // Will be "Easy", "Medium", or "Hard"
    }
}
