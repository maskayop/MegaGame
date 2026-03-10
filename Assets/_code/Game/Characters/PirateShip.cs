using UnityEngine;

namespace MegaGame
{
    public class PirateShip : Warship
    {
        //[SerializeField] float distanceToPointToAppear = 20;

        Warship targetShip;

        float distance;

        Transform homePosition;
        Transform currentTarget;

        protected override void OnInit()
        {
            base.OnInit();

            FindTarget();
        }

        protected override bool CanAddTargetToList(BaseCharacter targetCharacter)
        {
            if (targetCharacter as Port)
                return false;

            if (targetCharacter as Village)
                if (targetCharacter != targetSettlement)
                    return false;

            if (targetCharacter as Fortress)
                return false;

            return true;
        }

        void FindTarget()
        {
            short owner = (short)Random.Range(0, 2);

            if (owner != 0)
            {
                short r = (short)Random.Range(0, gameController.playerVillages.Count);
                targetSettlement = gameController.playerVillages[r];

                if (!targetSettlement)
                    return;

                destinationPosition = targetSettlement.transform;
            }
            else
            {
                short r = (short)Random.Range(0, gameController.enemyVillages.Count);
                targetSettlement = gameController.enemyVillages[r];

                if (!targetSettlement)
                    return;

                destinationPosition = targetSettlement.transform;
            }
        }
    }
}
