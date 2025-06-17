using UnityEngine;

public class SnakeBodySegment : MonoBehaviour
{
    public Transform target; // The object to follow (head or previous segment)
    public float followSpeed = 10f;
    public float minDistance = 0.5f;

    private void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > minDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 targetPosition = target.position - direction * minDistance;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
    }
}
