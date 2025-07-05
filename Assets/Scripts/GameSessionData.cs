using UnityEngine;

public class GameSessionData : MonoBehaviour
{
    public static GameSessionData Instance;

    public enum SnakeType { Floyd, Zach, Darnell }
    public enum Difficulty { Easy, Medium, Hard }

    public SnakeType selectedSnake = SnakeType.Floyd;
    public Difficulty selectedDifficulty = Difficulty.Easy;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
