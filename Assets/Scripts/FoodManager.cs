using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public GameObject normalFoodPrefab;
    public GameObject powerFoodPrefab;
    public GameObject megaFoodPrefab;

    public float spawnInterval = 2f;
    public Vector3 spawnAreaMin;
    public Vector3 spawnAreaMax;

    [Range(0, 100)] public float normalFoodChance = 70f;
    [Range(0, 100)] public float powerFoodChance = 25f;
    [Range(0, 100)] public float megaFoodChance = 5f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRandomFood();
            timer = 0f;
        }
    }

    void SpawnRandomFood()
    {
        float roll = Random.Range(0f, 100f);
        GameObject foodToSpawn;

        if (roll <= megaFoodChance)
            foodToSpawn = megaFoodPrefab;
        else if (roll <= megaFoodChance + powerFoodChance)
            foodToSpawn = powerFoodPrefab;
        else
            foodToSpawn = normalFoodPrefab;

        Vector3 spawnPos = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            0.5f, // Adjust height as needed
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        Instantiate(foodToSpawn, spawnPos, Quaternion.identity);
    }
}
