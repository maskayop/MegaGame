using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class NeutralAI : MonoBehaviour
    {
        [SerializeField] int daysForBuildingDecision = 100;

        List<Port> allNeutralPorts = new List<Port>();
        List<Port> portsWithoutFort = new List<Port>();

        int currentDecisionDay = 0;

        GameController gameController;
        GlobalTimeController globalTime;

        int currentDay = 0;

        void Start()
        {
            gameController = GameController.Instance;
            globalTime = GlobalTimeController.Instance;

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
    }
}
