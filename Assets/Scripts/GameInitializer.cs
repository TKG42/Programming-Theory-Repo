using UnityEngine;
using System.Collections;

public class GameInitializer : MonoBehaviour
{
    public Transform spawnPoint;

    public GameObject floydPrefab;
    public GameObject zachPrefab;
    public GameObject darnellPrefab;

    private void Start()
    {
        StartCoroutine(SpawnSnakeWhenReady());
    }

    private IEnumerator SpawnSnakeWhenReady()
    {
        // Wait for GameManager to exist
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        // Proceed with spawn
        if (GameSessionData.Instance == null)
        {
            Debug.LogError("[GameInitializer] Missing GameSessionData.");
            yield break;
        }

        GameObject chosenSnakePrefab = GetSnakePrefab(GameSessionData.Instance.selectedSnake);
        if (chosenSnakePrefab == null)
        {
            Debug.LogError("[GameInitializer] Chosen snake prefab is null.");
            yield break;
        }

        GameObject snakeInstance = Instantiate(chosenSnakePrefab, spawnPoint.position, spawnPoint.rotation);
        BaseSnake snakeScript = snakeInstance.GetComponentInChildren<BaseSnake>();

        GameManager.Instance.currentSnake = snakeScript;
        Debug.Log($"[GameInitializer] Spawned {snakeScript.name} and assigned to GameManager.");
    }

    private GameObject GetSnakePrefab(GameSessionData.SnakeType snake)
    {
        switch (snake)
        {
            case GameSessionData.SnakeType.Floyd: return floydPrefab;
            case GameSessionData.SnakeType.Zach: return zachPrefab;
            case GameSessionData.SnakeType.Darnell: return darnellPrefab;
            default: return null;
        }
    }
}
