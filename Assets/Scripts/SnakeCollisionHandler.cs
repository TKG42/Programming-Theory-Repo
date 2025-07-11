using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeCollisionHandler : MonoBehaviour
{
    public BaseSnake snake;

    private void Start()
    {
        // Identify snake from head or segment
        snake = GameObject.FindGameObjectWithTag("SnakeHead")?.GetComponent<BaseSnake>();

        if (snake == null)
        {
            SnakeBodySegment segment = GetComponentInParent<SnakeBodySegment>();
            if (segment?.target != null)
            {
                snake = segment.target.GetComponentInParent<BaseSnake>();
            }
        }

        Debug.Log($"{gameObject.name} has tag {tag} and belongs to {snake?.name ?? "null snake"}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (snake == null || GameManager.Instance == null) return;

        bool isObstacleLayer = other.gameObject.layer == LayerMask.NameToLayer("Obstacles");
        bool isBreakable = other.CompareTag("BreakableObstacle");
        bool isSnakeSegment = other.CompareTag("SnakeSegment");

        bool isOuterWall = isObstacleLayer && !isBreakable;
        bool isBreakableWall = isObstacleLayer && isBreakable;

        // 1. Outer wall — instant death
        if (isOuterWall)
        {
            Debug.Log("Snake collided with outer wall!");
            ForceKill(snake);
            return;
        }

        // 2. Breakable wall
        if (isBreakableWall)
        {
            // Break with slam
            if (SceneManager.GetActiveScene().name != "Easy" &&
                CrackAttackManager.Instance.HasSlamBuff())
            {
                CrackAttackManager.Instance.ConsumeSlamCharge();
                AudioManager.Instance.PlaySFX(AudioManager.Instance.wallBreakSFX);
                VFXManager.Instance?.PlaySlamExplosion(other.transform.position);
                CameraShake.Instance?.Shake(0.4f, 0.5f);
                Destroy(other.gameObject);
                return;
            }

            // Darnell shield logic
            if (snake is Darnell darnell)
            {
                if (darnell.HasShield())
                {
                    darnell.ConsumeShield();
                    return;
                }

                if (darnell.isTemporarilyInvulnerable)
                {
                    return;
                }
            }

            Debug.Log("Snake collided with breakable wall — no slam or shield.");
            ForceKill(snake);
            return;
        }

        // 3. Self-collision — instant death
        if (CompareTag("SnakeHead") && isSnakeSegment)
        {
            Debug.Log("Snake collided with itself!");
            ForceKill(snake);
        }
    }

    private void ForceKill(BaseSnake target)
    {
        target.GetComponent<SlamVFXController>()?.InterruptAndClearVFX();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.deathSFX);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.wallSplatSFX);

        if (target is Darnell d)
        {
            // override shield/invuln and force death
            d.hasShield = false;
            d.isTemporarilyInvulnerable = false;
            UIManager.Instance.ShowShieldIcon(false);
            Debug.Log("Force-killing Darnell — bypassing shield/invuln.");
            d.Die(); // Now runs base.Die()
        }
        else
        {
            target.Die();
        }
    }
}
