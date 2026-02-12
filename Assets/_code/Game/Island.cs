using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class Island : MonoBehaviour
    {
        public BaseCharacter.Owner owner;
        public Data_Island islandData;

        [SerializeField] NameWidget nameWidget;

        public List<Port> ports = new List<Port>();
        public List<Village> villages = new List<Village>();
        public List<Fortress> fortresses = new List<Fortress>();

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

            for (int i = 0; i < ports.Count; i++)
                gameController.allPorts.Add(ports[i]);

            for (int i = 0; i < villages.Count; i++)
                gameController.allVillages.Add(villages[i]);

            for (int i = 0; i < fortresses.Count; i++)
                gameController.allFortresses.Add(fortresses[i]);

            UpdateIslandState();
        }

        void SetThisIslandToSettlements()
        {
            for (int i = 0; i < ports.Count; i++)
                ports[i].Island = this;

            for (int i = 0; i < villages.Count; i++)
                villages[i].Island = this;

            for (int i = 0; i < fortresses.Count; i++)
                fortresses[i].Island = this;
        }

        public void UpdateIslandState()
        {
            if (nameWidget)
            {
                nameWidget.SetText(islandData.islandName.GetLocalizedString());
                nameWidget.SetColor(owner);
            }

            for (int i = 0; i < ports.Count; i++)
            {
                UpdateSettlementState(ports[i], i);
                ports[i].SetVisualAsTarget(false, owner);
            }

            for (int i = 0; i < villages.Count; i++)
                UpdateSettlementState(villages[i], i);

            for (int i = 0; i < fortresses.Count; i++)
                UpdateSettlementState(fortresses[i], i);
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
