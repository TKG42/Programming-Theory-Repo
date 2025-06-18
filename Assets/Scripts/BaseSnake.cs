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
            GameObject newSegment = Instantiate(bodySegmentPrefab, lastSegment.position, lastSegment.rotation);
            SnakeBodySegment segmentScript = newSegment.GetComponent<SnakeBodySegment>();
            segmentScript.target = lastSegment;
            bodySegments.Add(newSegment.transform);
        }
        segmentCount += count;
    }

    public virtual void OnEatNormalFood()
    {
        ScoreManager.Instance.AddPoints(10);
        AddSegment(1);
    }

    public virtual void OnEatPowerFood() { }

    public virtual void OnEatMegaFood()
    {
        ScoreManager.Instance.AddPoints(100);
        ScoreManager.Instance.ActivateMultiplier(3, 5f); // 3x for 5 seconds
        AddSegment(3);
    }

    public virtual void Die()
    {
        Debug.Log("Snake has died!");
        GameManager.Instance.GameOver();
    }

    public int GetCurrentScore() => points;
}
