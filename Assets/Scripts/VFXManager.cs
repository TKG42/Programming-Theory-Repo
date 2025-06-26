using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    /// <summary>
    /// Plays a particle effect from the given source (e.g., food item) and detaches it before playing.
    /// </summary>
    public void PlayFoodConsumedVFX(Transform foodTransform)
    {
        Transform vfx = foodTransform.Find("Food_consumed");
        if (vfx != null)
        {
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                // Detach so it survives the destruction of the food
                vfx.SetParent(null);
                ps.Play();

                StartCoroutine(ReattachAfterPlay(vfx, foodTransform, ps.main.duration));
            }
            else
            {
                Debug.LogWarning($"No ParticleSystem found on {vfx.name}");
            }
        }
        else
        {
            Debug.LogWarning($"Food_consumed child not found on {foodTransform.name}");
        }
    }

    private System.Collections.IEnumerator ReattachAfterPlay(Transform vfx, Transform originalParent, float delay)
    {
        yield return new WaitForSeconds(delay);

        vfx.SetParent(originalParent);
        vfx.localPosition = Vector3.zero;
        vfx.localRotation = Quaternion.identity;
        vfx.gameObject.SetActive(false); // reset if needed
    }
}
