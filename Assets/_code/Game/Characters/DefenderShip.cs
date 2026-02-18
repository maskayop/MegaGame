using UnityEngine;

namespace MegaGame
{
    public class DefenderShip : Warship
    {
        [SerializeField] float distanceToPointToChange = 1;

        [Header("Info")]
        short currentDefendingPoint = 0;
        float distance;
        Transform currentTarget;

        protected override void OnUpdateTargetSettlementState()
        {
            if (!currentTarget)
                distance = Vector3.Distance(transform.position, targetSettlement.transform.position);
            else
                distance = Vector3.Distance(transform.position, targetSettlement.Island.defendingPoints[currentDefendingPoint].position);

            if (distance < distanceToPointToChange)
            {
                currentDefendingPoint++;

                if (currentDefendingPoint >= targetSettlement.Island.defendingPoints.Count)
                    currentDefendingPoint = 0;

                currentTarget = targetSettlement.Island.defendingPoints[currentDefendingPoint];
                destinationPosition = currentTarget;
            }
        }
    }
}
