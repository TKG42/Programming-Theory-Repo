public class PowerFood : BaseFood
{
    protected override void OnConsume(BaseSnake snake)
    {
        snake.OnEatPowerFood();
    }
}
