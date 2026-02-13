using UnityEngine;
using TMPro;

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

        [Header("Enemy")]
        [SerializeField] string growthColorFormat = "<color=green>";
        [SerializeField] string wasteColorFormat = "<color=red>";

        GameController gameController;
        ObjectsManager objectsManager;
        ResourcesController resourcesController;

        int playerMoneyGrowth;
        int enemyMoneyGrowth;

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
        }

        void Update()
        {
            if (gameController.CampaignIsEnded)
                return;

            UpdateGameCharacteristics();
        }

        void UpdateGameCharacteristics()
        {
            playerMoneyGrowth = resourcesController.GetPlayerRevenue() - resourcesController.GetPlayerMaintenance();

            if (playerMoneyGrowth > 0)
                playerMoneyAmountText.text = resourcesController.GetPlayerMoney().ToString() + growthColorFormat + " +" + playerMoneyGrowth.ToString() + "</color>" + "</size>";
            else if (playerMoneyGrowth == 0)
                playerMoneyAmountText.text = resourcesController.GetPlayerMoney().ToString();
            else 
                playerMoneyAmountText.text = resourcesController.GetPlayerMoney().ToString() + wasteColorFormat + " " + playerMoneyGrowth.ToString() + "</color>" + "</size>";

            playerShipsAmountText.text = objectsManager.playerShips.Count.ToString();
            playerPortsAmountText.text = gameController.PlayerPortsCount.ToString();
            playerVillagesAmountText.text = gameController.PlayerVillagesCount.ToString();
            playerRevenueText.text = "+" + resourcesController.GetPlayerRevenue().ToString();
            playerMaintenanceText.text = "-" + resourcesController.GetPlayerMaintenance().ToString();

            enemyMoneyGrowth = resourcesController.GetEnemyRevenue() - resourcesController.GetEnemyMaintenance();

            if (enemyMoneyGrowth > 0)
                enemyMoneyAmountText.text = resourcesController.GetEnemyMoney().ToString() + growthColorFormat + " +" + enemyMoneyGrowth.ToString() + "</color>" + "</size>";
            else if (enemyMoneyGrowth == 0)
                enemyMoneyAmountText.text = resourcesController.GetEnemyMoney().ToString();
            else
                enemyMoneyAmountText.text = resourcesController.GetEnemyMoney().ToString() + wasteColorFormat + " " + enemyMoneyGrowth.ToString() + "</color>" + "</size>";

            enemyShipsAmountText.text = objectsManager.enemyShips.Count.ToString();
            enemyPortsAmountText.text = gameController.EnemyPortsCount.ToString();
            enemyVillagesAmountText.text = gameController.EnemyVillagesCount.ToString();
            enemyRevenueText.text = "+" + resourcesController.GetEnemyRevenue().ToString();
            enemyMaintenanceText.text = "-" + resourcesController.GetEnemyMaintenance().ToString();
        }
    }
}
