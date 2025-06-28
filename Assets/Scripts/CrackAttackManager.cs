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
        if (currentSnake == null)
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
        ObjectPooler.Instance.SpawnFromPool(crackAttackPoolTag, randomPos, Quaternion.identity);
    }

    private void PlayCrackVFX(Vector3 pos)
    {
        if (crackVFXPrefab != null)
            Instantiate(crackVFXPrefab, pos, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // For now, just return a point within bounds — this can be improved later
        float range = 10f;
        return new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
    }
}
