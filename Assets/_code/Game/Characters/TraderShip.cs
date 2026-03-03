using UnityEngine;

namespace MegaGame
{
    public class TraderShip : Warship
    {
        SettlementConstructions homeTradeCompany;
        public SettlementConstructions HomeTradeCompany { get { return homeTradeCompany; } set { homeTradeCompany = value; } }

        [SerializeField] float distanceToPointToChange = 1;

        [Header("Info")]
        public float distance;

        public BaseSettlement currentTarget;

        ResourcesController resourcesController;

        short profit;

        protected override void OnInit()
        {
            resourcesController = ResourcesController.Instance;

            UpdateCurrentTarget();
        }

        protected override void OnUpdateTargetSettlementState()
        {
            distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance <= distanceToPointToChange)
            {
                UpdateCurrentTarget();

                profit = resourcesController.GetRandomTraderProfit();

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
