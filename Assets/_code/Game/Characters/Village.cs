using MegaGame.UI;
using UnityEngine;

namespace MegaGame
{
    public class Village : BaseSettlement
    {
        [SerializeField] int timeForReport = 4;

        float currentTime = 0;

        protected override void OnUpdate()
        {
            base.OnUpdate();

            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
                currentTime = 0;
        }

        protected override void OnTakeDamage(BaseCharacter damager)
        {
            if (!damager)
                return;

            if (damager as PirateShip)
                if (currentTime <= 0)
                    if (owner == Owner.player)
                        ReportPiratesAttack();
        }

        void ReportPiratesAttack()
        {
            currentTime = timeForReport;
            UIMainCanvas.Instance.SpawnPiratesAttackVillageMessage(this);
        }
    }
}
