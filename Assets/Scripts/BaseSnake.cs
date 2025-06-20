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

    [SerializeField] private Vector3 segmentRotationEuler = new Vector3(0f, 0f, 90f);

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

            PushTailBack(segmentScript.minDistance); // move tail out of the way
        }

        segmentCount += count;
    }

    private void PushTailBack(float distance)
    {
        if (bodySegments.Count < 2 || bodyRoot == null) return;

        // Use orientation rather than position delta
        Vector3 direction = bodySegments[bodySegments.Count - 2].forward;

        Transform tailRoot = bodyRoot.Find("Tail");
        if (tailRoot == null) return;

        foreach (Transform tailSegment in tailRoot)
        {
            tailSegment.position += direction * distance;
            Debug.Log("Tail pushed back by: " + direction * distance);
        }
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
