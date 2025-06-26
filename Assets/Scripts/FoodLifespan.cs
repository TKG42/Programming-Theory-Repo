using UnityEngine;

public class FoodLifespan : MonoBehaviour
{
    private float timer;
    private float maxLifetime;

    public void ResetTimer(float duration)
    {
        maxLifetime = duration;
        timer = 0f;
    }
       
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= maxLifetime)
        {
            gameObject.SetActive(false);
        }

        // Rotate food around Y-axis
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
    }
}
