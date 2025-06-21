using UnityEngine;
using System.Collections.Generic;

public abstract class BaseSnake : MonoBehaviour
{
    [Header("Snake Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;

    public GameObject bodySegmentPrefab;
    public Transform bodyRoot;
    public List<Transform> bodySegments = new List<Transform>();

    [SerializeField] protected Vector3 segmentRotationEuler = new Vector3(0f, 0f, 90f);

    protected int points = 0;
    protected int segmentCount = 0;

    protected virtual void Start()
    {
        // Initial setup (may be overridden)
        if (bodySegments.Count == 0 && bodyRoot != null)
        {
            foreach (Transform child in bodyRoot)
                bodySegments.Add(child);
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
                Quaternion.Euler(segmentRotationEuler)
            );

            SnakeBodySegment segmentScript = newSegment.GetComponent<SnakeBodySegment>();
            segmentScript.target = lastSegment;
            bodySegments.Add(newSegment.transform);
        }

        segmentCount += count;
    }

    protected void UpdateTailScales()
    {
        int count = bodySegments.Count;
        if (count <= 5) return;

        // Tail visual sizes and spacing configs
        float[] tailScales = new float[] { 0.4f, 0.6f, 0.8f };
        float[] tailDistances = new float[] { 0.5f, 0.8f, 1.0f }; // match indices

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
    }

    public virtual void Die()
    {
        Debug.Log("Snake has died!");
        GameManager.Instance.GameOver();
    }

    public int GetCurrentScore() => points;
}
