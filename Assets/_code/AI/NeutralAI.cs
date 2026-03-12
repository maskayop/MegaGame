using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;
using static MegaGame.GameController;

namespace MegaGame
{
    public class NeutralAI : MonoBehaviour
    {
        [Header("Building")]
        [SerializeField] short daysForBuildingDecision = 100;

        [Header("Pirates")]
        [SerializeField] short pirateIslandsCount = 5;
        [SerializeField] short maxPirateShips = 10;
        [SerializeField] short portsCountSubtractor = 10;

        List<Port> allNeutralPorts = new List<Port>();
        List<Port> portsWithoutFort = new List<Port>();

        public List<PirateLair> pirateLairs = new List<PirateLair>();

        short currentDecisionDay = 0;

        GameController gameController;
        GlobalTimeController globalTime;
        ObjectsManager objectsManager;
        GameplayObjectsBuilder gameplayObjectsBuilder;

        int currentDay = 0;

        GameState currentGameState;

        void Start()
        {
            gameController = GameController.Instance;
            globalTime = GlobalTimeController.Instance;
            objectsManager = ObjectsManager.Instance;
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;

            Init();
        }

        void Update()
        {
            if (currentGameState != gameController.gameState)
            {
                if (gameController.gameState == GameState.battle)
                    UpdatePirateIslands();
            }

            currentGameState = gameController.gameState;

            if (gameController.gameState != GameState.battle)
                return;

            if (globalTime.currentDay != currentDay)
            {
                currentDay = globalTime.currentDay;
                currentDecisionDay--;

                TryBuildPirateShip();
            }

            if (currentDecisionDay <= 0)
            {
                currentDecisionDay = daysForBuildingDecision;
                UpdateAllNeutralPorts();
                TryBuildFort();
            }
        }

        public void Init()
        {
            UpdateAllNeutralPorts();

            currentDecisionDay = daysForBuildingDecision;
        }

        void UpdateAllNeutralPorts()
        {
            allNeutralPorts.Clear();

            for (short i = 0; i < gameController.allPorts.Count; i++)
            {
                if (gameController.allPorts[i].owner == BaseCharacter.Owner.neutral)
                    allNeutralPorts.Add(gameController.allPorts[i]);
            }

            portsWithoutFort.Clear();

            for (short i = 0; i < allNeutralPorts.Count; i++)
            {
                if (allNeutralPorts[i].GetSettlementConstructions() && !allNeutralPorts[i].GetSettlementConstructions().fortIsBuilt)
                    portsWithoutFort.Add(allNeutralPorts[i]);
            }
        }

        void TryBuildFort()
        {
            if (portsWithoutFort.Count == 0)
                return;

            short r = (short)Random.Range(0, portsWithoutFort.Count);
            portsWithoutFort[r].GetSettlementConstructions().TryBuildFort();
        }

        void TryBuildPirateShip()
        {
            if (gameController.PlayerVillagesCount == 0 && gameController.EnemyVillagesCount == 0)
                return;

            short portsCountDependence = (short)(gameController.PlayerPortsCount + gameController.EnemyPortsCount - portsCountSubtractor);

            if (portsCountDependence >= 0)
                if (objectsManager.pirateShips.Count < portsCountDependence && objectsManager.pirateShips.Count < maxPirateShips)
                    gameplayObjectsBuilder.TryCreatePirateShip(GetPirateShipHomePosition());
        }

        Transform GetPirateShipHomePosition()
        {
            short r = (short)Random.Range(0, pirateLairs.Count);
            return pirateLairs[r].transform;
        }

        void UpdatePirateIslands()
        {
            pirateLairs.Clear();

            gameController.allEmptyIslands.Shuffle();

            for (int i = 0; i < gameController.allEmptyIslands.Count; i++)
            {
                gameController.allEmptyIslands[i].pirateLair.gameObject.SetActive(false);
                gameController.allEmptyIslands[i].ShowPirateLair(false);
            }

            for (int i = 0; i < pirateIslandsCount; i++)
                pirateLairs.Add(gameController.allEmptyIslands[i].pirateLair);

            for (int i = 0; i < pirateLairs.Count; i++)
            {
                pirateLairs[i].gameObject.SetActive(true);
                pirateLairs[i].Init();
            }
        }
    }
}
