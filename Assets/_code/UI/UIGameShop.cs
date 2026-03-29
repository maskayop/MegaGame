using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        BaseCharacter currentCharacter;

        RustorePayments rustorePayments;

        bool isInitialized = false;

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
                currentItemData = shopItemsData[0].itemData;
                currentItemId = 0;
            }

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

            UIShipsSelection.Instance?.UpdateButtonsState();
            ResourcesController.Instance?.CloseEnemyResources();

            if (isInitialized)
                gameDataSaver.SavePremiumShopData();
        }

        void ShowCurrentItem()
        {
            currentItemData = shopItemsData[currentItemId].itemData;
            additionalSceneObjects.ShowShopItem(currentItemData);

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
            shopItemsData[currentItemId].itemIconGameObject.SetActive(true);

            UpdateItemOpenState();
            UpdateItemTexts();
            ShowOpenItemQuestionButtons(false);
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
                itemIsAvailablePanel.SetActive(true);
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
            additionalSceneObjects.ShowShopItem(currentItemData);

            ShowOpenItemQuestionButtons(false);
            UpdateItemOpenState();
            UIShipsSelection.Instance.UpdateButtonsState();
        }

        void HideAllItemIcons()
        {
            for (int i = 0; i < shopItemsData.Count; i++)
                shopItemsData[i].itemIconGameObject.SetActive(false);
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
    }
}
