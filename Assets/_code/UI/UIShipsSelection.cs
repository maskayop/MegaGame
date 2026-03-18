using System;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame.UI
{
    [Serializable]
    public class ShipSelectionItem
    {
        public string name;
        public UIShipSelectionButton shipSelectionButton;
        public Data_Item shopItemData;
    }

    public class UIShipsSelection : MonoBehaviour
    {
        public static UIShipsSelection Instance { get; private set; }

        [SerializeField] GameObject buttonsPanel;
        [SerializeField] GameObject addButton;
        [SerializeField] List<ShipSelectionItem> shipSelectionItems = new List<ShipSelectionItem>();
        [SerializeField] ShipSelectionItem defenderShipBuildingItem;

        GameController gameController;
        GameplayObjectsBuilder gameplayObjectsBuilder;
        GameShop gameShop;

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
            gameShop = GameShop.Instance;

            SetMaxBuildingShip(2);
            SetBuildDefenderShip();
            UpdateButtonsState();
        }

        public void SetMaxBuildingShip(int id)
        {
            gameplayObjectsBuilder.SetMaxBuildingShip(id);

            for (int i = 0; i < shipSelectionItems.Count; i++)
            {
                if (id >= i)
                    shipSelectionItems[i].shipSelectionButton.Select(true);
                else
                    shipSelectionItems[i].shipSelectionButton.Select(false);
            }
        }

        public void SetBuildDefenderShip()
        {
            gameplayObjectsBuilder.CanBuildDefenderShips = !gameplayObjectsBuilder.CanBuildDefenderShips;
            defenderShipBuildingItem.shipSelectionButton.Select(gameplayObjectsBuilder.CanBuildDefenderShips);
        }

        public void UpdateButtonsState()
        {
            for (int i = 0; i < shipSelectionItems.Count; i++)
            {
                if (shipSelectionItems[i].shopItemData.openGamePrice == 0 && shipSelectionItems[i].shopItemData.openRealPrice == 0)
                {
                    shipSelectionItems[i].shipSelectionButton.gameObject.SetActive(true);
                    SetMaxBuildingShip(i);
                }
                else
                {
                    if (gameShop.CheckForPurchasing(shipSelectionItems[i].shopItemData))
                    {
                        shipSelectionItems[i].shipSelectionButton.gameObject.SetActive(true);
                        SetMaxBuildingShip(i);
                    }
                    else
                        shipSelectionItems[i].shipSelectionButton.gameObject.SetActive(false);
                }
            }

            if (gameShop.CheckForPurchasing(defenderShipBuildingItem.shopItemData))
                defenderShipBuildingItem.shipSelectionButton.gameObject.SetActive(true);
            else
            {
                defenderShipBuildingItem.shipSelectionButton.gameObject.SetActive(false);
                gameplayObjectsBuilder.CanBuildDefenderShips = true;
                SetBuildDefenderShip();
            }
        }
    }
}
