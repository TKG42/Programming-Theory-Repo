using UnityEngine;

public abstract class BaseFood : MonoBehaviour
{
    protected abstract void OnConsume(BaseSnake snake);

    private void OnTriggerEnter(Collider other)
    {
        BaseSnake snake = other.GetComponent<BaseSnake>();
        if (snake != null)
        {
            OnConsume(snake);
            VFXManager.Instance.PlayFoodConsumedVFX(transform);
            Destroy(gameObject);
        }
    }
}
