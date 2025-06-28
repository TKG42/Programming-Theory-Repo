using UnityEngine;

public class SnakeBodySegment : MonoBehaviour
{
    public Transform target; // The object to follow (head or previous segment)
    public float followSpeed = 10f;
    public float minDistance = 0.5f;

    private void Update()
    {
        if (target == null) return;

        Vector3 toTarget = target.position - transform.position;

        // Defensive check: NaN position on target or self
        if (float.IsNaN(toTarget.x) || float.IsNaN(toTarget.y) || float.IsNaN(toTarget.z))
            return;

        float distance = toTarget.magnitude;

        // Skip movement if too close or invalid
        if (distance <= minDistance || distance <= Mathf.Epsilon)
            return;

        Vector3 direction = toTarget / distance; // safe normalization
        Vector3 targetPosition = target.position - direction * minDistance;

        // Defensive NaN check before applying move
        if (!float.IsNaN(targetPosition.x) && !float.IsNaN(targetPosition.y) && !float.IsNaN(targetPosition.z))
        {
            float dynamicSpeed = Mathf.Max(followSpeed, distance / Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dynamicSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        target = null; // Prevent late-update from running into NaN after destruction
    }
}
