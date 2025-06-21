using UnityEngine;

public class FoodLifespan : MonoBehaviour
{
    public float despawnTime = 10f; // Set in inspector or by FoodManager

    private void Start()
    {
        Destroy(gameObject, despawnTime);
    }

    private void Update()
    {
        // Rotate food around Y-axis
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
    }
}
