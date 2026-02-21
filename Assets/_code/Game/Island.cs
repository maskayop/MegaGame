using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class Island : MonoBehaviour
    {
        public BaseCharacter.Owner owner;
        public Data_Island islandData;

        public BaseSettlement settlement;

        [Header("Widgets")]
        [SerializeField] NameWidget nameWidget;

        [Header("Battle")]
        public bool isStartIsland = false;
        public List<Island> possibleTargets = new List<Island>();

        [Header("Defend")]
        public List<Transform> defendingPoints = new List<Transform>();

        GameController gameController;

        DefenderShip defenderShip;
        public DefenderShip DefenderShip { get { return defenderShip; } set { defenderShip = value; } }

        bool isDefenderShip;

        void Awake()
        {
            SetThisIslandToSettlements();
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                return;

            if (isDefenderShip == defenderShip)
                return;

            nameWidget.SetDefenderShip(defenderShip);

            if (defenderShip)
                nameWidget.SetDefenderShipColor(defenderShip.owner);

            isDefenderShip = defenderShip;
        }

        void Init()
        {
            if (GameController.Instance)
                gameController = GameController.Instance;

            if (!gameController)
                return;

            gameController.allIslands.Add(this);

            if (settlement as Port)
                gameController.allPorts.Add((Port)settlement);
            else if (settlement as Village)
                gameController.allVillages.Add((Village)settlement);
            else if (settlement as Fortress)
                gameController.allFortresses.Add((Fortress)settlement);

            UpdateIslandState();

            isDefenderShip = true;
        }

        void SetThisIslandToSettlements()
        {
            if (!settlement)
                return;

            settlement.Island = this;
        }

        public void UpdateIslandState()
        {
            if (nameWidget)
            {
                nameWidget.SetText(islandData.islandName.GetLocalizedString());
                nameWidget.SetColor(owner);
            }

            if (!settlement)
                return;

            UpdateSettlementState(settlement);

            if (settlement as Port)
                settlement.GetComponent<Port>().SetVisualAsTarget(false, owner);
        }

        void UpdateSettlementState(BaseSettlement INsettlement)
        {
            INsettlement.owner = owner;
            INsettlement.SetVisual();

            string settlementType = "";

            if (INsettlement as Port)
                settlementType = " Port ";
            else if (INsettlement as Village)
                settlementType = " Village ";
            else if (INsettlement as Fortress)
                settlementType = " Fortress ";

            INsettlement.gameObject.name = islandData.islandName.GetLocalizedString() + settlementType.ToString();
        }
    }
}
