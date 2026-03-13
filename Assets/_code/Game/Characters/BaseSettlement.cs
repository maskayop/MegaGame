namespace MegaGame
{
    public class BaseSettlement : BaseCharacter
    {
        public bool isCaptured = false;
        public bool IsCaptured { get { return isCaptured; } }

        protected SettlementConstructions settlementConstructions;
        protected SettlementVisual settlementVisual;
        protected SettlementFX settlementFX;

        protected override void OnInit()
        {
            isCaptured = false;
            SetVisual();

            settlementConstructions = GetComponent<SettlementConstructions>();
            settlementVisual = GetComponent<SettlementVisual>();
            settlementFX = GetComponent<SettlementFX>();

            UpdateCharacteristics();
        }

        protected override void OnUpdate()
        {
            if (isCaptured)
                return;
        }

        protected override void Attack()
        {
            if (isCaptured)
                return;

            base.Attack();
        }

        protected override void OnAttack()
        {
            if (isCaptured)
                return;

            if (targetEnemies.Count == 0)
                return;

            if (!targetEnemies[0])
                return;

            if (settlementFX)
                settlementFX.PlayShotFX(targetEnemies[0].transform.position);
        }

        protected override void OnKilled()
        {
            UpdateHealthWidget();
            isCaptured = true;
        }

        public void SetVisual()
        {
            if (!settlementVisual)
                return;

            settlementVisual.SetVisual();
        }

        public void UpdateCharacteristics()
        {
            if (!settlementConstructions)
                return;

            if (settlementConstructions.fortIsBuilt)
            {
                MaxDamage = damage + settlementConstructions.additionalDamage;
                MaxHealth = health + settlementConstructions.additionalHealth;
                MaxRegeneration = healthRegeneration + settlementConstructions.additionalHealthRegeneration;
            }
            else
            {
                MaxDamage = damage;
                MaxHealth = health;
                MaxRegeneration = healthRegeneration;
            }
        }

        public SettlementConstructions GetSettlementConstructions()
        {
            return settlementConstructions;
        }
    }
}
