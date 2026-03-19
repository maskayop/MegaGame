using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class ResourcesController : MonoBehaviour
    {
        public static ResourcesController Instance { get; private set; }

        [Header("Start Money")]
        [SerializeField] int startMoneyForPlayer;
        [SerializeField] int startMoneyForEnemy;

        [Header("Trader Profit")]
        [SerializeField] Vector2Int playerTraderProfit;
        [SerializeField] Vector2Int enemyTraderProfit;

        [Header("Send Spies")]
        [SerializeField] Data_Item sendSpiesItem;
        [SerializeField] int sendSpiesPriceMultiplier = 100;

        string playerMoney;
        public string PlayerMoney { get { return playerMoney; } set { playerMoney = value; } }

        string enemyMoney;
        public string EnemyMoney { get { return enemyMoney; } set { enemyMoney = value; } }

        int playerRevenue = 0;
        int playerMaintenance = 0;

        int enemyRevenue = 0;
        int enemyMaintenance = 0;

        int playerShipsCount = 0;
        int enemyShipsCount = 0;

        ObjectsManager objectsManager;
        GlobalTimeController globalTime;
        GameController gameController;
        GameShop gameShop;

        int currentDay = 0;
        public int CurrentDay { get { return currentDay; } set { currentDay = value; } }

        bool enemyResourcesAreOpen = false;
        public bool EnemyResourcesAreOpen { get { return enemyResourcesAreOpen; } }

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create ResourcesController");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                return;

            UpdateMoney();
        }

        public void Init()
        {
            globalTime = GlobalTimeController.Instance;
            objectsManager = ObjectsManager.Instance;
            gameController = GameController.Instance;
            gameShop = GameShop.Instance;
        }

        public void RemoveMoneyFromPlayer(int value)
        {
            if (Strint.Subtraction(playerMoney, Strint.GetString(value)) < 0)
            {
                playerMoney = Strint.GetString(0);
                return;
            }

            playerMoney = Strint.GetString(Strint.Subtraction(playerMoney, Strint.GetString(value)));
        }

        public void RemoveMoneyFromEnemy(int value)
        {
            if (Strint.Subtraction(enemyMoney, Strint.GetString(value)) < 0)
            {
                enemyMoney = Strint.GetString(0);
                return;
            }

            enemyMoney = Strint.GetString(Strint.Subtraction(enemyMoney, Strint.GetString(value)));
        }

        public void AddMoneyToPlayer(int value)
        {
            if (int.MaxValue - Strint.GetInt(playerMoney) <= value)
                playerMoney = Strint.GetString(int.MaxValue);
            else
                playerMoney = Strint.GetString(Strint.Summation(playerMoney, Strint.GetString(value)));
        }

        public void AddMoneyToEnemy(int value)
        {
            if (int.MaxValue - Strint.GetInt(enemyMoney) <= value)
                enemyMoney = Strint.GetString(int.MaxValue);
            else
                enemyMoney = Strint.GetString(Strint.Summation(enemyMoney, Strint.GetString(value)));
        }

        void UpdateMoney()
        {
            if (playerShipsCount != objectsManager.playerShips.Count)
                UpdatePlayerMaintenance();

            if (enemyShipsCount != objectsManager.enemyShips.Count)
                UpdateEnemyMaintenance();

            if (globalTime.currentDay != currentDay)
            {
                UpdatePlayerMaintenance();
                UpdateEnemyMaintenance();
                UpdateRevenues();
            }
        }

        public void UpdatePlayerMaintenance()
        {
            playerMaintenance = 0;
            playerMaintenance += GetPlayerShipsMaintenance();
            playerMaintenance += GetPlayerSettlementsMaintenance();
        }

        public void UpdateEnemyMaintenance()
        {
            enemyMaintenance = 0;
            enemyMaintenance += GetEnemyShipsMaintenance();
            enemyMaintenance += GetEnemySettlementsMaintenance();
        }

        public int GetPlayerShipsMaintenance()
        {
            int maintenance = 0;

            for (int i = 0; i < objectsManager.playerShips.Count; i++)
                if (objectsManager.playerShips[i])
                    if (objectsManager.playerShips[i].GetComponent<Warship>())
                        maintenance += objectsManager.playerShips[i].GetComponent<Warship>().maintenance;

            playerShipsCount = objectsManager.playerShips.Count;
            maintenance += GetShipsBureaucracyMaintenance(playerShipsCount);

            return maintenance;
        }

        public int GetEnemyShipsMaintenance()
        {
            int maintenance = 0;

            for (int i = 0; i < objectsManager.enemyShips.Count; i++)
                if (objectsManager.enemyShips[i])
                    if (objectsManager.enemyShips[i].GetComponent<Warship>())
                        maintenance += objectsManager.enemyShips[i].GetComponent<Warship>().maintenance;

            enemyShipsCount = objectsManager.enemyShips.Count;
            maintenance += GetShipsBureaucracyMaintenance(enemyShipsCount);

            return maintenance;
        }

        public int GetPlayerSettlementsMaintenance()
        {
            int maintenance = 0;

            for (int i = 0; i < gameController.playerPorts.Count; i++)
                if (gameController.playerPorts[i] && gameController.playerPorts[i].GetSettlementConstructions())
                    if (gameController.playerPorts[i].GetSettlementConstructions().fortIsBuilt)
                        maintenance += gameController.playerPorts[i].GetSettlementConstructions().fortMaintenance;

            return maintenance;
        }

        public int GetEnemySettlementsMaintenance()
        {
            int maintenance = 0;

            for (int i = 0; i < gameController.enemyPorts.Count; i++)
                if (gameController.enemyPorts[i] && gameController.enemyPorts[i].GetSettlementConstructions())
                    if (gameController.enemyPorts[i].GetSettlementConstructions().fortIsBuilt)
                        maintenance += gameController.enemyPorts[i].GetSettlementConstructions().fortMaintenance;

            return maintenance;
        }

        public int GetShipsBureaucracyMaintenance(int shipsCount)
        {
            int value = 0;

            if (shipsCount >= 0 && shipsCount <= 5)
                value = 0;
            else if (shipsCount > 5 && shipsCount <= 10)
                value = 1; // 0 + 1
            else if (shipsCount > 10 && shipsCount <= 15)
                value = 3; // 1 + 2
            else if (shipsCount > 15 && shipsCount <= 20)
                value = 6; // 1 + 2 + 3
            else if (shipsCount > 20 && shipsCount <= 25)
                value = 10; // 1 + 2 + 3 + 4
            else if (shipsCount > 25 && shipsCount <= 30)
                value = 15; // 1 + 2 + 3 + 4 + 5
            else if (shipsCount > 30 && shipsCount <= 35)
                value = 21; // 1 + 2 + 3 + 4 + 5 + 6
            else if (shipsCount > 35 && shipsCount <= 40)
                value = 28; // 1 + 2 + 3 + 4 + 5 + 6 + 7
            else if (shipsCount > 40 && shipsCount <= 45)
                value = 36; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8
            else if (shipsCount > 45 && shipsCount <= 50)
                value = 45; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9
            else if (shipsCount > 50 && shipsCount <= 55)
                value = 55; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10
            else if (shipsCount > 55 && shipsCount <= 60)
                value = 66; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11
            else if (shipsCount > 60 && shipsCount <= 65)
                value = 78; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12
            else if (shipsCount > 65 && shipsCount <= 70)
                value = 91; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12 + 13
            else if (shipsCount > 70 && shipsCount <= 75)
                value = 105; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12 + 13 + 14
            else if (shipsCount > 75 && shipsCount <= 80)
                value = 120; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12 + 13 + 14 + 15
            else if (shipsCount > 80 && shipsCount <= 85)
                value = 136; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12 + 13 + 14 + 15 + 16
            else if (shipsCount > 85 && shipsCount <= 90)
                value = 153; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12 + 13 + 14 + 15 + 16 + 17
            else if (shipsCount > 90 && shipsCount <= 95)
                value = 171; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12 + 13 + 14 + 15 + 16 + 17 + 18
            else if (shipsCount > 95 && shipsCount <= 100)
                value = 190; // 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12 + 13 + 14 + 15 + 16 + 17 + 18 + 19
            else if (shipsCount > 100 && shipsCount <= 150)
                value = 250;
            else if (shipsCount > 150 && shipsCount <= 200)
                value = 300;
            else if (shipsCount > 200 && shipsCount <= 250)
                value = 400;
            else if (shipsCount > 250)
                value = 500;
            else if (shipsCount > 300)
                value = 1000;

            return value;
        }

        public void UpdateRevenues()
        {
            playerRevenue = 0;
            enemyRevenue = 0;

            for (int i = 0; i < gameController.playerPorts.Count; i++)
                playerRevenue += gameController.playerPorts[i].revenue;

            for (int i = 0; i < gameController.playerVillages.Count; i++)
                playerRevenue += gameController.playerVillages[i].revenue;

            for (int i = 0; i < gameController.enemyPorts.Count; i++)
                enemyRevenue += gameController.enemyPorts[i].revenue;

            for (int i = 0; i < gameController.enemyVillages.Count; i++)
                enemyRevenue += gameController.enemyVillages[i].revenue;

            AddMoneyToPlayer(playerRevenue);
            AddMoneyToEnemy(enemyRevenue);

            RemoveMoneyFromPlayer(playerMaintenance);
            RemoveMoneyFromEnemy(enemyMaintenance);

            currentDay = globalTime.currentDay;
        }

        public int GetPlayerMoney()
        {
            return Strint.GetInt(playerMoney);
        }

        public int GetEnemyMoney()
        {
            return Strint.GetInt(enemyMoney);
        }

        public int GetPlayerRevenue()
        {
            return playerRevenue;
        }

        public int GetEnemyRevenue()
        {
            return enemyRevenue;
        }

        public int GetPlayerMaintenance()
        {
            return playerMaintenance;
        }

        public int GetEnemyMaintenance()
        {
            return enemyMaintenance;
        }

        public void AddBattleStartMoneyToPlayer(int multiplier)
        {
            AddMoneyToPlayer(startMoneyForPlayer * multiplier);
        }

        public void AddBattleStartMoneyToEnemy(int multiplier)
        {
            AddMoneyToEnemy(startMoneyForEnemy * multiplier);
        }

        public int GetRandomTraderProfit(bool isPlayer)
        {
            int r = 0;

            if (isPlayer)
                r = Random.Range(playerTraderProfit.x, playerTraderProfit.y);
            else
                r = Random.Range(enemyTraderProfit.x, enemyTraderProfit.y);

            return r;
        }

        public int GetSendSpiesPrice()
        {
            return gameController.EnemyPortsCount * sendSpiesPriceMultiplier;
        }

        public void TryOpenEnemyResources()
        {
            if (Strint.Subtraction(playerMoney, Strint.GetString(GetSendSpiesPrice())) < 0)
                return;

            enemyResourcesAreOpen = true;
            RemoveMoneyFromPlayer(GetSendSpiesPrice());
        }

        public void CloseEnemyResources()
        {
            if (gameShop)
            {
                if (gameShop.CheckForPurchasing(sendSpiesItem))
                    enemyResourcesAreOpen = true;
                else
                    enemyResourcesAreOpen = false;
            }
            else
                enemyResourcesAreOpen = false;
        }
    }
}
