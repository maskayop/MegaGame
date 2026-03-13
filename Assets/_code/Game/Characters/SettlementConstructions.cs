using MegaGame.UI;
using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class SettlementConstructions : MonoBehaviour
    {
        public bool fortIsBuilt = false;
        public bool traderIsBuilt = false;

        [Header("Fort Modificators")]
        public float additionalDamage = 1f;
        public float additionalHealth = 10;
        public float additionalHealthRegeneration = 1f;

        BaseSettlement settlement;
        public BaseSettlement Settlement { get { return settlement; } set { settlement = value; } }

        List<Island> traderTargets = new List<Island>();

        ResourcesController resourcesController;
        GameController gameController;
        GlobalTimeController globalTime;
        GameplayObjectsBuilder gameplayObjectsBuilder;

        int currentDay = 0;

        TraderShip traderShip;

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (!gameController)
                return;

            if (gameController.CampaignIsEnded)
                return;

            if (gameController.gameState != GameController.GameState.battle)
                return;

            if (globalTime.currentDay != currentDay)
            {
                TryBuildTraderShip();
                currentDay = globalTime.currentDay;
            }
        }

        public void Init()
        {
            resourcesController = ResourcesController.Instance;
            gameController = GameController.Instance;
            globalTime = GlobalTimeController.Instance;
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;

            settlement = GetComponent<BaseSettlement>();
        }

        public void TryBuildFort()
        {
            if (!settlement)
                return;

            if (fortIsBuilt)
                return;

            if (settlement.owner == BaseCharacter.Owner.player)
            {
                if (settlement as Port)
                {
                    if (settlement.GetComponent<Port>().isBigPort)
                    {
                        if (Strint.Subtraction(resourcesController.PlayerMoney, gameplayObjectsBuilder.GetSettlementBuildingCost(3)) < 0)
                            return;
                    }
                    else
                    {
                        if (Strint.Subtraction(resourcesController.PlayerMoney, gameplayObjectsBuilder.GetSettlementBuildingCost(2)) < 0)
                            return;
                    }
                }
            }
            else if (settlement.owner == BaseCharacter.Owner.enemy)
            {
                if (settlement as Port)
                {
                    if (settlement.GetComponent<Port>().isBigPort)
                    {
                        if (Strint.Subtraction(resourcesController.EnemyMoney, gameplayObjectsBuilder.GetSettlementBuildingCost(3)) < 0)
                            return;
                    }
                    else
                    {
                        if (Strint.Subtraction(resourcesController.EnemyMoney, gameplayObjectsBuilder.GetSettlementBuildingCost(2)) < 0)
                            return;
                    }
                }
            }

            fortIsBuilt = true;

            settlement.UpdateCharacteristics();
            settlement.Island.UpdateIslandState();

            if (settlement.owner == BaseCharacter.Owner.player)
            {
                if (settlement as Port)
                {
                    if (settlement.GetComponent<Port>().isBigPort)
                        resourcesController.RemoveMoneyFromPlayer(GetSettlementBuildingCost(3));
                    else
                        resourcesController.RemoveMoneyFromPlayer(GetSettlementBuildingCost(2));
                }
            }
            else if (settlement.owner == BaseCharacter.Owner.enemy)
            {
                if (settlement as Port)
                {
                    if (settlement.GetComponent<Port>().isBigPort)
                        resourcesController.RemoveMoneyFromEnemy(GetSettlementBuildingCost(3));
                    else
                        resourcesController.RemoveMoneyFromEnemy(GetSettlementBuildingCost(2));
                }
            }

            UIMainCanvas.Instance.SpawnFortConstructionMessage(settlement as Port);
        }

        public void TryBuildTrader()
        {
            if (!settlement)
                return;

            if (traderIsBuilt)
                return;

            if (settlement.owner == BaseCharacter.Owner.player)
            {
                if (settlement as Port)
                {
                    if (Strint.Subtraction(resourcesController.PlayerMoney, gameplayObjectsBuilder.GetSettlementBuildingCost(1)) < 0)
                        return;
                }
            }
            else if (settlement.owner == BaseCharacter.Owner.enemy)
            {
                if (settlement as Port)
                {
                    if (Strint.Subtraction(resourcesController.EnemyMoney, gameplayObjectsBuilder.GetSettlementBuildingCost(1)) < 0)
                        return;
                }
            }

            traderIsBuilt = true;

            settlement.UpdateCharacteristics();
            settlement.Island.UpdateIslandState();

            if (settlement.owner == BaseCharacter.Owner.player)
                resourcesController.RemoveMoneyFromPlayer(GetSettlementBuildingCost(1));
            else if (settlement.owner == BaseCharacter.Owner.enemy)
                resourcesController.RemoveMoneyFromEnemy(GetSettlementBuildingCost(1));

            UIMainCanvas.Instance.SpawnTraderConstructionMessage(settlement as Port);
        }

        void TryBuildTraderShip()
        {
            if (!settlement)
                return;

            if (!traderIsBuilt)
                return;

            if (traderShip)
                return;

            UpdateTradeTargets();

            if (traderTargets.Count <= 1)
                return;

            if (settlement.owner == BaseCharacter.Owner.player)
                gameplayObjectsBuilder.TryCreatePlayerTraderShip(settlement.transform, out traderShip);
            else if (settlement.owner == BaseCharacter.Owner.enemy)
                gameplayObjectsBuilder.TryCreateEnemyTraderShip(settlement.transform, out traderShip);

            traderShip.HomeTradeCompany = this;
        }

        void UpdateTradeTargets()
        {
            traderTargets.Clear();
            traderTargets.Add(settlement.Island);

            for (int i = 0; i < settlement.Island.possibleTargets.Count; i++)
            {
                if (settlement.Island.possibleTargets[i].owner == settlement.owner)
                    traderTargets.Add(settlement.Island.possibleTargets[i]);
            }
        }

        public Island GetRandomTradeTarget(BaseSettlement currentSettlement)
        {
            UpdateTradeTargets();

            int r = Random.Range(0, traderTargets.Count);

            if (traderTargets[r].settlement == currentSettlement)
            {
                if (r + 1 < traderTargets.Count)
                    return traderTargets[r + 1];
                else
                    r = 0;
            }

            return traderTargets[r];
        }

        public int GetSettlementBuildingCost(int id)
        {
            if (id == 1)
                return Strint.GetInt(gameplayObjectsBuilder.GetSettlementBuildingCost(1));
            else if (id == 2)
                return Strint.GetInt(gameplayObjectsBuilder.GetSettlementBuildingCost(2));
            else if (id == 3)
                return Strint.GetInt(gameplayObjectsBuilder.GetSettlementBuildingCost(3));
            else
                return 0;
        }
    }
}
