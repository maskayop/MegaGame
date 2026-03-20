using System;
using System.Collections.Generic;
using UnityEngine;
using static MegaGame.GameController;

namespace MegaGame
{
    [Serializable]
    public class ShopItemSceneObject
    {
        public string name;
        public Data_Item itemData;
        public GameObject itemGameObject;
    }

    [Serializable]
    public class ShopPremiumItemSceneObject
    {
        public string name;
        public Data_Item itemData;
        public GameObject itemContainer;
        public GameObject lockedItemGameObject;
        public GameObject openedItemGameObject;
        public GameObject lockGameObject;
    }

    public class AdditionalSceneObjects : MonoBehaviour
    {
        public static AdditionalSceneObjects Instance;

        [Header("Start Game")]
        [SerializeField] ModelButton startGameModelButton;
        [SerializeField] float offsetY = 0;

        [Header("End Game")]
        [SerializeField] GameObject victoryPanel;
        public AdditionalCamera victoryAdditionalCamera;

        [SerializeField] GameObject defeatPanel;
        public AdditionalCamera defeatAdditionalCamera;

        [Header("Shop")]
        [SerializeField] GameObject shopPanel;

        [SerializeField] GameObject gameModelsPreview;
        public AdditionalCamera shopAdditionalCamera;
        [SerializeField] List<ShopItemSceneObject> shopItemSceneObjects = new List<ShopItemSceneObject>();

        [Space(10)]
        [SerializeField] GameObject premiumItemsPreview;
        public AdditionalCamera shopPremiumAdditionalCamera;
        [SerializeField] List<ShopPremiumItemSceneObject> shopPremiumItemSceneObjects = new List<ShopPremiumItemSceneObject>();

        GameController gameController;

        bool shopPanelIsOpen = false;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create AdditionalSceneObjects");
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
            if (gameController.gameState == GameState.menu)
                startGameModelButton.gameObject.SetActive(false);

            if (gameController.CampaignIsEnded)
                return;

            if (gameController.gameState != GameState.battle)
                return;
        }

        public void Init()
        {
            gameController = GameController.Instance;

            HideAllPanels();
        }

        public void ShowVictoryPanel(bool state)
        {
            victoryPanel.SetActive(state);
        }

        public void ShowDefeatPanel(bool state)
        {
            defeatPanel.SetActive(state);
        }

        public void ShowShopPanel()
        {
            shopPanel.SetActive(true);
            shopPanelIsOpen = true;
        }

        public void HideShopPanel()
        {
            shopPanel.SetActive(false);
            shopPanelIsOpen = false;
            HideAllShopItemsObjects();
        }

        public void HideAllPanels()
        {
            ShowVictoryPanel(false);
            ShowDefeatPanel(false);
            HideShopPanel();
        }

        public void ShowStartGameModelButton(bool state)
        {
            startGameModelButton.gameObject.SetActive(state);
        }

        public void PlaceStartGameModelButtonBetweenPorts()
        {
            if (!startGameModelButton)
                return;

            startGameModelButton.transform.position = gameController.CalculatePositionBetweenPorts();
            startGameModelButton.transform.position += new Vector3(0, offsetY, 0);
        }

        public void ShowShopItem(Data_Item item)
        {
            HideAllShopItemsObjects();

            if (!shopPanelIsOpen)
                ShowShopPanel();

            for (int i = 0; i < shopItemSceneObjects.Count; i++)
                if (item == shopItemSceneObjects[i].itemData)
                {
                    shopItemSceneObjects[i].itemGameObject.SetActive(true);
                    break;
                }
        }

        void HideAllShopItemsObjects()
        {
            for (int i = 0; i < shopItemSceneObjects.Count; i++)
                shopItemSceneObjects[i].itemGameObject.SetActive(false);
        }
    }
}
