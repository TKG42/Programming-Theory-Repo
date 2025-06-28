using UnityEngine;

public class CrackAttackFood : BaseFood
{
    public int pointsWorth = 250;

    protected override void OnConsume(BaseSnake snake)
    {
        CrackAttackManager.Instance.OnCrackAttackEaten(snake, pointsWorth);
    }
}
