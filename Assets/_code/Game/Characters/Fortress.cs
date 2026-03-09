namespace MegaGame
{
    public class Fortress : BaseSettlement
    {
        SettlementFX settlementFX;

        protected override void OnInit()
        {
            base.OnInit();

            settlementFX = GetComponent<SettlementFX>();
        }

        protected override void OnAttack()
        {
            if (settlementFX)
                settlementFX.PlayShotFX(targetEnemies[0].transform.position);
        }
    }
}
