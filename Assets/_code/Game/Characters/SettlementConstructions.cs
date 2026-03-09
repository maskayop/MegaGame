using MegaGame.UI;
using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class SettlementConstructions : MonoBehaviour
    {
        public bool fortIsBuilt = false;
        public bool tradeIsBuilt = false;

        [Header("Fort Modificators")]
        public float additionalDamage = 1f;
        public float additionalHealth = 10;
        public float additionalHealthRegeneration = 1f;

        [Header("Constructions Prices")]
        public short fortBuildingCost = 1000;
        public short tradeBuildingCost = 500;

        string fortCost;
        public string FortCost { get { return fortCost; } }

        string tradeCost;
        public string TradeCost { get { return tradeCost; } }

        BaseSettlement settlement;
        public BaseSettlement Settlement { get { return settlement; } set { settlement = value; } }

        public List<Island> tradeTargets = new List<Island>();

        ResourcesController resourcesController;
        GameController gameController;
        GlobalTimeController globalTime;
        GameplayObjectsBuilder gameplayObjectsBuilder;

        int currentDay = 0;

        public TraderShip traderShip;

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

            fortCost = Strint.GetString(fortBuildingCost);
            tradeCost = Strint.GetString(tradeBuildingCost);
        }

        public void TryBuildFort()
        {
            if (!settlement)
                return;

            if (fortIsBuilt)
                return;

            if (settlement.owner == BaseCharacter.Owner.player)
            {
                if (Strint.Subtraction(resourcesController.PlayerMoney, fortCost) < 0)
                    return;
            }

            fortIsBuilt = true;

            settlement.UpdateCharacteristics();
            settlement.Island.UpdateIslandState();

            if (settlement.owner == BaseCharacter.Owner.player)
                resourcesController.RemoveMoneyFromPlayer(fortBuildingCost);
            else if (settlement.owner == BaseCharacter.Owner.enemy)
                resourcesController.RemoveMoneyFromEnemy(fortBuildingCost);

            UIMainCanvas.Instance.SpawnFortConstructionMessage(settlement as Port);
        }

        public void TryBuildTrade()
        {
            if (!settlement)
                return;

            if (tradeIsBuilt)
                return;

            if (settlement.owner == BaseCharacter.Owner.player)
            {
                if (Strint.Subtraction(resourcesController.PlayerMoney, tradeCost) < 0)
                    return;
            }

            tradeIsBuilt = true;

            settlement.UpdateCharacteristics();
            settlement.Island.UpdateIslandState();

            if (settlement.owner == BaseCharacter.Owner.player)
                resourcesController.RemoveMoneyFromPlayer(tradeBuildingCost);
            else if (settlement.owner == BaseCharacter.Owner.enemy)
                resourcesController.RemoveMoneyFromEnemy(tradeBuildingCost);

            UIMainCanvas.Instance.SpawnTraderConstructionMessage(settlement as Port);
        }

        void TryBuildTraderShip()
        {
            if (!settlement)
                return;

            if (!tradeIsBuilt)
                return;

            if (traderShip)
                return;

            UpdateTradeTargets();

            if (tradeTargets.Count <= 1)
                return;

            if (settlement.owner == BaseCharacter.Owner.player)
                gameplayObjectsBuilder.TryCreatePlayerTraderShip(settlement.transform, out traderShip);
            else if (settlement.owner == BaseCharacter.Owner.enemy)
                gameplayObjectsBuilder.TryCreateEnemyTraderShip(settlement.transform, out traderShip);

            traderShip.HomeTradeCompany = this;
        }

        void UpdateTradeTargets()
        {
            tradeTargets.Clear();
            tradeTargets.Add(settlement.Island);

            for (int i = 0; i < settlement.Island.possibleTargets.Count; i++)
            {
                if (settlement.Island.possibleTargets[i].owner == settlement.owner)
                    tradeTargets.Add(settlement.Island.possibleTargets[i]);
            }
        }

        public Island GetRandomTradeTarget(BaseSettlement currentSettlement)
        {
            UpdateTradeTargets();

            short r = (short)Random.Range(0, tradeTargets.Count);

            if (tradeTargets[r].settlement == currentSettlement)
            {
                if (r + 1 < tradeTargets.Count)
                    return tradeTargets[r + 1];
                else
                    r = 0;
            }

            return tradeTargets[r];
        }
    }
}
