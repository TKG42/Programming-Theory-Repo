using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class BaseSnake : MonoBehaviour
{
    [Header("Snake Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;

    public GameObject bodySegmentPrefab;
    public Transform bodyRoot;
    public List<Transform> bodySegments = new List<Transform>();

    [SerializeField] protected Vector3 segmentRotationEuler = new Vector3(0f, 0f, 90f);

    protected int segmentCount = 0;

    protected virtual void Start()
    {
        // Initial setup (may be overridden)
        if (bodySegments.Count == 0 && bodyRoot != null)
        {
            foreach (Transform child in bodyRoot)
                bodySegments.Add(child);
        }

        // Assign self to head controller
        SnakeHeadController head = GetComponent<SnakeHeadController>();
        if (head != null)
            head.snake = this;
    }

    public virtual void AddSegment(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Transform lastSegment = bodySegments[bodySegments.Count - 1];
            GameObject newSegment = Instantiate(
                bodySegmentPrefab,
                lastSegment.position,
                Quaternion.Euler(segmentRotationEuler),
                bodyRoot // <--- sets parent to bodyRoot
            );

            SnakeBodySegment segmentScript = newSegment.GetComponent<SnakeBodySegment>();
            segmentScript.target = lastSegment;
            bodySegments.Add(newSegment.transform);

            if (CrackAttackManager.Instance.HasSlamBuff())
            {
                var vfx = GetComponent<SlamVFXController>();
                if (vfx != null)
                    vfx.AddSegmentVFX(newSegment.transform, CrackAttackManager.Instance.CurrentStacks);
            }
        }

        segmentCount += count;
    }

    protected void UpdateTailScales()
    {
        int count = bodySegments.Count;
        if (count <= 5) return;

        // Tail visual sizes and spacing configs
        float[] tailScales = new float[] { 0.4f, 0.6f, 0.8f };
        float[] tailDistances = new float[] { 0.8f, 1f, 1.2f }; // match indices

        for (int i = 0; i < tailScales.Length; i++)
        {
            int index = count - 1 - i; // Start from last segment and work backward
            if (index < 5) break;

            Transform segment = bodySegments[index];
            float scale = tailScales[i];
            float distance = tailDistances[i];

            segment.localScale = new Vector3(scale, scale, scale);

            SnakeBodySegment segmentScript = segment.GetComponent<SnakeBodySegment>();
            if (segmentScript != null)
                segmentScript.minDistance = distance;
        }

        // Reset any previous tail segments that should return to normal
        for (int i = count - 4; i >= 5; i--)
        {
            Transform segment = bodySegments[i];
            segment.localScale = Vector3.one;

            SnakeBodySegment segmentScript = segment.GetComponent<SnakeBodySegment>();
            if (segmentScript != null)
                segmentScript.minDistance = 1.0f; // or whatever your default is
        }
    }


    public virtual void OnEatNormalFood()
    {
        ScoreManager.Instance.AddPoints(10);
        AddSegment(1);
        UpdateTailScales();
    }

    public virtual void OnEatPowerFood() { }

    public virtual void OnEatMegaFood()
    {
        ScoreManager.Instance.AddPoints(100);
        ScoreManager.Instance.ActivateMultiplier(3, 5f); // 3x for 5 seconds
        AddSegment(3);
        UpdateTailScales();

        CrackAttackManager.Instance?.RegisterMegaFoodEaten();
    }

    public virtual void Die()
    {
        GetComponent<SlamVFXController>()?.DeactivateAll();

        Debug.Log("Snake has died!");
        ExplodeSnake();
        StartCoroutine(HandleDeathDelay());
    }

    private void ExplodeSnake()
    {
        foreach (Transform segment in bodySegments)
        {
            // Try child object called "BodyMesh" (for default Body1/Body2 structure)
            Transform meshTransform = segment.Find("BodyMesh");

            // If not found, try "Segment" (for prefab-based segments)
            if (meshTransform == null)
                meshTransform = segment.Find("Segment");

            // Fallback: if neither found, default to root
            if (meshTransform == null)
                meshTransform = segment;

            // Modify the Rigidbody on the correct child
            Rigidbody rb = meshTransform.GetComponent<Rigidbody>();
            if (rb == null) rb = meshTransform.gameObject.AddComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Disable isTrigger on all colliders
            foreach (Collider col in meshTransform.GetComponentsInChildren<Collider>())
            {
                col.isTrigger = false;
            }

            // Detach follow logic on segment root
            SnakeBodySegment sbs = segment.GetComponent<SnakeBodySegment>();
            if (sbs != null) Destroy(sbs);

            // Add light physics impulse
            Vector3 randomDir = Random.insideUnitSphere.normalized;
            rb.AddForce(randomDir * Random.Range(5f, 20f), ForceMode.Impulse);
        }

        // Apply same to the head
        Rigidbody headRb = GetComponent<Rigidbody>();
        if (headRb == null) headRb = gameObject.AddComponent<Rigidbody>();

        headRb.isKinematic = false;
        headRb.useGravity = true;
        headRb.interpolation = RigidbodyInterpolation.Interpolate;
        headRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Disable isTrigger on the head collider(s)
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.isTrigger = false;
        }

        // Add a strong upward/backward push to emphasize impact
        Vector3 impactForce = -transform.forward + Vector3.up * 0.5f;
        headRb.AddForce(impactForce.normalized * 25f, ForceMode.Impulse);

        // Optionally remove movement control script so player can't move after death
        SnakeHeadController headController = GetComponent<SnakeHeadController>();
        if (headController != null) Destroy(headController);

        // Force physics update this frame
        Physics.SyncTransforms();
    }

    private IEnumerator HandleDeathDelay()
    {
        yield return new WaitForSeconds(4f);

        // Hide gameplay UI
        if (UIManager.Instance != null)
            UIManager.Instance.gameObject.SetActive(false);

        // Trigger proper game over logic
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}
