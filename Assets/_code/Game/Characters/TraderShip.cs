using UnityEngine;

namespace MegaGame
{
    public class TraderShip : Warship
    {
        SettlementConstructions homeTradeCompany;
        public SettlementConstructions HomeTradeCompany { get { return homeTradeCompany; } set { homeTradeCompany = value; } }

        [SerializeField] short distanceToPointToChange = 1;

        float distance;
        short profit;

        BaseSettlement currentTarget;

        ResourcesController resourcesController;

        protected override void OnInit()
        {
            base.OnInit();

            resourcesController = ResourcesController.Instance;

            UpdateCurrentTarget();
        }

        protected override void OnUpdateTargetSettlementState()
        {
            distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance <= distanceToPointToChange)
            {
                profit = resourcesController.GetRandomTraderProfit();

                UpdateCurrentTarget();

                if (owner == Owner.player)
                    resourcesController.AddMoneyToPlayer(profit);
                else if (owner == Owner.enemy)
                    resourcesController.AddMoneyToEnemy(profit);
            }
        }

        void UpdateCurrentTarget()
        {
            currentTarget = homeTradeCompany.GetRandomTradeTarget(currentTarget).settlement;
            destinationPosition = currentTarget.transform;
            ScenePrefabsManager.Instance.SpawnTraderProfitWidget(transform.position, profit);
        }
    }
}
