using UnityEngine;

public class Darnell : BaseSnake
{
    private bool hasShield = false;

    public override void OnEatPowerFood()
    {
        hasShield = true;
        UIManager.Instance.ShowShieldIcon(true);
        Debug.Log("Darnell: Shield activated!");
    }

    public override void OnEatNormalFood()
    {
        if (hasShield)
        {
            ScoreManager.Instance.ActivateMultiplier(2, 4f);
        }
        ScoreManager.Instance.AddPoints(10);
        AddSegment(1);
    }

    public override void Die()
    {
        if (hasShield)
        {
            Debug.Log("Darnell smashed a wall with shield!");
            hasShield = false;
            UIManager.Instance.ShowShieldIcon(false);
        }
        else
        {
            base.Die();
        }
    }
}
