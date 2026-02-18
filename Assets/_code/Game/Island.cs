using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class Island : MonoBehaviour
    {
        public BaseCharacter.Owner owner;
        public Data_Island islandData;

        [SerializeField] NameWidget nameWidget;

        public List<BaseSettlement> settlements = new List<BaseSettlement>();

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
            nameWidget.SetColor(owner);

            isDefenderShip = defenderShip;
        }

        void Init()
        {
            if (GameController.Instance)
                gameController = GameController.Instance;

            if (!gameController)
                return;

            gameController.allIslands.Add(this);

            for (int i = 0; i < settlements.Count; i++)
            {
                if (settlements[i] as Port)
                    gameController.allPorts.Add((Port)settlements[i]);
                else if (settlements[i] as Village)
                    gameController.allVillages.Add((Village)settlements[i]);
                else if (settlements[i] as Fortress)
                    gameController.allFortresses.Add((Fortress)settlements[i]);
            }

            UpdateIslandState();

            isDefenderShip = true;
        }

        void SetThisIslandToSettlements()
        {
            for (int i = 0; i < settlements.Count; i++)
                settlements[i].Island = this;
        }

        public void UpdateIslandState()
        {
            if (nameWidget)
            {
                nameWidget.SetText(islandData.islandName.GetLocalizedString());
                nameWidget.SetColor(owner);
            }

            for (int i = 0; i < settlements.Count; i++)
            {
                UpdateSettlementState(settlements[i]);

                if (settlements[i] as Port)
                    settlements[i].GetComponent<Port>().SetVisualAsTarget(false, owner);
            }
        }

        void UpdateSettlementState(BaseSettlement settlement)
        {
            settlement.owner = owner;
            settlement.SetVisual();

            string settlementType = "";

            if (settlement as Port)
                settlementType = " Port ";
            else if (settlement as Village)
                settlementType = " Village ";
            else if (settlement as Fortress)
                settlementType = " Fortress ";

            settlement.gameObject.name = islandData.islandName.GetLocalizedString() + settlementType.ToString();
        }
    }
}
