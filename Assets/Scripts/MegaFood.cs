public class MegaFood : BaseFood
{
    protected override void OnConsume(BaseSnake snake)
    {
        snake.OnEatMegaFood();
    }
}
