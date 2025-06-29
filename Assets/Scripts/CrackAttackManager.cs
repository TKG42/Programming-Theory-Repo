using UnityEngine;
using System.Collections.Generic;

public class CrackAttackManager : MonoBehaviour
{
    public static CrackAttackManager Instance;

    [Header("Settings")]
    public int megaFoodThreshold = 3;
    public int maxStack = 3;

    [Header("UI Slots")]
    public List<GameObject> slamIcons; // assign 3 icon GameObjects in inspector

    [Header("VFX")]
    public GameObject crackVFXPrefab;

    [Header("Spawn Settings")]
    public string crackAttackPoolTag = "CrackAttack";

    private int megaFoodConsumed = 0;
    private int currentStacks = 0;
    private BaseSnake currentSnake;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void RegisterMegaFoodEaten()
    {
        megaFoodConsumed++;
        if (megaFoodConsumed >= megaFoodThreshold)
        {
            SpawnCrackAttack();
            megaFoodConsumed = 0;
        }
    }

    public void OnCrackAttackEaten(BaseSnake snake, int points)
    {      
        currentSnake = snake;

        if (currentStacks < maxStack)
        {
            currentStacks++;
            ScoreManager.Instance.AddPoints(points);
            UpdateUI();
            PlayCrackVFX(snake.transform.position);
        }
        else
        {
            // Overstack = instant death + VFX
            PlayCrackVFX(snake.transform.position);
            snake.Die(); // immediate game over
        }
    }

    public bool HasSlamBuff()
    {
        return currentStacks > 0;
    }

    public void ConsumeSlamCharge()
    {
        if (currentStacks > 0)
        {
            currentStacks--;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slamIcons.Count; i++)
            slamIcons[i].SetActive(i < currentStacks);
    }

    private void SpawnCrackAttack()
    {
        Vector3 randomPos = GetRandomSpawnPosition(); // define as needed
        GameObject spawned = ObjectPooler.Instance.SpawnFromPool(crackAttackPoolTag, randomPos, Quaternion.identity);

        if (spawned == null)
            Debug.LogWarning("[CrackAttackManager] CrackAttack pool returned null. Is it set up correctly?");
        else
            Debug.Log($"[CrackAttackManager] CrackAttack spawned at {randomPos}");

        FoodLifespan lifespan = spawned.GetComponent<FoodLifespan>();
        if (lifespan != null)
            lifespan.ResetTimer(15f);   
    }

    private void PlayCrackVFX(Vector3 pos)
    {
        if (crackVFXPrefab != null)
            Instantiate(crackVFXPrefab, pos, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return FoodManager.GetValidSpawnPositionStatic(
            new Vector3(-44f, 0f, -85f),
            new Vector3(44f, 0f, -44f)
        );
    }
}
