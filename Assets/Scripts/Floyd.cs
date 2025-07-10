using UnityEngine;

public class Floyd : BaseSnake
{
    public GameObject lightGreenSegmentPrefab;
    public GameObject darkGreenSegmentPrefab;

    private int segmentSpawnCounter = 0;

    public override void OnEatPowerFood()
    {
        ScoreManager.Instance.ActivateMultiplier(3, 4f);
        ScoreManager.Instance.AddPoints(50);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.eatSFX);
        AddSegment(2);
        UpdateTailScales();
        IncreaseSpeed();
    }

    public override void OnEatNormalFood()
    {
        base.OnEatNormalFood();
    }

    public override void AddSegment(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Transform lastSegment = bodySegments[bodySegments.Count - 1];

            GameObject chosenPrefab = (segmentSpawnCounter % 3 == 2)
                ? darkGreenSegmentPrefab
                : lightGreenSegmentPrefab;

            Quaternion offsetRotation = Quaternion.Euler(segmentRotationEuler);

            GameObject newSegment = Instantiate(
                chosenPrefab,
                lastSegment.position,
                offsetRotation,
                bodyRoot // <--- sets parent to bodyRoot
                );

            SnakeBodySegment segmentScript = newSegment.GetComponent<SnakeBodySegment>();
            segmentScript.target = lastSegment;
            bodySegments.Add(newSegment.transform);

            segmentSpawnCounter++;

            TryAttachSlamVFXToSegment(newSegment.transform);
        }

        segmentCount += count;
    }
}
