using UnityEngine;

public class Zach : BaseSnake
{
    private float comboTimer = 0f;
    private float comboDuration = 3f;
    private bool inCombo = false;

    private void Update()
    {
        if (inCombo)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
                inCombo = false;
        }
    }

    public override void OnEatNormalFood()
    {
        if (inCombo)
        {
            ScoreManager.Instance.ActivateMultiplier(2, comboDuration);
        }

        inCombo = true;
        comboTimer = comboDuration;
        ScoreManager.Instance.AddPoints(10);
        AddSegment(1);
    }

    public override void OnEatPowerFood()
    {
        // speed boost to be handled elsewhere (like movement script)
        Debug.Log("Zach: Speed boost activated!");
    }
}
