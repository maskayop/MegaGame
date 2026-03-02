using System.Collections.Generic;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIShipsSelection : MonoBehaviour
    {
        public static UIShipsSelection Instance { get; private set; }

        [SerializeField] GameObject buttonsPanel;
        [SerializeField] List<UIShipSelectionButton> shipSelectionButtons = new List<UIShipSelectionButton>();

        GameController gameController;
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

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                buttonsPanel.SetActive(false);
            else
                buttonsPanel.SetActive(true);
        }

        public void Init()
        {
            gameController = GameController.Instance;
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;

            //shipSelectionButtons[3].gameObject.SetActive(false);

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
