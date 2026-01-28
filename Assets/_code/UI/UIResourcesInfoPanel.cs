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

        GameController gameController;
        ObjectsManager objectsManager;

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
            playerMoneyAmountText.text = gameController.GetPlayerMoney().ToString();
            playerShipsAmountText.text = objectsManager.playerShips.Count.ToString();
            playerPortsAmountText.text = gameController.PlayerPortsCount.ToString();
            playerRevenueText.text = "+" + gameController.GetPlayerRevenue().ToString();
            playerMaintenanceText.text = "-" + gameController.GetPlayerMaintenance().ToString();

            enemyMoneyAmountText.text = gameController.GetEnemyMoney().ToString();
            enemyShipsAmountText.text = objectsManager.enemyShips.Count.ToString();
            enemyPortsAmountText.text = gameController.EnemyPortsCount.ToString();
            enemyRevenueText.text = "+" + gameController.GetEnemyRevenue().ToString();
            enemyMaintenanceText.text = "-" + gameController.GetEnemyMaintenance().ToString();
        }
    }
}
