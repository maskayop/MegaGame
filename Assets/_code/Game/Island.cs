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
        public List<Island> possibleEnemyStartIsland = new List<Island>();
        public List<Island> possibleTargets = new List<Island>();

        [Header("Defend")]
        public List<Transform> defendingPoints = new List<Transform>();

        [Header("Exploring")]
        [SerializeField] GameObject exploringCircle;
        public PirateLair pirateLair;

        GameController gameController;

        DefenderShip defenderShip;
        public DefenderShip DefenderShip { get { return defenderShip; } set { defenderShip = value; } }

        bool isDefenderShip;

        Collider coll;

        void Awake()
        {
            coll = GetComponent<Collider>();

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

            if (settlement)
            {
                if (settlement as Port)
                    gameController.allPorts.Add((Port)settlement);
                else if (settlement as Village)
                    gameController.allVillages.Add((Village)settlement);
                else if (settlement as Fortress)
                    gameController.allFortresses.Add((Fortress)settlement);
            }

            UpdateIslandState();

            isDefenderShip = true;

            if (exploringCircle)
                EnableExploringCircle(false);

            if (pirateLair)
                ShowPirateLair(false);
        }

        void SetThisIslandToSettlements()
        {
            if (!settlement && !pirateLair)
                return;

            if (settlement)
                settlement.Island = this;

            if (pirateLair)
            {
                pirateLair.Island = this;
                UpdatePirateLairState();
            }
        }

        public void UpdateIslandState()
        {
            if (nameWidget)
            {
                nameWidget.SetText(islandData.islandName);

                if (settlement && settlement.GetSettlementConstructions())
                {
                    nameWidget.SetDefenceFort(settlement.GetSettlementConstructions().fortIsBuilt);
                    nameWidget.SetTrade(settlement.GetSettlementConstructions().traderIsBuilt);
                }

                nameWidget.SetColor(owner);
            }

            if (!settlement)
                return;

            UpdateSettlementState();

            if (settlement as Port)
            {
                Port p = (Port)settlement;

                if (gameController.playerOpposingPorts.protagonPort == p ||
                    gameController.playerOpposingPorts.antagonPort == p ||
                    gameController.enemyOpposingPorts.protagonPort == p ||
                    gameController.enemyOpposingPorts.antagonPort == p)
                    p.SetVisualAsTarget(true, owner);
                else
                    p.SetVisualAsTarget(false, owner);
            }
        }

        void UpdateSettlementState()
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
            settlement.UpdateCharacteristics();
        }

        void UpdatePirateLairState()
        {
            pirateLair.gameObject.name = islandData.islandName.GetLocalizedString() + " - Pirate Lair";
        }

        public void EnableExploringCircle(bool state)
        {
            exploringCircle.SetActive(state);
            coll.enabled = state;
        }

        public void ShowPirateLair(bool state)
        {
            pirateLair.ShowPirateLair(state);
        }
    }
}
