using UnityEngine;

namespace MegaGame
{
    public class Port : BaseCharacter
    {
        [Header("Visual")]
        [SerializeField] GameObject playerVisual;
        [SerializeField] GameObject enemyVisual;
        [SerializeField] GameObject neutralVisual;

        [SerializeField] GameObject playerTarget;
        [SerializeField] GameObject enemyTarget;

        [Header("FX")]
        [SerializeField] ParticleSystem FXShot;

        protected override void OnStart()
        {
            Init();
        }

        protected override void OnInit()
        {
            isCaptured = false;
            SetVisual();
        }

        protected override void OnUpdate()
        {
            if (isCaptured)
                return;
        }

        protected override void OnAttack()
        {
            if (FXShot)
                FXShot.Play();
        }

        protected override void OnKilled()
        {
            UpdateHealthWidget();
            isCaptured = true;
        }

        void SetVisual()
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
