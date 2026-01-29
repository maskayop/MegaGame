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
        [SerializeField] TextMeshProUGUI playerRevenueText;
        [SerializeField] TextMeshProUGUI playerMaintenanceText;

        [Header("Enemy")]
        [SerializeField] TextMeshProUGUI enemyMoneyAmountText;
        [SerializeField] TextMeshProUGUI enemyShipsAmountText;
        [SerializeField] TextMeshProUGUI enemyPortsAmountText;
        [SerializeField] TextMeshProUGUI enemyRevenueText;
        [SerializeField] TextMeshProUGUI enemyMaintenanceText;

        [Header("Enemy")]
        [SerializeField] string growthColorFormat = "<color=green>";
        [SerializeField] string wasteColorFormat = "<color=red>";

        GameController gameController;
        ObjectsManager objectsManager;

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
        }

        void Update()
        {
            UpdateGameCharacteristics();
        }

        void UpdateGameCharacteristics()
        {
            playerMoneyGrowth = gameController.GetPlayerRevenue() - gameController.GetPlayerMaintenance();

            if (playerMoneyGrowth > 0)
                playerMoneyAmountText.text = gameController.GetPlayerMoney().ToString() + growthColorFormat + " +" + playerMoneyGrowth.ToString() + "</color>" + "</size>";
            else if (playerMoneyGrowth == 0)
                playerMoneyAmountText.text = gameController.GetPlayerMoney().ToString();
            else 
                playerMoneyAmountText.text = gameController.GetPlayerMoney().ToString() + wasteColorFormat + " -" + playerMoneyGrowth.ToString() + "</color>" + "</size>";

            playerShipsAmountText.text = objectsManager.playerShips.Count.ToString();
            playerPortsAmountText.text = gameController.PlayerPortsCount.ToString();
            playerRevenueText.text = "+" + gameController.GetPlayerRevenue().ToString();
            playerMaintenanceText.text = "-" + gameController.GetPlayerMaintenance().ToString();

            enemyMoneyGrowth = gameController.GetEnemyRevenue() - gameController.GetEnemyMaintenance();

            if (enemyMoneyGrowth > 0)
                enemyMoneyAmountText.text = gameController.GetEnemyMoney().ToString() + growthColorFormat + " +" + enemyMoneyGrowth.ToString() + "</color>" + "</size>";
            else if (enemyMoneyGrowth == 0)
                enemyMoneyAmountText.text = gameController.GetEnemyMoney().ToString();
            else
                enemyMoneyAmountText.text = gameController.GetEnemyMoney().ToString() + wasteColorFormat + " -" + enemyMoneyGrowth.ToString() + "</color>" + "</size>";

            enemyShipsAmountText.text = objectsManager.enemyShips.Count.ToString();
            enemyPortsAmountText.text = gameController.EnemyPortsCount.ToString();
            enemyRevenueText.text = "+" + gameController.GetEnemyRevenue().ToString();
            enemyMaintenanceText.text = "-" + gameController.GetEnemyMaintenance().ToString();
        }
    }
}
