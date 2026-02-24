using System.Collections.Generic;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIShipsSelection : MonoBehaviour
    {
        public static UIShipsSelection Instance { get; private set; }

        [SerializeField] List<UIShipSelectionButton> shipSelectionButtons = new List<UIShipSelectionButton>();

        GameplayObjectsBuilder gameplayObjectsBuilder;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIShipsSelection");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;

            shipSelectionButtons[3].gameObject.SetActive(false);

            SetMaxBuildingShip(2);
        }

        public void SetMaxBuildingShip(short id)
        {
            gameplayObjectsBuilder.SetMaxBuildingShip(id);

            for (int i = 0; i < shipSelectionButtons.Count; i++)
            {
                if (id >= i)
                    shipSelectionButtons[i].Select(true);
                else
                    shipSelectionButtons[i].Select(false);
            }
        }
    }
}
