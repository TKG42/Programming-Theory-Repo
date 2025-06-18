public class NormalFood : BaseFood
{
    protected override void OnConsume(BaseSnake snake)
    {
        snake.OnEatNormalFood();
    }
}
