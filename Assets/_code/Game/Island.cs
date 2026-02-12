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

        GameController gameController;

        void Awake()
        {
            SetThisIslandToSettlements();
        }

        void Start()
        {
            Init();
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
                UpdateSettlementState(settlements[i], i);

                if (settlements[i] as Port)
                    settlements[i].GetComponent<Port>().SetVisualAsTarget(false, owner);
            }
        }

        void UpdateSettlementState(BaseSettlement settlement, int id)
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

            settlement.gameObject.name = islandData.islandName.GetLocalizedString() + settlementType + id.ToString();
        }
    }
}
