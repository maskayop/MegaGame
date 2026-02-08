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

            UpdateIslandState();
        }

        void SetThisIslandToSettlements()
        {
            for (int i = 0; i < ports.Count; i++)
                ports[i].Island = this;

            for (int i = 0; i < villages.Count; i++)
                villages[i].Island = this;
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
                ports[i].owner = owner;
                ports[i].SetVisual();
                ports[i].SetVisualAsTarget(false, owner);
                ports[i].gameObject.name = islandData.islandName.GetLocalizedString() + " - Port " + i.ToString();
            }
        }
    }
}
