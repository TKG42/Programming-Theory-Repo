using UnityEngine;

public class SnakeHeadController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float turnSpeed = 10f;
    public BaseSnake snake; // assigined at runtime in BaseSnake.Start()
    public LayerMask groundLayer; // Assign a ground layer in the Inspector

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return; // Prevent movement during pause
        FollowMouse();
        MoveForward();
    }

    void FollowMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundLayer))
        {
            Vector3 targetPos = hitInfo.point;
            targetPos.y = transform.position.y; // Keep snake flat on plane

            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
    }

    void MoveForward()
    {
        if (snake == null) return;
        transform.position += transform.forward * snake.moveSpeed * Time.deltaTime;
    }
}
