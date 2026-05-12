using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vopere.Common;

namespace MegaGame.UI
{
    [Serializable]
    public class UIShopItem
    {
        public string name;
        public Data_Item itemData;
        public GameObject itemIconGameObject;
    }

    public class UIGameShop : MonoBehaviour
    {
        public static UIGameShop Instance { get; private set; }

        bool isOpen = false;
        public bool IsOpen { get { return isOpen; } }

        [SerializeField] GameObject window;

        [Header("Shop or Infopedia")]
        [SerializeField] UIShopInfoButton shopButton;
        [SerializeField] UIShopInfoButton infopediaButton;

        [Header("Name, Description, Money")]
        [SerializeField] TextMeshProUGUI itemNameText;
        [SerializeField] TextMeshProUGUI itemDescriptionText;
        [SerializeField] TextMeshProUGUI playerMoneyText;

        [Header("Open Item Panel")]
        [SerializeField] GameObject openItemButton;
        [SerializeField] GameObject openItemButtonImage;
        [SerializeField] GameObject openItemQuestionButtons;
        [SerializeField] TextMeshProUGUI itemPriceText;
        [SerializeField] Image shopGameMoneyImage;
        [SerializeField] Image shopRealMoneyImage;
        [SerializeField] GameObject itemIsAvailablePanel;
        [SerializeField] GameObject premiumItemIsAvailablePanel;

        [SerializeField] GameObject previewRawImage;
        [SerializeField] GameObject premiumPreviewRawImage;

        [Header("Characteristics")]
        [SerializeField] GameObject characteristicsPanel;

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
        [SerializeField] List<UIShopItem> shopItemsData = new List<UIShopItem>();
        [SerializeField] List<UIShopItem> infopediaItemsData = new List<UIShopItem>();

        [Header("Status Windows")]
        [SerializeField] UIShopLoadingIndicator shopLoadingIndicator;
        [SerializeField] GameObject shopBuySuccessWindow;
        [SerializeField] GameObject shopBuyFailedWindow;

        GameController gameController;
        CameraController cameraController;
        AdditionalSceneObjects additionalSceneObjects;
        GameplayObjectsBuilder gameplayObjectsBuilder;
        GameShop gameShop;
        GameDataSaver gameDataSaver;

        Data_Item currentItemData;
        int currentItemId = 0;
        int currentInfopediaItemId = 0;
        BaseCharacter currentCharacter;

        RustorePayments rustorePayments;

        bool isInitialized = false;
        bool isShopItems = true;

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

            playerMoneyText.text = ResourcesController.Instance.GetPlayerMoney().ToString();
        }

        public void Init()
        {
            gameController = GameController.Instance;
            cameraController = CameraController.Instance;
            additionalSceneObjects = AdditionalSceneObjects.Instance;
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;
            gameShop = GameShop.Instance;
            gameDataSaver = GameDataSaver.Instance;
            rustorePayments = RustorePayments.Instance;

            if (shopItemsData.Count > 0)
            {
                currentItemId = 0;
                currentItemData = shopItemsData[0].itemData;
            }

            ShowShopItems();

            Close();

            isInitialized = true;

            //---

            GameShop.OnLoading -= StartLoading;
            GameShop.OnBuyProductSuccess -= BuyProductSuccess;
            GameShop.OnBuyProductFailed -= BuyProductFailed;

            //+++

            GameShop.OnLoading += StartLoading;
            GameShop.OnBuyProductSuccess += BuyProductSuccess;
            GameShop.OnBuyProductFailed += BuyProductFailed;
        }

        public void Open()
        {
            isOpen = true;
            window.SetActive(true);
            cameraController.Freeze(true);

            rustorePayments?.CheckStoreAvailability();

            if (CanGetRustoreData())
            {
                rustorePayments?.LoadProducts();
                rustorePayments?.GetPurchases();
            }

            gameShop?.UpdateRustorePurchases();

            ShowShopItems();
            ShowCurrentItem();

            shopBuySuccessWindow.SetActive(false);
            shopBuyFailedWindow.SetActive(false);
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);
            cameraController.Freeze(false);

            additionalSceneObjects.HideShopPanel();

            ResourcesController.Instance?.TryOpenPurchasedEnemyResources();

            if (isInitialized)
                gameDataSaver.SavePremiumShopData();
        }

        void ShowCurrentItem()
        {
            if (isShopItems)
            {
                currentItemData = shopItemsData[currentItemId].itemData;
                additionalSceneObjects.ShowShopItem(currentItemData, false);
            }
            else
            {
                currentItemData = infopediaItemsData[currentInfopediaItemId].itemData;
                additionalSceneObjects.ShowShopItem(currentItemData, true);
            }

            if (currentItemData.prefab)
            {
                currentCharacter = currentItemData.prefab.GetComponent<BaseCharacter>();

                characteristicsPanel.SetActive(true);
                previewRawImage.SetActive(true);
                premiumPreviewRawImage.SetActive(false);
            }
            else
            {
                characteristicsPanel.SetActive(false);
                previewRawImage.SetActive(false);
                premiumPreviewRawImage.SetActive(true);
            }

            HideAllItemIcons();

            if (isShopItems)
                shopItemsData[currentItemId].itemIconGameObject.SetActive(true);
            else
                infopediaItemsData[currentInfopediaItemId].itemIconGameObject.SetActive(true);

            UpdateItemOpenState();
            UpdateItemTexts();
            ShowOpenItemQuestionButtons(false);
        }

        public void ShowPrevNextItem(bool next)
        {
            if (next)
            {
                if (isShopItems)
                {
                    currentItemId++;

                    if (currentItemId >= shopItemsData.Count)
                        currentItemId = 0;
                }
                else
                {
                    currentInfopediaItemId++;

                    if (currentInfopediaItemId >= infopediaItemsData.Count)
                        currentInfopediaItemId = 0;
                }
            }
            else
            {
                if (isShopItems)
                {
                    currentItemId--;

                    if (currentItemId < 0)
                        currentItemId = shopItemsData.Count - 1;
                }
                else
                {
                    currentInfopediaItemId--;

                    if (currentInfopediaItemId < 0)
                        currentInfopediaItemId = infopediaItemsData.Count - 1;
                }
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

            damageContainer.SetActive(currentItemData.showDamage);
            attackContainer.SetActive(currentItemData.showAttackSpeed);
            movementContainer.SetActive(currentItemData.showMovementSpeed);
            healthContainer.SetActive(currentItemData.showHealth);
            regenerationContainer.SetActive(currentItemData.showRegeneration);
            maintenanceContainer.SetActive(currentItemData.showMaintenance);
            revenueContainer.SetActive(currentItemData.showRevenue);
            buildingPriceContainer.SetActive(currentItemData.showBuildingPrice);

            damageText.text = currentCharacter.damage.ToString();
            attackSpeedText.text = Mathf.FloorToInt(60 / currentCharacter.attackDelay).ToString();

            if (currentCharacter as Warship)
                movementSpeedText.text = currentCharacter.GetComponent<Warship>().speed.ToString();
            else if (currentCharacter as BaseSettlement)
                revenueText.text = currentCharacter.revenue.ToString();

            healthText.text = currentCharacter.health.ToString();
            regenerationText.text = currentCharacter.healthRegeneration.ToString();
            maintenanceText.text = currentCharacter.maintenance.ToString();
            buildingPriceText.text = gameplayObjectsBuilder.GetShipBuildingCost(currentItemData.priceId).ToString();

            SettlementConstructions sc = currentCharacter.GetComponent<SettlementConstructions>();

            if (!sc)
                return;

            if (currentItemData.type == Data_Item.SettlementConstructionType.bigPortFort ||
                currentItemData.type == Data_Item.SettlementConstructionType.smallPortFort)
            {
                damageText.text = "+" + sc.additionalDamage.ToString();
                healthText.text = "+" + sc.additionalHealth.ToString();
                regenerationText.text = "+" + sc.additionalHealthRegeneration.ToString();
                maintenanceText.text = sc.fortMaintenance.ToString();

                if (currentItemData.type == Data_Item.SettlementConstructionType.bigPortFort)
                    buildingPriceText.text = Strint.GetInt(gameplayObjectsBuilder.GetSettlementBuildingCost(3)).ToString();
                else
                    buildingPriceText.text = Strint.GetInt(gameplayObjectsBuilder.GetSettlementBuildingCost(2)).ToString();
            }
            else if (currentItemData.type == Data_Item.SettlementConstructionType.trader)
                buildingPriceText.text = Strint.GetInt(gameplayObjectsBuilder.GetSettlementBuildingCost(1)).ToString();
        }

        void UpdateItemOpenState()
        {
            if (!currentItemData)
                return;

            if (!gameController)
                return;

            premiumItemIsAvailablePanel.SetActive(false);

            if (currentItemData.openGamePrice == 0 && currentItemData.openRealPrice == 0)
            {
                openItemButton.SetActive(false);

                if (isShopItems)
                    itemIsAvailablePanel.SetActive(true);
                else
                    itemIsAvailablePanel.SetActive(false);

                return;
            }

            openItemButton.SetActive(true);
            itemIsAvailablePanel.SetActive(false);

            if (!currentItemData.IsPremium() && currentItemData.openGamePrice != 0)
                ShowGameItemObjects();

            if (currentItemData.IsPremium())
                ShowPremiumItemPriceObjects();

            if (gameShop.CheckForPurchasing(currentItemData))
                ShowPurchasedItemObjects();

            if (gameController.gameState != GameController.GameState.battle)
            {
                if (currentItemData.IsPremium())
                {
#if UNITY_EDITOR
                    if (!gameShop.CheckForPurchasing(currentItemData))
                        openItemButton.SetActive(true);
#else
                    if (CanGetRustoreData())
                    {
                        if (!gameShop.CheckForPurchasing(currentItemData))
                            openItemButton.SetActive(true);
                        else
                            openItemButton.SetActive(false);
                    }
                    else
                        openItemButton.SetActive(false);
#endif
                }
                else
                    openItemButton.SetActive(false);
            }
        }

        void ShowGameItemObjects()
        {
            itemPriceText.text = currentItemData.openGamePrice.ToString();
            itemPriceText.color = shopGameMoneyImage.color;
            shopGameMoneyImage.gameObject.SetActive(true);
            shopRealMoneyImage.gameObject.SetActive(false);
        }

        void ShowPremiumItemPriceObjects()
        {
            if (CanGetRustoreData())
            {
                if (rustorePayments.GetProductById(currentItemData.rustoreId) != null)
                    itemPriceText.text = rustorePayments?.GetProductById(currentItemData.rustoreId).amountLabel.value;
                else
                    itemPriceText.text = "???";
            }
            else
                itemPriceText.text = currentItemData.openRealPrice.ToString();

            itemPriceText.color = shopRealMoneyImage.color;
            shopGameMoneyImage.gameObject.SetActive(false);
            shopRealMoneyImage.gameObject.SetActive(true);
        }

        void ShowPurchasedItemObjects()
        {
            openItemButton.SetActive(false);

            if (!isShopItems)
            {
                premiumItemIsAvailablePanel.SetActive(false);
                itemIsAvailablePanel.SetActive(false);
                return;
            }

            if (currentItemData.IsPremium())
                premiumItemIsAvailablePanel.SetActive(true);
            else
                itemIsAvailablePanel.SetActive(true);
        }

        public void ShowOpenItemQuestionButtons(bool state)
        {
            openItemQuestionButtons.SetActive(state);
            openItemButtonImage.SetActive(!state);
        }

        public void TryPurchaseItem()
        {
            gameShop.TryPurchaseItem(currentItemData);
            additionalSceneObjects.ShowShopItem(currentItemData, false);

            ShowOpenItemQuestionButtons(false);
            UpdateItemOpenState();
            UIShipsSelection.Instance.UpdateButtonsState();
        }

        void HideAllItemIcons()
        {
            for (int i = 0; i < shopItemsData.Count; i++)
                shopItemsData[i].itemIconGameObject.SetActive(false);

            for (int i = 0; i < infopediaItemsData.Count; i++)
                infopediaItemsData[i].itemIconGameObject.SetActive(false);
        }

        bool CanGetRustoreData()
        {
            if (rustorePayments)
            {
                if (rustorePayments.RustoreIsAvailable && rustorePayments.UserIsAuthorized)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public void StartLoading(object sender, EventArgs e)
        {
            shopLoadingIndicator.Show();
        }

        public void BuyProductSuccess(object sender, EventArgs e)
        {
            shopBuySuccessWindow.SetActive(true);
            shopLoadingIndicator.Hide();
        }

        public void BuyProductFailed(object sender, EventArgs e)
        {
            shopBuyFailedWindow.SetActive(true);
            shopLoadingIndicator.Hide();

            shopBuyFailedWindow.GetComponent<UIShopMessagePanel>()?.Show(gameShop.GetCurrentError());
        }

        public void ShowShopItems()
        {
            shopButton.Select(true);
            isShopItems = true;

            HideInfopediaItems();
            ShowCurrentItem();
        }

        void HideShopItems()
        {
            shopButton.Select(false);
        }

        public void ShowInfopediaItems()
        {
            infopediaButton.Select(true);
            isShopItems = false;

            HideShopItems();
            ShowCurrentItem();
        }

        void HideInfopediaItems()
        {
            infopediaButton.Select(false);
        }
    }
}
