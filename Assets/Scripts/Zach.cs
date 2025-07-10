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
    private float baseSpeedWithoutBoost;

    protected override void Start()
    {
        base.Start();
        baseSpeedWithoutBoost = moveSpeed;
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
                isSpeedBoosted = false;
                moveSpeed = baseSpeedWithoutBoost;
                UIManager.Instance?.ShowSpeedIcon(false);
            }
        }
    }

    protected override void IncreaseSpeed()
    {
        // Modify the base speed and then apply boost again if active
        baseSpeedWithoutBoost = Mathf.Min(baseSpeedWithoutBoost + speedIncreasePerFood, maxMoveSpeed);
        moveSpeed = baseSpeedWithoutBoost + (isSpeedBoosted ? speedBoostAmount : 0f);
        Debug.Log($"[Zach] Speed increased to {moveSpeed} (boosted: {isSpeedBoosted})");
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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.eatSFX);
        AddSegment(1);
        UpdateTailScales();
        IncreaseSpeed();
    }

    public override void OnEatPowerFood()
    {
        if (!isSpeedBoosted)
        {
            isSpeedBoosted = true;
            IncreaseSpeed();
            UIManager.Instance?.ShowSpeedIcon(true);
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.eatSFX);
        ScoreManager.Instance.AddPoints(50);

        speedBoostTimer = speedBoostDuration; // resets or extends the timer
        Debug.Log("Zach: Speed boost timer extended or activated!");
    }
}
