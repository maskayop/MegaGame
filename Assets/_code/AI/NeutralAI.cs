using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class NeutralAI : MonoBehaviour
    {
        [Header("Building")]
        [SerializeField] short daysForBuildingDecision = 100;

        [Header("Pirates")]
        [SerializeField] short maxPirateShips = 10;
        [SerializeField] short portsCountSubtractor = 10;

        List<Port> allNeutralPorts = new List<Port>();
        List<Port> portsWithoutFort = new List<Port>();

        short currentDecisionDay = 0;

        GameController gameController;
        GlobalTimeController globalTime;
        ObjectsManager objectsManager;
        GameplayObjectsBuilder gameplayObjectsBuilder;

        int currentDay = 0;

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
            if (gameController.gameState != GameController.GameState.battle)
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
            short r = (short)Random.Range(0, portsWithoutFort.Count);
            portsWithoutFort[r].GetSettlementConstructions().TryBuildFort();
        }

        void TryBuildPirateShip()
        {
            if (gameController.PlayerVillagesCount == 0 && gameController.EnemyVillagesCount == 0)
                return;

            if (gameController.PlayerPortsCount + gameController.EnemyPortsCount - portsCountSubtractor >= 0)
                if (objectsManager.pirateShips.Count < maxPirateShips)
                    gameplayObjectsBuilder.TryCreatePirateShip(GetPirateShipHomePosition());
        }

        Transform GetPirateShipHomePosition()
        {
            UpdateAllNeutralPorts();

            short r = (short)Random.Range(0, allNeutralPorts.Count);
            return allNeutralPorts[r].transform;
        }
    }
}
