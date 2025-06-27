using UnityEngine;
using System.Collections;

public class Darnell : BaseSnake
{
    public GameObject lightBlueSegmentPrefab;
    public GameObject blueSegmentPrefab;
    public GameObject tanSegmentPrefab;

    public bool HasShield() => hasShield;
    public bool IsInvulnerable() => isTemporarilyInvulnerable;

    private int segmentSpawnCounter = 0;
    private bool hasShield = false;
    private bool isTemporarilyInvulnerable = false;

    private float postShieldInvulnerabilityDuration = 2f;

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
                offsetRotation,
                bodyRoot // <--- sets parent to bodyRoot
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
            ConsumeShield();
            return;
        }

        if (isTemporarilyInvulnerable) 
        {
            Debug.Log("Darnell is invulnerable - no death triggered.");
            return;
        }

        base.Die();
    }

    public void ConsumeShield()
    {
        hasShield = false;
        UIManager.Instance.ShowShieldIcon(false);
        Debug.Log("Darnell's shield was consumed!");

        // Trigger camera shake
        CameraShake.Instance?.Shake(0.25f, 0.4f); // duration, intensity
        StartCoroutine(TemporaryInvulnerability());

        // Optional: Add shield break VFX or sound here
    }

    private IEnumerator TemporaryInvulnerability()
    {
        isTemporarilyInvulnerable = true;
        yield return new WaitForSeconds(postShieldInvulnerabilityDuration);
        isTemporarilyInvulnerable = false;  
    }
}
