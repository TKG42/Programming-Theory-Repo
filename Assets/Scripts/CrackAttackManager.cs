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

    public int CurrentStacks => currentStacks;

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

        SlamVFXController vfx = currentSnake.GetComponent<SlamVFXController>();

        if (currentStacks < maxStack)
        {
            currentStacks++;
            ScoreManager.Instance.AddPoints(points);
            UpdateUI();
            PlayCrackVFX(snake.transform.position);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.electricSFX);

            if (currentStacks == 1)
                vfx?.ActivateSlamVFX(currentStacks); // First activation
            else
                vfx?.UpdateSlamVFXScale(currentStacks); // Scaling up
        }
        else
        {
            PlayCrackVFX(snake.transform.position);
            vfx?.DeactivateAll(); // Clean up visual effects on overstack death

            // Game over from overstack
            if (snake is Darnell darnell)
            {
                // Bypass shield specifically
                darnell.ForceDeathByCrackAttack();
            }
            else
            {
                snake.Die();
            }
        }
        
    }

    public bool HasSlamBuff()
    {
        return currentStacks > 0;
    }

    public void ConsumeSlamCharge()
    {
        Debug.Log("Consuming slam charge...");
        currentStacks = Mathf.Clamp(currentStacks, 0, maxStack);
        Debug.Log("Stacks: " + currentStacks);

        if (currentStacks > 0)
        {
            Debug.Log("Decrementing stacks...");
            currentStacks--;
            Debug.Log("Updated stacks: " + currentStacks);
            UpdateUI();

            SlamVFXController vfx = currentSnake?.GetComponent<SlamVFXController>();

            Debug.Log("VFX: " + vfx);
            if (currentStacks == 0)
                vfx?.InterruptAndClearVFX();
            else
                vfx?.DowngradeVFXLevel(currentStacks);
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
            lifespan.ResetTimer(30f);   
    }

    private void PlayCrackVFX(Vector3 pos)
    {
        if (crackVFXPrefab != null)
            Instantiate(crackVFXPrefab, pos, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 navPos = FoodManager.GetValidSpawnPositionStatic(
            new Vector3(-44f, 0f, -85f),
            new Vector3(44f, 0f, -44f)
        );

        // Raise the Y value by 2 units
        return new Vector3(navPos.x, navPos.y + 2f, navPos.z);
    }
}
