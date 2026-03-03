using UnityEngine;

namespace MegaGame
{
    public class BaseSettlement : BaseCharacter
    {
        [Header("Visual")]
        [SerializeField] GameObject playerVisual;
        [SerializeField] GameObject enemyVisual;
        [SerializeField] GameObject neutralVisual;

        protected bool isCaptured = false;

        protected SettlementConstructions settlementConstructions;

        protected override void OnInit()
        {
            isCaptured = false;
            SetVisual();

            settlementConstructions = GetComponent<SettlementConstructions>();
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
            playerVisual.gameObject.SetActive(false);
            enemyVisual.gameObject.SetActive(false);
            neutralVisual.gameObject.SetActive(false);

            if (owner == Owner.player)
                playerVisual.gameObject.SetActive(true);
            else if (owner == Owner.enemy)
                enemyVisual.gameObject.SetActive(true);
            else if (owner == Owner.neutral)
                neutralVisual.gameObject.SetActive(true);
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
