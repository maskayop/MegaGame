namespace MegaGame
{
    public class PirateLair : BaseSettlement
    {
        protected override void OnTriggerEnterUpdate()
        {
            for (int i = 0; i < targetEnemies.Count; i++)
            {
                if (targetEnemies[i].owner != Owner.neutral)
                {
                    ShowPirateLair(true);
                    return;
                }
            }
        }

        public void ShowPirateLair(bool state)
        {
            settlementVisual.GetVisualObject().SetActive(state);
        }
    }
}
