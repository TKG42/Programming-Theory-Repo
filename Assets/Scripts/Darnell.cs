using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Darnell : BaseSnake
{
    public GameObject lightBlueSegmentPrefab;
    public GameObject blueSegmentPrefab;
    public GameObject tanSegmentPrefab;

    public Material blinkMaterial;

    public bool HasShield() => hasShield;
    public bool IsInvulnerable() => isTemporarilyInvulnerable;

    private List<Renderer> renderers = new List<Renderer>();
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    private int segmentSpawnCounter = 0;
    private bool hasShield = false;
    private bool isTemporarilyInvulnerable = false;

    private float postShieldInvulnerabilityDuration = 2f;

    protected override void Start()
    {
        base.Start();
        CacheOriginalRenderers();
    }

    private void LateUpdate()
    {
        if (!isTemporarilyInvulnerable)
        {
            RestoreOriginalMaterials(); // just in case
        }
    }

    private void CacheOriginalRenderers()
    {
        renderers.Clear();
        originalMaterials.Clear();

        // Head renderer(s)
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            renderers.Add(r);
            originalMaterials[r] = r.materials;
        }

        // Body segment renderers
        foreach (var segment in bodySegments)
        {
            foreach (var r in segment.GetComponentsInChildren<Renderer>())
            {
                renderers.Add(r);
                originalMaterials[r] = r.materials;
            }
        }
    }


    public override void OnEatPowerFood()
    {
        ScoreManager.Instance.AddPoints(50);
        hasShield = true;
        UIManager.Instance.ShowShieldIcon(true);
        IncreaseSpeed();
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

            TryAttachSlamVFXToSegment(newSegment.transform);
        }

        segmentCount += count;
        CacheOriginalRenderers();
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
        IncreaseSpeed();
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

        float elapsed = 0f;
        float blinkInterval = 0.2f;
        bool isBlinkOn = false;

        while (elapsed < postShieldInvulnerabilityDuration)
        {
            if (isBlinkOn)
                RestoreOriginalMaterials();
            else
                ApplyBlinkMaterial();

            isBlinkOn = !isBlinkOn;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        RestoreOriginalMaterials();
        isTemporarilyInvulnerable = false;
        Debug.Log("Invulnerability ended. Restoring materials.");
    }

    private void ApplyBlinkMaterial()
    {
        if (blinkMaterial == null) return;

        foreach (var r in renderers)
        {
            int count = r.materials.Length;
            Material[] tempMats = new Material[count];
            for (int i = 0; i < count; i++)
            {
                tempMats[i] = blinkMaterial;
            }
            r.materials = tempMats;
        }
    }

    private void RestoreOriginalMaterials()
    {
        foreach (var kvp in originalMaterials)
        {
            if (kvp.Key != null)
                kvp.Key.materials = kvp.Value;
        }
    }

    public void ForceDeathByCrackAttack()
    {
        hasShield = false; // disable shield manually
        isTemporarilyInvulnerable = false;
        UIManager.Instance.ShowShieldIcon(false);
        Debug.Log("Darnell overridden death by CrackAttack");
        base.Die();
    }
}
