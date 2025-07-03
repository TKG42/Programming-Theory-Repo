using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeCollisionHandler : MonoBehaviour
{
    public BaseSnake snake;

    private void Start()
    {
        // If we’re on the head or any object that has a BaseSnake directly       
        snake = GameObject.FindGameObjectWithTag("SnakeHead").GetComponent<BaseSnake>();

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

        // Colliding with a wall or obstacle
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            // Only the head should break breakables
            if (SceneManager.GetActiveScene().name != "Easy" && CrackAttackManager.Instance.HasSlamBuff() && other.CompareTag("BreakableObstacle"))
            {
                CrackAttackManager.Instance.ConsumeSlamCharge();

                // Explosion VFX
                VFXManager.Instance?.PlaySlamExplosion(other.transform.position);
                // Cam shake
                CameraShake.Instance?.Shake(0.4f, 0.5f);

                Destroy(other.gameObject); 
                return;
            }

            if (snake is Darnell darnell && darnell.HasShield())
            {
                darnell.ConsumeShield();
                return; // Prevent death
            }

            Debug.Log("Snake collided with an obstacle!");
            snake.GetComponent<SlamVFXController>()?.InterruptAndClearVFX();
            snake.Die();
            return;
        }

        // Colliding with own body - always fatal
        if (CompareTag("SnakeHead") && other.CompareTag("SnakeSegment"))
        {
            Debug.Log("Snake collided with itself!");
            snake.Die();
        }
    }
}
