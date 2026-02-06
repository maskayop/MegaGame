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

        [Header("Battle")]
        public bool isStartIsland = false;
        public List<Island> possibleTargets = new List<Island>();

        GameController gameController;

        void Awake()
        {
            SetThisIslandToPorts();
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

        void SetThisIslandToPorts()
        {
            for (int i = 0; i < ports.Count; i++)
                ports[i].island = this;
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
                ports[i].SetVisualAsTarget(false, owner);
                ports[i].gameObject.name = islandData.islandName.GetLocalizedString() + " - Port " + i.ToString();
            }
        }
    }
}
