using UnityEngine;
using UnityEngine.AI;

public class FoodManager : MonoBehaviour
{
    public string normalFoodTag = "NormalFood";
    public string powerFoodTag = "PowerFood";
    public string megaFoodTag = "MegaFood";
    public string crackAttackTag = "CrackAttack";

    public float spawnInterval = 2f;
    public float despawnTime = 10f;
    public Vector3 spawnAreaMin = new Vector3(-44f, 0f, -22f);
    public Vector3 spawnAreaMax = new Vector3(44f, 0f, 22f);

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
        string foodTag = ChooseFoodTag();
        Vector3 randomPos = GetValidSpawnPosition();

        if (foodTag == megaFoodTag)
        {
            CrackAttackManager.Instance?.RegisterMegaFoodEaten(); // <-- register the mega food
        }
        if (randomPos != Vector3.zero)
        {
            GameObject food = ObjectPooler.Instance.SpawnFromPool(foodTag, randomPos, Quaternion.identity);

            // Assign despawn time if the script is attached
            FoodLifespan lifespan = food.GetComponent<FoodLifespan>();
            if (lifespan != null)
                lifespan.ResetTimer(despawnTime);
        }
    }

    string ChooseFoodTag()
    {
        float roll = Random.Range(0f, 100f);
        if (roll <= megaFoodChance)
            return megaFoodTag;
        else if (roll <= megaFoodChance + powerFoodChance)
            return powerFoodTag;
        else
            return normalFoodTag;
    }

    Vector3 GetValidSpawnPosition()
    {
        for (int attempts = 0; attempts < 10; attempts++)
        {
            Vector3 candidate = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                51.0f,
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                return hit.position;
        }

        Debug.LogWarning("Failed to find valid spawn position on NavMesh.");
        return Vector3.zero;
    }
}
