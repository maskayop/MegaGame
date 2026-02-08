using UnityEngine;

namespace MegaGame
{
    public class Port : BaseSettlement
    {
        [Header("Target Visual")]
        [SerializeField] GameObject playerTarget;
        [SerializeField] GameObject enemyTarget;

        [Header("FX")]
        [SerializeField] ParticleSystem FXShot;

        protected override void OnAttack()
        {
            if (FXShot)
                FXShot.Play();
        }

        public void SetVisualAsTarget(bool isTarget, Owner targetOwner)
        {
            playerTarget.SetActive(false);
            enemyTarget.SetActive(false);

            if (!isTarget)
                return;

            if (targetOwner == Owner.player)
                playerTarget.SetActive(true);
            else if (targetOwner == Owner.enemy)
                enemyTarget.SetActive(true);
        }
    }
}
