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
        string tradeCost;

        BaseSettlement settlement;
        public BaseSettlement Settlement { get { return settlement; } set { settlement = value; } }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            fortCost = Strint.GetString(fortBuildingCost);
            tradeCost = Strint.GetString(tradeBuildingCost);
        }
    }
}
