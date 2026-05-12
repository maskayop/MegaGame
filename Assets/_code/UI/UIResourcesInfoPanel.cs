using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIResourcesInfoPanel : MonoBehaviour
    {
        public static UIResourcesInfoPanel Instance { get; private set; }

        [Header("Player")]
        [SerializeField] TextMeshProUGUI playerMoneyAmountText;
        [SerializeField] TextMeshProUGUI playerShipsAmountText;
        [SerializeField] TextMeshProUGUI playerPortsAmountText;
        [SerializeField] TextMeshProUGUI playerVillagesAmountText;
        [SerializeField] TextMeshProUGUI playerRevenueText;
        [SerializeField] TextMeshProUGUI playerMaintenanceText;

        [Header("Enemy")]
        [SerializeField] TextMeshProUGUI enemyMoneyAmountText;
        [SerializeField] TextMeshProUGUI enemyShipsAmountText;
        [SerializeField] TextMeshProUGUI enemyPortsAmountText;
        [SerializeField] TextMeshProUGUI enemyVillagesAmountText;
        [SerializeField] TextMeshProUGUI enemyRevenueText;
        [SerializeField] TextMeshProUGUI enemyMaintenanceText;

        [Header("Send Spies")]
        [SerializeField] Data_Item sendSpiesItem;
        [SerializeField] UISendSpiesButton sendSpiesButton;
        [SerializeField] GameObject sendSpiesPanel;
        [SerializeField] TextMeshProUGUI sendSpicesPriceText;
        [SerializeField] Button sendSpicesButton;

        GameController gameController;
        ObjectsManager objectsManager;
        ResourcesController resourcesController;
        UIColors uiColors;

        int playerMoneyGrowth;
        int enemyMoneyGrowth;

        bool sendSpiesPanelIsOpen = false;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIResourcesInfoPanel");
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
            gameController = GameController.Instance;
            objectsManager = ObjectsManager.Instance;
            resourcesController = ResourcesController.Instance;
            uiColors = UIColors.Instance;
        }

        void Update()
        {
            if (!gameController)
                return;

            if (gameController.CampaignIsEnded)
                return;

            if (gameController.gameState == GameController.GameState.battle)
            {
                sendSpiesButton.gameObject.SetActive(true);

                if (resourcesController.EnemyResourcesAreOpen)
                    sendSpiesButton.Select(true);
                else
                    sendSpiesButton.Select(false);
            }
            else
                sendSpiesButton.gameObject.SetActive(false);

            UpdateFractionsResources();

            if (sendSpiesPanelIsOpen)
            {
                sendSpicesPriceText.text = resourcesController.GetSendSpiesPrice().ToString();

                if (resourcesController.GetSendSpiesPrice() > resourcesController.GetPlayerMoney())
                    sendSpicesButton.interactable = false;
                else
                    sendSpicesButton.interactable = true;
            }
        }

        void UpdateFractionsResources()
        {
            UpdatePlayerResources();
            UpdateEnemyResources();
        }

        void UpdatePlayerResources()
        {
            playerMoneyGrowth = resourcesController.GetPlayerRevenue() - resourcesController.GetPlayerShipsMaintenance() -
                resourcesController.GetPlayerSettlementsMaintenance();

            if (playerMoneyGrowth > 0)
                playerMoneyAmountText.text = resourcesController.GetPlayerMoney().ToString() + uiColors.GetMoneyGrowthColorString()
                    + " +" + playerMoneyGrowth.ToString() + "</color></size>";
            else if (playerMoneyGrowth == 0)
                playerMoneyAmountText.text = resourcesController.GetPlayerMoney().ToString();
            else
                playerMoneyAmountText.text = resourcesController.GetPlayerMoney().ToString() + uiColors.GetMoneyWasteColorString()
                    + " " + playerMoneyGrowth.ToString() + "</color></size>";

            playerShipsAmountText.text = objectsManager.playerShips.Count.ToString();
            playerPortsAmountText.text = gameController.PlayerPortsCount.ToString();
            playerVillagesAmountText.text = gameController.PlayerVillagesCount.ToString();
            playerRevenueText.text = "+" + (resourcesController.GetPlayerRevenue() - resourcesController.GetPlayerSettlementsMaintenance()).ToString();
            playerMaintenanceText.text = "-" + resourcesController.GetPlayerShipsMaintenance().ToString();
        }

        void UpdateEnemyResources()
        {
            if (resourcesController.EnemyResourcesAreOpen)
            {
                enemyMoneyGrowth = resourcesController.GetEnemyRevenue() - resourcesController.GetEnemyShipsMaintenance() -
                    resourcesController.GetEnemySettlementsMaintenance();

                if (enemyMoneyGrowth > 0)
                    enemyMoneyAmountText.text = resourcesController.GetEnemyMoney().ToString() + uiColors.GetMoneyGrowthColorString()
                        + " +" + enemyMoneyGrowth.ToString() + "</color></size>";
                else if (enemyMoneyGrowth == 0)
                    enemyMoneyAmountText.text = resourcesController.GetEnemyMoney().ToString();
                else
                    enemyMoneyAmountText.text = resourcesController.GetEnemyMoney().ToString() + uiColors.GetMoneyWasteColorString()
                        + " " + enemyMoneyGrowth.ToString() + "</color></size>";

                enemyShipsAmountText.text = objectsManager.enemyShips.Count.ToString();
                enemyPortsAmountText.text = gameController.EnemyPortsCount.ToString();
                enemyVillagesAmountText.text = gameController.EnemyVillagesCount.ToString();
                enemyRevenueText.text = "+" + (resourcesController.GetEnemyRevenue() - resourcesController.GetEnemySettlementsMaintenance()).ToString();
                enemyMaintenanceText.text = "-" + resourcesController.GetEnemyShipsMaintenance().ToString();
            }
            else
            {
                enemyMoneyAmountText.text = enemyShipsAmountText.text = enemyPortsAmountText.text =
                    enemyVillagesAmountText.text = enemyRevenueText.text = enemyMaintenanceText.text = "???";
            }
        }

        public void OpenSendSpiesPanel()
        {
            sendSpiesPanelIsOpen = true;
            sendSpiesPanel.SetActive(true);
        }

        public void CloseSendSpiesPanel()
        {
            sendSpiesPanelIsOpen = false;
            sendSpiesPanel.SetActive(false);
        }

        public void TryOpenEnemyResources()
        {
            resourcesController.TryOpenEnemyResources();
            CloseSendSpiesPanel();
        }
    }
}
