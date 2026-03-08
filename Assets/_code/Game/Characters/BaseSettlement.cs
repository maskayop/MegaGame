namespace MegaGame
{
    public class BaseSettlement : BaseCharacter
    {
        protected bool isCaptured = false;

        protected SettlementConstructions settlementConstructions;
        protected SettlementVisual settlementVisual;

        protected override void OnInit()
        {
            isCaptured = false;
            SetVisual();

            settlementConstructions = GetComponent<SettlementConstructions>();
            settlementVisual = GetComponent<SettlementVisual>();

            UpdateCharacteristics();
        }

        protected override void OnUpdate()
        {
            if (isCaptured)
                return;
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
