using UnityEngine;

namespace MegaGame
{
    public class PirateLair : BaseSettlement
    {
        public bool isActive;

        [SerializeField] GameObject capturedFX;

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (isCaptured)
            {
                isActive = false;
                GetCurrentHealthWidget().SetValue(0);
                GetCurrentHealthWidget().gameObject.SetActive(false);
            }
        }

        protected override void Attack()
        {
            if (isActive)
                base.Attack();
            else
                return;
        }

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

        protected override void UpdateProperties()
        {
            if (isCaptured)
                return;

            if (globalTime.currentDay != currentDay)
            {
                currentHealth += maxRegeneration;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

                currentDay = globalTime.currentDay;
            }
        }

        protected override void OnKilled()
        {
            base.OnKilled();

            isCaptured = true;

            if (isCaptured)
                capturedFX.SetActive(true);

            GetCurrentHealthWidget().SetValue(0);
            GetCurrentHealthWidget().gameObject.SetActive(false);
        }

        public void ShowPirateLair(bool state)
        {
            settlementVisual.GetVisualObject().SetActive(state);

            if (!isCaptured)
                capturedFX.SetActive(false);
            else
                isActive = true;
        }
    }
}
