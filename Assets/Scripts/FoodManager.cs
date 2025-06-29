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
        Vector3 randomPos = FoodManager.GetValidSpawnPositionStatic(spawnAreaMin, spawnAreaMax);

        if (randomPos != Vector3.zero)
        {
            GameObject food = ObjectPooler.Instance.SpawnFromPool(foodTag, randomPos, Quaternion.identity);

            FoodLifespan lifespan = food.GetComponent<FoodLifespan>();
            if (lifespan != null)
                lifespan.ResetTimer(despawnTime);

            if (foodTag == megaFoodTag)
            {
                CrackAttackManager.Instance?.RegisterMegaFoodEaten();
            }
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

    public static Vector3 GetValidSpawnPositionStatic(Vector3 min, Vector3 max)
    {
        for (int attempts = 0; attempts < 10; attempts++)
        {
            Vector3 candidate = new Vector3(
                Random.Range(min.x, max.x),
                51.0f,
                Random.Range(min.z, max.z)
            );

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                return hit.position;
        }

        Debug.LogWarning("Failed to find valid spawn position on NavMesh.");
        return Vector3.zero;
    }
}
