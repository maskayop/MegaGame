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
