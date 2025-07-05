using UnityEngine;

public class SnakeBodySegment : MonoBehaviour
{
    public Transform target; // The object to follow (head or previous segment)
    public float followSpeed = 10f;
    public float minDistance = 0.5f;

    private static bool hasLoggedPaused = false;
    private bool wasPausedLastFrame = false;

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (!hasLoggedPaused)
            {
                Debug.LogWarning("[Segment] Skipped update due to pause.");
                hasLoggedPaused = true;
            }

            wasPausedLastFrame = true; 
            return;
        }

        if (wasPausedLastFrame)
        {
            wasPausedLastFrame = false; 
            return;
        }

        if (hasLoggedPaused)
            hasLoggedPaused = false;

        if (target == null) return;

        Vector3 toTarget = target.position - transform.position;

        // Defensive check: NaN position on target or self
        if (float.IsNaN(toTarget.x) || float.IsNaN(toTarget.y) || float.IsNaN(toTarget.z))
            return;

        float distance = toTarget.magnitude;

        // Skip movement if too close or invalid
        if (distance <= minDistance || distance <= Mathf.Epsilon)
            return;

        Vector3 direction = toTarget / distance;
        Vector3 targetPosition = target.position - direction * minDistance;

        // Defensive NaN check before applying move
        if (!float.IsNaN(targetPosition.x) && !float.IsNaN(targetPosition.y) && !float.IsNaN(targetPosition.z))
        {
            float dynamicSpeed = Mathf.Max(followSpeed, distance / Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dynamicSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(toTarget);
            float maxTurnAngle = followSpeed * 360f * Time.deltaTime; // Dynamically scale with speed. 540f for tighter turns
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxTurnAngle);
        }
    }

    private void OnDisable()
    {
        target = null; // Prevent late-update from running into NaN after destruction
    }
}
