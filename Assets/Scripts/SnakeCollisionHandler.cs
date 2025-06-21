using UnityEngine;

public class SnakeCollisionHandler : MonoBehaviour
{
    private BaseSnake snake;

    private void Start()
    {
        // If we’re on the head or any object that has a BaseSnake directly
        snake = GetComponentInParent<BaseSnake>();

        // If we’re on a segment, trace from the segment script
        if (snake == null)
        {
            SnakeBodySegment segment = GetComponentInParent<SnakeBodySegment>();
            if (segment != null && segment.target != null)
            {
                snake = segment.target.GetComponentInParent<BaseSnake>();
            }
        }

        Debug.Log($"{gameObject.name} has tag {tag} and belongs to {snake?.name ?? "null snake"}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (snake == null || GameManager.Instance == null) return;

        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Snake collided with an obstacle!");
            snake.Die();
            return;
        }

        if (CompareTag("SnakeHead") && other.CompareTag("SnakeSegment"))
        {
            Debug.Log("Snake collided with itself!");
            snake.Die();
        }
    }
}
