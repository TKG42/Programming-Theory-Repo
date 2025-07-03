using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SlamVFXController : MonoBehaviour
{
    private BaseSnake snake;
    private List<Transform> vfxObjects = new List<Transform>();
    private float[] vfxScales = new float[] { 1f, 1.2f, 1.5f };
    private string vfxPoolTag = "ElectricVFX";

    private Coroutine vfxCoroutine = null;
    private bool isActivating = false;

    private void Awake()
    {
        snake = GetComponent<BaseSnake>();
    }

    public void ActivateSlamVFX(int level)
    {
        if (vfxCoroutine != null)
            StopCoroutine(vfxCoroutine);

        isActivating = true;
        vfxCoroutine = StartCoroutine(ActivateVFXSequence(level));
    }

    private IEnumerator ActivateVFXSequence(int level)
    {
        vfxObjects.Clear();

        Transform headVFX = ActivateVFXOnTransform(transform);
        if (headVFX != null)
        {
            SetScale(headVFX, vfxScales[level - 1]);
            vfxObjects.Add(headVFX);
            yield return new WaitForSeconds(0.1f);
        }

        List<Transform> segmentsSnapshot = new List<Transform>(snake.bodySegments);
        foreach (Transform segment in segmentsSnapshot)
        {
            if (!isActivating) yield break; // Bail early if interrupted

            if (segment == null) continue;
            Transform vfx = ActivateVFXOnTransform(segment);
            if (vfx != null)
            {
                SetScale(vfx, vfxScales[level - 1]);
                vfxObjects.Add(vfx);
                yield return new WaitForSeconds(0.1f);
            }
        }

        vfxCoroutine = null;
        isActivating = false;
    }

    public void InterruptAndClearVFX()
    {
        isActivating = false;

        if (vfxCoroutine != null)
        {
            StopCoroutine(vfxCoroutine);
            vfxCoroutine = null;
        }

        DeactivateAll();
    }

    public void AddSegmentVFX(Transform newSegment, int currentLevel)
    {
        Transform vfx = newSegment.Find("electric_vfx");

        if (vfx == null)
        {
            // fallback to pooled if prefab didn't include child
            GameObject pooled = ObjectPooler.Instance.SpawnFromPool(vfxPoolTag, newSegment.position, newSegment.rotation);
            if (pooled != null)
            {
                pooled.transform.SetParent(newSegment);
                pooled.transform.localPosition = Vector3.zero;
                pooled.transform.localRotation = Quaternion.identity;
                pooled.SetActive(true);
                vfx = pooled.transform;
            }
        }

        if (vfx != null)
        {
            vfx.gameObject.SetActive(true); // Ensure VFX is visible
            SetScale(vfx, vfxScales[Mathf.Clamp(currentLevel - 1, 0, vfxScales.Length - 1)]);
            if (!vfxObjects.Contains(vfx))
                vfxObjects.Add(vfx);
        }
    }

    public void DowngradeVFXLevel(int newLevel)
    {
        Debug.Log("Downgrading VFX level to: " + newLevel);
        isActivating = false;

        if (vfxCoroutine != null)
        {
            StopCoroutine(vfxCoroutine);
            vfxCoroutine = null;
        }

        // Ensure all expected segments have VFX applied
        EnsureFullVFXList(newLevel);

        // Apply new scale
        ApplyScale(newLevel);
    }

    private void EnsureFullVFXList(int level)
    {
        Transform headVFX = ActivateVFXOnTransform(transform);
        if (headVFX != null && !vfxObjects.Contains(headVFX))
        {
            SetScale(headVFX, vfxScales[Mathf.Clamp(level - 1, 0, vfxScales.Length - 1)]);
            vfxObjects.Add(headVFX);
        }

        foreach (Transform segment in snake.bodySegments)
        {
            if (segment == null) continue;
            Transform vfx = ActivateVFXOnTransform(segment);
            if (vfx != null && !vfxObjects.Contains(vfx))
            {
                SetScale(vfx, vfxScales[Mathf.Clamp(level - 1, 0, vfxScales.Length - 1)]);
                vfxObjects.Add(vfx);
            }
        }
    }

    public void UpdateSlamVFXScale(int level)
    {
        ApplyScale(level);
    }

    public void DeactivateAll()
    {
        foreach (Transform vfx in vfxObjects)
        {
            if (vfx != null) vfx.gameObject.SetActive(false);
        }
        vfxObjects.Clear();
    }

    private void ApplyScale(int level)
    {
        float scale = vfxScales[Mathf.Clamp(level - 1, 0, vfxScales.Length - 1)];
        Debug.Log("Applying scale for level " + level + " at scale " + scale);
        foreach (Transform vfx in vfxObjects)
        {
            SetScale(vfx, scale);
        }
    }

    private Transform ActivateVFXOnTransform(Transform target)
    {
        Transform existing = target.Find("electric_vfx");
        if (existing != null)
        {
            existing.gameObject.SetActive(true);
            return existing;
        }

        // fallback to pooled object (for segment prefabs that do not have embedded vfx)
        GameObject pooled = ObjectPooler.Instance.SpawnFromPool(vfxPoolTag, target.position, target.rotation);
        if (pooled != null)
        {
            pooled.transform.SetParent(target);
            pooled.transform.localPosition = Vector3.zero;
            pooled.transform.localRotation = Quaternion.identity;
            pooled.SetActive(true);
            return pooled.transform;
        }

        return null;
    }

    private void SetScale(Transform vfx, float scale)
    {
        Debug.Log("Setting scale of " + scale + " to vfx object: " + vfx);
        if (vfx != null)
        {
            vfx.localScale = new Vector3(scale, scale, scale);
        }
    }
}
