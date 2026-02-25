using MegaGame.UI;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class SettlementConstructions : MonoBehaviour
    {
        public bool fortIsBuilt = false;
        public bool tradeIsBuilt = false;

        [Header("Fort Modificators")]
        public float additionalHealth = 10;
        public float additionalDamage = 1f;

        [Header("Constructions Prices")]
        public short fortBuildingCost = 1000;
        public short tradeBuildingCost = 500;

        string fortCost;
        public string FortCost { get { return fortCost; } }

        string tradeCost;
        public string TradeCost { get { return tradeCost; } }

        BaseSettlement settlement;
        public BaseSettlement Settlement { get { return settlement; } set { settlement = value; } }

        ResourcesController resourcesController;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            resourcesController = ResourcesController.Instance;

            settlement = GetComponent<BaseSettlement>();

            fortCost = Strint.GetString(fortBuildingCost);
            tradeCost = Strint.GetString(tradeBuildingCost);
        }

        public void TryBuildFort()
        {
            if (fortIsBuilt)
                return;

            if (Strint.Subtraction(resourcesController.PlayerMoney, fortCost) < 0)
                return;

            fortIsBuilt = true;

            resourcesController.RemoveMoneyFromPlayer(fortBuildingCost);
            settlement.UpdateCharacteristics();
        }

        public void TryBuildTrade()
        {
            if (tradeIsBuilt)
                return;

            if (Strint.Subtraction(resourcesController.PlayerMoney, tradeCost) < 0)
                return;

            tradeIsBuilt = true;

            resourcesController.RemoveMoneyFromPlayer(tradeBuildingCost);
            settlement.UpdateCharacteristics();
        }
    }
}
