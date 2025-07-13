using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Darnell : BaseSnake
{
    // This class demontrates INHERITANCE.
    // This snake is a unique variant that inherits from the abstract class BaseSnake.

    public GameObject lightBlueSegmentPrefab;
    public GameObject blueSegmentPrefab;
    public GameObject tanSegmentPrefab;

    public Material blinkMaterial;

    public bool HasShield() => hasShield;
    public bool IsInvulnerable() => isTemporarilyInvulnerable;

    private List<Renderer> renderers = new List<Renderer>();
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    private int segmentSpawnCounter = 0;
    public bool hasShield = false;
    public bool isTemporarilyInvulnerable = false;

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
        // This method demonstrates POLYMORPHISM
        // The base method is inherited from the abstact class, and unique attributes are overridden here.

        if (!hasShield)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.metalSFX);
        }

        ScoreManager.Instance.AddPoints(50);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.eatSFX);
        hasShield = true;
        UIManager.Instance.ShowShieldIcon(true);
        IncreaseSpeed();
        Debug.Log("Darnell: Shield activated!");
    }

    public override void AddSegment(int count = 1)
    {
        // This method demonstrates POLYMORPHISM
        // The base method is inherited from the abstact class, and unique attributes are overridden here.

        // This method also demonstrates ABSTRACTION
        // This high-level function defines the complex behavior for adding a new snake body segment.
        // It is referenced in other functions, such as in OnEatNormalFood() below.

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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.eatSFX);
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
        // This method demonstrates ABSTRACTION
        // This high-level function defines the behavior for when a snake uses up its shield.
        // It is referenced in other functions, such as in Die() above.

        hasShield = false;
        UIManager.Instance.ShowShieldIcon(false);
        Debug.Log("Darnell's shield was consumed!");

        // metalSlamSFX
        AudioManager.Instance.PlaySFX(AudioManager.Instance.metalSlamSFX);

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

        AudioManager.Instance.PlaySFX(AudioManager.Instance.deathSFX);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.wallSplatSFX);

        Debug.Log("Darnell overridden death by CrackAttack");
        base.Die();
    }
}
