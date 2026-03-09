using UnityEngine;

namespace MegaGame
{
    public class Port : BaseSettlement
    {
        [Space(20)]
        public bool isBigPort;

        [Header("Target Visual")]
        [SerializeField] GameObject playerTarget;
        [SerializeField] GameObject enemyTarget;
        [SerializeField] GameObject neutralTarget;

        SettlementFX settlementFX;

        protected override void OnInit()
        {
            base.OnInit();

            settlementFX = GetComponent<SettlementFX>();
        }

        protected override void OnAttack()
        {
            if (targetEnemies.Count == 0)
                return;

            if (!targetEnemies[0])
                return;

            if (settlementFX)
                settlementFX.PlayShotFX(targetEnemies[0].transform.position);
        }

        public void SetVisualAsTarget(bool isTarget, Owner targetOwner)
        {
            playerTarget.SetActive(false);
            enemyTarget.SetActive(false);
            neutralTarget.SetActive(false);

            if (!isTarget)
                return;

            if (targetOwner == Owner.player)
                playerTarget.SetActive(true);
            else if (targetOwner == Owner.enemy)
                enemyTarget.SetActive(true);
            else if (targetOwner == Owner.neutral)
                neutralTarget.SetActive(true);
        }
    }
}
