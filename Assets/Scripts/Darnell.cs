using UnityEngine;

public class Darnell : BaseSnake
{
    public GameObject lightBlueSegmentPrefab;
    public GameObject blueSegmentPrefab;
    public GameObject tanSegmentPrefab;

    private int segmentSpawnCounter = 0;
    private bool hasShield = false;

    public override void OnEatPowerFood()
    {
        hasShield = true;
        UIManager.Instance.ShowShieldIcon(true);
        Debug.Log("Darnell: Shield activated!");
    }

    public override void AddSegment(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Transform lastSegment = bodySegments[bodySegments.Count - 1];

            GameObject chosenPrefab;

            int patternStep = segmentSpawnCounter % 5;
            if (patternStep == 0 || patternStep == 1)
                chosenPrefab = lightBlueSegmentPrefab;
            else if (patternStep == 2)
                chosenPrefab = blueSegmentPrefab;
            else
                chosenPrefab = tanSegmentPrefab;

            Quaternion offsetRotation = Quaternion.Euler(segmentRotationEuler);

            GameObject newSegment = Instantiate(
                chosenPrefab,
                lastSegment.position,
                offsetRotation
            );

            SnakeBodySegment segmentScript = newSegment.GetComponent<SnakeBodySegment>();
            segmentScript.target = lastSegment;
            bodySegments.Add(newSegment.transform);

            segmentSpawnCounter++;
        }

        segmentCount += count;
    }

    public override void OnEatNormalFood()
    {
        if (hasShield)
        {
            ScoreManager.Instance.ActivateMultiplier(2, 4f);
        }
        ScoreManager.Instance.AddPoints(10);
        AddSegment(1);
        UpdateTailScales();
    }

    public override void Die()
    {
        if (hasShield)
        {
            Debug.Log("Darnell smashed a wall with shield!");
            hasShield = false;
            UIManager.Instance.ShowShieldIcon(false);
        }
        else
        {
            base.Die();
        }
    }
}
