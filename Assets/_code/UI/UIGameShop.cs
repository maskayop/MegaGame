using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    [Serializable]
    public class UIShopItem
    {
        public string name;
        public Data_Item itemData;
        public GameObject itemGameObject;
    }

    public class UIGameShop : MonoBehaviour
    {
        public static UIGameShop Instance { get; private set; }

        public bool isOpen = false;

        [SerializeField] GameObject window;

        [Header("Name and Description")]
        [SerializeField] TextMeshProUGUI itemNameText;
        [SerializeField] TextMeshProUGUI itemDescriptionText;

        [Header("Characteristics")]
        [SerializeField] GameObject damageContainer;
        [SerializeField] TextMeshProUGUI damageText;

        [SerializeField] GameObject attackContainer;
        [SerializeField] TextMeshProUGUI attackSpeedText;

        [SerializeField] GameObject movementContainer;
        [SerializeField] TextMeshProUGUI movementSpeedText;

        [SerializeField] GameObject healthContainer;
        [SerializeField] TextMeshProUGUI healthText;

        [SerializeField] GameObject regenerationContainer;
        [SerializeField] TextMeshProUGUI regenerationText;

        [SerializeField] GameObject maintenanceContainer;
        [SerializeField] TextMeshProUGUI maintenanceText;

        [SerializeField] GameObject revenueContainer;
        [SerializeField] TextMeshProUGUI revenueText;

        [SerializeField] GameObject buildingPriceContainer;
        [SerializeField] TextMeshProUGUI buildingPriceText;

        [Header("Items")]
        [SerializeField] List<Data_Item> shopItemsData = new List<Data_Item>();

        GameDataSaver gameDataSaver;
        CameraController cameraController;
        AdditionalSceneObjects additionalSceneObjects;
        GameplayObjectsBuilder gameplayObjectsBuilder;

        Data_Item currentItemData;
        int currentItemId = 0;
        BaseCharacter currentCharacter;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIGameShop");
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
            if (!isOpen)
                return;
        }

        public void Init()
        {
            gameDataSaver = GameDataSaver.Instance;
            cameraController = CameraController.Instance;
            additionalSceneObjects = AdditionalSceneObjects.Instance;
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;

            if (shopItemsData.Count > 0)
            {
                currentItemData = shopItemsData[0];
                currentItemId = 0;
            }

            Close();
        }

        public void Open()
        {
            isOpen = true;
            window.SetActive(true);
            cameraController.Freeze(true);

            ShowCurrentItem();
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);
            cameraController.Freeze(false);

            additionalSceneObjects.HideShopPanel();
        }

        void ShowCurrentItem()
        {
            currentItemData = shopItemsData[currentItemId];
            additionalSceneObjects.ShowShopItem(currentItemData);
            currentCharacter = currentItemData.prefab.GetComponent<BaseCharacter>();

            UpdateItemTexts();
        }

        public void ShowPrevNextItem(bool next)
        {
            if (next)
            {
                currentItemId++;

                if (currentItemId >= shopItemsData.Count)
                    currentItemId = 0;
            }
            else
            {
                currentItemId--;

                if (currentItemId < 0)
                    currentItemId = shopItemsData.Count - 1;
            }

            ShowCurrentItem();
        }

        void UpdateItemTexts()
        {
            itemNameText.text = currentItemData.itemName.GetLocalizedString();
            itemDescriptionText.text = currentItemData.itemDescription.GetLocalizedString();

            UpdateCharacteristics();
        }

        void UpdateCharacteristics()
        {
            if (!currentCharacter)
                return;

            damageText.text = currentCharacter.damage.ToString();
            attackSpeedText.text = Mathf.FloorToInt(60 / currentCharacter.attackDelay).ToString();

            if (currentCharacter as Warship)
            {
                movementSpeedText.text = currentCharacter.GetComponent<Warship>().speed.ToString();
                revenueContainer.SetActive(false);
            }
            else if (currentCharacter as BaseSettlement)
            {
                revenueContainer.SetActive(true);
                revenueText.text = currentCharacter.revenue.ToString();
            }

            healthText.text = currentCharacter.health.ToString();
            regenerationText.text = currentCharacter.healthRegeneration.ToString();
            maintenanceText.text = currentCharacter.maintenance.ToString();

            buildingPriceText.text = gameplayObjectsBuilder.GetShipBuildingCost(currentItemData.priceId).ToString();
        }
    }
}
