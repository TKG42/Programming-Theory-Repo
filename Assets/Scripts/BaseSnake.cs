using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class BaseSnake : MonoBehaviour
{
    [Header("Snake Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;

    [Header("Speed Settings")]
    public float speedIncreasePerFood = 0.1f; // Tune as needed
    public float maxMoveSpeed = 20f; // speed clamp

    public GameObject bodySegmentPrefab;
    public Transform bodyRoot;
    public List<Transform> bodySegments = new List<Transform>();

    [SerializeField] protected Vector3 segmentRotationEuler = new Vector3(0f, 0f, 90f);

    protected int segmentCount = 0;
    private float defaultMoveSpeed;

    protected virtual void Start()
    {
        StartCoroutine(DelayedStartMovement());

        defaultMoveSpeed = moveSpeed;

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

    protected virtual IEnumerator DelayedStartMovement()
    {
        SnakeHeadController headController = GetComponent<SnakeHeadController>();
        if (headController != null)
        {
            headController.enabled = false;
            yield return new WaitForSeconds(0.1f); // Or WaitForFixedUpdate()
            headController.enabled = true;
        }
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

    protected virtual void IncreaseSpeed()
    {
        moveSpeed = Mathf.Min(moveSpeed + speedIncreasePerFood, maxMoveSpeed);
        Debug.Log($"[BaseSnake] Speed increased to {moveSpeed}");
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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.eatSFX);
        AddSegment(1);
        UpdateTailScales();
        IncreaseSpeed();
    }

    public virtual void OnEatPowerFood()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.eatSFX);
        IncreaseSpeed();
    }

    public virtual void OnEatMegaFood()
    {
        ScoreManager.Instance.AddPoints(100);
        ScoreManager.Instance.ActivateMultiplier(3, 5f); // 3x for 5 seconds
        AudioManager.Instance.PlaySFX(AudioManager.Instance.eatSFX);
        AddSegment(3);
        UpdateTailScales();
        CrackAttackManager.Instance?.RegisterMegaFoodEaten();
        IncreaseSpeed();
    }

    public virtual void Die()
    {
        GetComponent<SlamVFXController>()?.InterruptAndClearVFX();

        Debug.Log("Snake has died!");
        ExplodeSnake();
       
        AudioManager.Instance.PlaySFX(AudioManager.Instance.wallSplatSFX);

        moveSpeed = defaultMoveSpeed;
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

    protected void TryAttachSlamVFXToSegment(Transform newSegment)
    {
        var crackMgr = CrackAttackManager.Instance;

        if (crackMgr != null && crackMgr.HasSlamBuff())
        {
            var vfx = GetComponent<SlamVFXController>();
            if (vfx != null)
            {
                Debug.Log($"[{name}] Adding VFX to new segment");
                vfx.AddSegmentVFX(newSegment, crackMgr.CurrentStacks);
            }
            else
            {
                Debug.LogWarning($"[{name}] SlamVFXController not found on snake");
            }
        }
        else
        {
            Debug.Log($"[{name}] No slam buff active — skipping VFX");
        }
    }
}
