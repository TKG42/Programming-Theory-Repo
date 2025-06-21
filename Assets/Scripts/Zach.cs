using UnityEngine;

public class Zach : BaseSnake
{
    public GameObject orangeSegmentPrefab;
    public GameObject darkOrangeSegmentPrefab;

    private int segmentSpawnCounter = 0;

    private float comboTimer = 0f;
    private float comboDuration = 3f;
    private bool inCombo = false;

    private void Update()
    {
        if (inCombo)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
                inCombo = false;
        }
    }

    public override void AddSegment(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Transform lastSegment = bodySegments[bodySegments.Count - 1];

            GameObject chosenPrefab = (segmentSpawnCounter % 3 == 2)
                ? darkOrangeSegmentPrefab
                : orangeSegmentPrefab;

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
        if (inCombo)
        {
            ScoreManager.Instance.ActivateMultiplier(2, comboDuration);
        }

        inCombo = true;
        comboTimer = comboDuration;
        ScoreManager.Instance.AddPoints(10);
        AddSegment(1);
        UpdateTailScales();
    }

    public override void OnEatPowerFood()
    {
        // speed boost to be handled elsewhere (like movement script)
        Debug.Log("Zach: Speed boost activated!");
    }
}
