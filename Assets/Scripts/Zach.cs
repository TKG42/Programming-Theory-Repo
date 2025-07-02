using UnityEngine;

public class Zach : BaseSnake
{
    public GameObject orangeSegmentPrefab;
    public GameObject darkOrangeSegmentPrefab;

    private int segmentSpawnCounter = 0;

    private float comboTimer = 0f;
    private float comboDuration = 3f;
    private bool inCombo = false;

    [SerializeField] private float speedBoostAmount = 10f;
    [SerializeField] private float speedBoostDuration = 3f;

    private bool isSpeedBoosted = false;
    private float speedBoostTimer = 0f;
    private float originalSpeed;

    protected override void Start()
    {
        base.Start();
        originalSpeed = moveSpeed;
    }

    private void Update()
    {
        if (inCombo)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
                inCombo = false;
        }

        if (isSpeedBoosted)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0)
            {
                moveSpeed = originalSpeed;
                isSpeedBoosted = false;
                UIManager.Instance?.ShowSpeedIcon(false);
            }
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
        if (!isSpeedBoosted)
        {
            isSpeedBoosted = true;
            moveSpeed += speedBoostAmount;
            UIManager.Instance?.ShowSpeedIcon(true);
        }

        speedBoostTimer = speedBoostDuration; // resets or extends the timer
        Debug.Log("Zach: Speed boost timer extended or activated!");
    }
}
