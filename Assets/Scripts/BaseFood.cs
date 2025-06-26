using UnityEngine;

public abstract class BaseFood : MonoBehaviour
{
    protected abstract void OnConsume(BaseSnake snake);

    private void OnEnable()
    {
        Transform vfx = transform.Find("Food_consumed");
        if (vfx != null)
            vfx.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseSnake snake = other.GetComponent<BaseSnake>();
        if (snake != null)
        {
            OnConsume(snake);
            VFXManager.Instance.PlayFoodConsumedVFX(transform);
            gameObject.SetActive(false);
        }
    }
}
