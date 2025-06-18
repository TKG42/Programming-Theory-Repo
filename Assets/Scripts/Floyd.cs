public class Floyd : BaseSnake
{
    public override void OnEatPowerFood()
    {
        ScoreManager.Instance.ActivateMultiplier(3, 4f);
        AddSegment(2);
    }

    public override void OnEatNormalFood()
    {
        base.OnEatNormalFood();
    }
}
