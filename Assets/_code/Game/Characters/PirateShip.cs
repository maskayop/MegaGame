using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class PirateShip : Warship
    {
        [SerializeField] short distanceToHomeToChange = 5;
        [SerializeField] short distanceForPossibleTarget = 200;
        //[SerializeField] float distanceToPointToAppear = 20;

        Transform homePosition;
        public Transform HomePosition { get { return homePosition; } set { homePosition = value; } }

        bool isGoingHome = false;

        List<BaseCharacter> allPossibleTargets = new List<BaseCharacter>();

        TraderShip targetShip;

        ObjectsManager objectsManager;

        protected override void OnInit()
        {
            base.OnInit();

            objectsManager = ObjectsManager.Instance;

            FindTarget();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (targetSettlement && targetSettlement.owner == Owner.neutral)
            {
                targetSettlement = null;
                targetShip = null;
                destinationPosition = homePosition;
                isGoingHome = true;
            }

            if (targetSettlement || targetShip)
                return;

            if (isGoingHome)
            {
                if (Vector3.Distance(transform.position, homePosition.position) >= distanceToHomeToChange)
                    return;
                else
                {
                    targetSettlement = null;
                    targetShip = null;

                    FindTarget();
                }
            }
            else
            {
                if (!targetShip && !targetSettlement)
                    FindTarget();
            }

            if (!targetShip && !targetSettlement)
                Kill();
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
            isGoingHome = false;

            allPossibleTargets.Clear();

            for (int i = 0; i < objectsManager.allCharacters.Count; i++)
            {
                TraderShip trader = objectsManager.allCharacters[i].GetComponent<TraderShip>();

                if (trader)
                    if (Vector3.Distance(homePosition.position, trader.transform.position) <= distanceForPossibleTarget)
                        allPossibleTargets.Add(trader);
            }

            for (int i = 0; i < gameController.allVillages.Count; i++)
            {
                Village village = gameController.allVillages[i].GetComponent<Village>();

                if (village)
                    if (village.owner != Owner.neutral)
                        if (Vector3.Distance(homePosition.position, village.transform.position) <= distanceForPossibleTarget)
                            allPossibleTargets.Add(village);
            }

            if (allPossibleTargets.Count == 0)
                return;

            short r = (short)Random.Range(0, allPossibleTargets.Count);

            BaseCharacter targetCharacter = allPossibleTargets[r];

            if (targetCharacter as TraderShip)
                targetShip = (TraderShip)targetCharacter;
            else if (targetCharacter as Village)
                targetSettlement = (Village)targetCharacter;

            destinationPosition = targetCharacter.transform;
        }
    }
}
