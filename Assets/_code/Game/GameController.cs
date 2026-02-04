using MegaGame.UI;
using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        public enum GameState { world, battle, menu }
        public GameState gameState;

        string playerMoney;
        string enemyMoney;

        [Header("Prices")]
        public int smallShipBuildingCost = 10;

        string smallShipCost;

        [Header("Ports")]
        public Port currentPlayerPort;
        public Port currentEnemyPort;

        [Header("3D Model Buttons")]
        [SerializeField] ModelButton startGameModelButton;
        [SerializeField] float offsetY = 0;

        [Header("Islands and Ports")]
        public List<Island> allIslands = new List<Island>();
        public List<Port> allPorts = new List<Port>();

        public List<Port> playerPorts = new List<Port>();
        public List<Port> enemyPorts = new List<Port>();

        public List<Port> allPossibleTargetPorts = new List<Port>();

        short playerPortsCount;
        public short PlayerPortsCount { get { return playerPortsCount; } }

        short enemyPortsCount;
        public short EnemyPortsCount { get { return enemyPortsCount; } }

        bool isVictory = false;
        public bool IsVictory { get { return isVictory; } }

        List<Island> startIslands = new List<Island>();
        List<Island> neutralIslands = new List<Island>();

        ObjectsManager objectsManager;
        CameraController cameraController;
        GlobalTimeController globalTime;
        ScenePrefabsManager scenePrefabsManager;

        public int currentDay = 0;

        [Header("Accounts")]
        public short currentAccountId = -1;
        public string currentAccountName;
        public short totalAccountsAmount = -1;

        short playerRevenue = 0;
        short playerMaintenance = 0;

        short enemyRevenue = 0;
        short enemyMaintenance = 0;

        short playerShipsCount = 0;
        short enemyShipsCount = 0;

        short playerStartIslandId;

        // Save Load Data
        DataSaveLoad dataSaveLoad;

        string islandOwnerFormat = " IO";
        string startPlayerIslandFormat = " SPI";

        string playerMoneyFormat = "PP";
        string enemyMoneyFormat = "EP";

        string currentDayFormat = "CD";

        string accountNameFormat = "ACC";
        string lastAccountIdFormat = "LACC";
        string currentAccountNameKey = "";
        string totalAccountsAmountFormat = "TAC";

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create GameController");
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
            if (gameState != GameState.battle)
                return;

            SelectObject();
            UpdateGameState();
            UpdateMoney();
        }

        public void Init()
        {
            dataSaveLoad = DataSaveLoad.Instance;
            globalTime = GlobalTimeController.Instance;
            objectsManager = ObjectsManager.Instance;
            cameraController = CameraController.Instance;
            scenePrefabsManager = ScenePrefabsManager.Instance;

            smallShipCost = Strint.GetString(smallShipBuildingCost);

            LoadLastAccount();

            startGameModelButton = FindFirstObjectByType<ModelButton>();
        }

        void InitializeScene()
        {
            playerPorts.Clear();
            enemyPorts.Clear();
            allPossibleTargetPorts.Clear();

            currentPlayerPort = null;
            currentEnemyPort = null;

            for (int i = 0; i < allIslands.Count; i++)
            {
                allIslands[i].owner = BaseCharacter.Owner.neutral;
                allIslands[i].UpdateIslandState();
            }
        }

        public void StartGame()
        {
            InitializeScene();
            
            LoadLastAccount();
            LoadGameData();

            for (int i = 0; i < allIslands.Count; i++)
            {
                if (allIslands[i].isStartIsland)
                    startIslands.Add(allIslands[i]);

                allIslands[i].UpdateIslandState();
            }

            if (playerStartIslandId == -1)
            {
                short rand = (short)Random.Range(0, startIslands.Count);
                playerStartIslandId = (short)startIslands[rand].islandData.id;
            }

            for (int i = 0; i < allPorts.Count; i++)
                if (allPorts[i].island.islandData.id == playerStartIslandId)
                    allPorts[i].owner = BaseCharacter.Owner.player;

            UpdatePortsLists();
            PrepareNewBattle();
        }

        void UpdatePortsLists()
        {
            playerPorts.Clear();
            enemyPorts.Clear();
            allPossibleTargetPorts.Clear();

            for (int i = 0; i < allPorts.Count; i++)
                if (allPorts[i].owner == BaseCharacter.Owner.player)
                    playerPorts.Add(allPorts[i]);
                else if (allPorts[i].owner == BaseCharacter.Owner.enemy)
                    enemyPorts.Add(allPorts[i]);

            playerPortsCount = (short)playerPorts.Count;
            enemyPortsCount = (short)enemyPorts.Count;

            for (int i = 0; i < playerPorts.Count; i++)
                for (int p = 0; p < playerPorts[i].island.possibleTargets.Count; p++)
                    if (playerPorts[i].island.possibleTargets[p].owner != BaseCharacter.Owner.player)
                        allPossibleTargetPorts.Add(playerPorts[i].island.possibleTargets[p].ports[0]);
        }

        void UpdatePortState(Port port, BaseCharacter.Owner owner)
        {
            port.owner = owner;
            port.Init();

            port.island.owner = owner;
            port.island.UpdateIslandState();
        }

        public void CreatePlayerShip()
        {
            if (Strint.Subtraction(playerMoney, smallShipCost) < 0)
                return;

            BuildShip(scenePrefabsManager.GetShipPrefab(true), currentPlayerPort.transform, currentEnemyPort.transform);
            RemoveMoneyFromPlayer(smallShipBuildingCost);
        }

        public void CreateEnemyShip()
        {
            if (Strint.Subtraction(enemyMoney, smallShipCost) < 0)
                return;

            BuildShip(scenePrefabsManager.GetShipPrefab(false), currentEnemyPort.transform, currentPlayerPort.transform);
            RemoveMoneyFromEnemy(smallShipBuildingCost);
        }

        public void BuildShip(GameObject shipOwner, Transform buildingPosition, Transform targetPosition)
        {
            GameObject ship = Instantiate(shipOwner, buildingPosition.position, buildingPosition.rotation);
            Character character = ship.GetComponent<Character>();
            character.destinationPosition = targetPosition;

            if (character.owner == BaseCharacter.Owner.player)
                ScenePrefabsManager.Instance.SpawnPortAsTargetFX(targetPosition.position, true);
            else if (character.owner == BaseCharacter.Owner.enemy)
                ScenePrefabsManager.Instance.SpawnPortAsTargetFX(targetPosition.position, false);
        }

        void SelectObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = CameraController.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 1000000, 1 << 9))
                {
                    Port port = hit.collider.GetComponentInParent<Port>();

                    if (port)
                        port.OnClickAction();
                }
            }
        }

        public void PrepareNewBattle()
        {
            ObjectsManager.Instance.Init();
            neutralIslands.Clear();

            for (int i = 0; i < allIslands.Count; i++)
            {
                SetIslandId(allIslands[i], i);

                allIslands[i].UpdateIslandState();

                if (allIslands[i].owner == BaseCharacter.Owner.neutral)
                    neutralIslands.Add(allIslands[i]);
            }

            if (neutralIslands.Count == 0)
            {
                EndCampaign();
                return;
            }

            CalculateCurrentPorts();

            if (currentEnemyPort)
            {
                UpdatePortState(currentEnemyPort, BaseCharacter.Owner.enemy);
                currentEnemyPort.SetVisualAsTarget(true, BaseCharacter.Owner.enemy);
            }
            else
            {
                EndCampaign();
                return;
            }

            UpdatePortsLists();
            PlaceCameraBetweenPorts();
            PlaceStartGameModelButtonBetweenPorts();
            SetGameStateAsWorld();

            startGameModelButton.gameObject.SetActive(true);

            SaveGameData();
        }

        void CalculateCurrentPorts()
        {
            if (playerPorts.Count <= 1)
            {
                Island currentPlayerIsland = null;

                for (int i = 0; i < allIslands.Count; i++)
                    if (allIslands[i].islandData.id == playerStartIslandId)
                        currentPlayerIsland = allIslands[i];

                currentPlayerPort = currentPlayerIsland.ports[0];
            }

            currentEnemyPort = allPossibleTargetPorts[Random.Range(0, allPossibleTargetPorts.Count)];

            if (playerPorts.Count > 1)
                currentPlayerPort = FindPossiblePlayerPortToTargetEnemyPort(currentEnemyPort);

            UpdatePortState(currentPlayerPort, BaseCharacter.Owner.player);
            currentPlayerPort.SetVisualAsTarget(true, BaseCharacter.Owner.player);
        }

        Port FindPossiblePlayerPortToTargetEnemyPort(Port target)
        {
            Port port = null;
            short rand = (short)Random.Range(0, 2);

            if (rand == 0)
            {
                for (int i = 0; i < target.island.possibleTargets.Count; i++)
                    if (target.island.possibleTargets[i].owner == BaseCharacter.Owner.player)
                        return target.island.possibleTargets[i].ports[0];
            }
            else
            {
                for (int i = target.island.possibleTargets.Count - 1; i >= 0; i--)
                    if (target.island.possibleTargets[i].owner == BaseCharacter.Owner.player)
                        return target.island.possibleTargets[i].ports[0];
            }

            return port;
        }

        public void StartBattle()
        {
            UpdatePortsLists();
            SetGameStateAsBattle();
            PlaceCameraBetweenPorts();
            startGameModelButton.gameObject.SetActive(false);
        }

        public void EndBattle()
        {
            ObjectsManager.Instance.Init();
            SaveGameData();
            UpdatePortsLists();
            SetGameStateAsWorld();
            startGameModelButton.gameObject.SetActive(false);
        }

        public void EndCampaign()
        {
            UIMainCanvas.Instance.ShowEndCampaignWindow();
            startGameModelButton.gameObject.SetActive(false);
        }

        void UpdateGameState()
        {
            if (!currentPlayerPort && !currentEnemyPort)
                return;

            if (currentPlayerPort.currentHealth <= 0)
            {
                UpdatePortState(currentPlayerPort, BaseCharacter.Owner.enemy);
                EndBattle();
                isVictory = false;
            }
            else if (currentEnemyPort.currentHealth <= 0)
            {
                UpdatePortState(currentEnemyPort, BaseCharacter.Owner.player);
                EndBattle();
                isVictory = true;
            }
        }

        void SetIslandId(Island island, int id)
        {
            if (island.islandData.id == -1)
            {
                island.islandData.id = id;
                island.islandData.SetId(id);
            }
        }

        void SaveGameData()
        {
            dataSaveLoad.Save(currentAccountNameKey + startPlayerIslandFormat, playerStartIslandId);

            for (int i = 0; i < allIslands.Count; i++)
            {
                if (allIslands[i].owner == BaseCharacter.Owner.player)
                    dataSaveLoad.Save(currentAccountNameKey + allIslands[i].islandData.id + islandOwnerFormat, (short)0);
                else if (allIslands[i].owner == BaseCharacter.Owner.enemy)
                    dataSaveLoad.Save(currentAccountNameKey + allIslands[i].islandData.id + islandOwnerFormat, (short)1);
                else if (allIslands[i].owner == BaseCharacter.Owner.neutral)
                    dataSaveLoad.Save(currentAccountNameKey + allIslands[i].islandData.id + islandOwnerFormat, (short)2);
                else if (allIslands[i].owner == BaseCharacter.Owner.mixed)
                    dataSaveLoad.Save(currentAccountNameKey + allIslands[i].islandData.id + islandOwnerFormat, (short)3);
            }

            dataSaveLoad.Save(currentAccountNameKey + playerMoneyFormat, playerMoney);
            dataSaveLoad.Save(currentAccountNameKey + enemyMoneyFormat, enemyMoney);

            dataSaveLoad.Save(currentAccountNameKey + "Player Money", Strint.GetInt(playerMoney));
            dataSaveLoad.Save(currentAccountNameKey + "Enemy Money", Strint.GetInt(enemyMoney));

            dataSaveLoad.Save(currentAccountNameKey + currentDayFormat, GlobalTimeController.Instance.currentDay);

            SaveLastAccount();
        }

        void LoadGameData()
        {
            playerStartIslandId = dataSaveLoad.GetSavedShort(currentAccountNameKey + startPlayerIslandFormat);

            for (int i = 0; i < allIslands.Count; i++)
            {
                if (dataSaveLoad.GetSavedShort(currentAccountNameKey + allIslands[i].islandData.id + islandOwnerFormat) == 0)
                    allIslands[i].owner = BaseCharacter.Owner.player;
                else if (dataSaveLoad.GetSavedShort(currentAccountNameKey + allIslands[i].islandData.id + islandOwnerFormat) == 1)
                    allIslands[i].owner = BaseCharacter.Owner.enemy;
                else if (dataSaveLoad.GetSavedShort(currentAccountNameKey + allIslands[i].islandData.id + islandOwnerFormat) == 2)
                    allIslands[i].owner = BaseCharacter.Owner.neutral;
                else if (dataSaveLoad.GetSavedShort(currentAccountNameKey + allIslands[i].islandData.id + islandOwnerFormat) == 3)
                    allIslands[i].owner = BaseCharacter.Owner.mixed;
            }

            playerMoney = dataSaveLoad.GetSavedString(currentAccountNameKey + playerMoneyFormat);
            enemyMoney = dataSaveLoad.GetSavedString(currentAccountNameKey + enemyMoneyFormat);

            GlobalTimeController.Instance.currentDay = dataSaveLoad.GetSavedInt(currentAccountNameKey + currentDayFormat);
            currentDay = dataSaveLoad.GetSavedInt(currentAccountNameKey + currentDayFormat);
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

        void PlaceCameraBetweenPorts()
        {
            if (!currentPlayerPort || !currentEnemyPort || !CameraController.Instance)
                return;

            CameraController.Instance.transform.position = CalculatePositionBetweenPorts();
        }

        void PlaceStartGameModelButtonBetweenPorts()
        {
            if (!currentPlayerPort || !currentEnemyPort || !CameraController.Instance || !startGameModelButton)
                return;

            startGameModelButton.transform.position = CalculatePositionBetweenPorts();
            startGameModelButton.transform.position += new Vector3(0, offsetY, 0);
        }

        Vector3 CalculatePositionBetweenPorts()
        {
            Vector3 newPosition = Vector3.zero;
            newPosition += currentPlayerPort.transform.position + currentEnemyPort.transform.position;
            newPosition /= 2;
            newPosition.y = 0;

            return newPosition;
        }

        void UpdateMoney()
        {
            if (playerShipsCount != objectsManager.playerShips.Count)
            {
                playerMaintenance = 0;

                for (int i = 0; i < objectsManager.playerShips.Count; i++)
                    playerMaintenance += objectsManager.playerShips[i].GetComponent<Character>().maintenance;

                playerShipsCount = (short)objectsManager.playerShips.Count;
            }

            if (enemyShipsCount != objectsManager.enemyShips.Count)
            {
                enemyMaintenance = 0;

                for (int i = 0; i < objectsManager.enemyShips.Count; i++)
                    enemyMaintenance += objectsManager.enemyShips[i].GetComponent<Character>().maintenance;

                enemyShipsCount = (short)objectsManager.enemyShips.Count;
            }

            if (globalTime.currentDay != currentDay)
            {
                playerRevenue = 0;
                enemyRevenue = 0;

                for (int i = 0; i < playerPorts.Count; i++)
                    playerRevenue += playerPorts[i].revenue;

                for (int i = 0; i < enemyPorts.Count; i++)
                    enemyRevenue += enemyPorts[i].revenue;

                AddMoneyToPlayer(playerRevenue);
                AddMoneyToEnemy(enemyRevenue);

                RemoveMoneyFromPlayer(playerMaintenance);
                RemoveMoneyFromEnemy(enemyMaintenance);

                currentDay = globalTime.currentDay;
            }
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

        public void SetGameStateAsWorld()
        {
            gameState = GameState.world;
            cameraController.SetTranslationZToMax();
        }

        public void SetGameStateAsBattle()
        {
            gameState = GameState.battle;
            cameraController.SetTranslationZToBase();
        }

        public void SetGameStateAsMenu()
        {
            gameState = GameState.menu;
        }

        void LoadLastAccount()
        {
            short lastAccount = dataSaveLoad.GetSavedShort(lastAccountIdFormat);

            if (dataSaveLoad.GetSavedShort(lastAccountIdFormat) == -1)
                currentAccountId = 1;
            else
                currentAccountId = lastAccount;

            currentAccountNameKey = accountNameFormat + currentAccountId.ToString() + "-";

            if (dataSaveLoad.GetSavedString(currentAccountNameKey) == "")
                currentAccountName = "A" + currentAccountId.ToString();
            else
                currentAccountName = dataSaveLoad.GetSavedString(currentAccountNameKey);

            if (dataSaveLoad.GetSavedInt(currentAccountNameKey + currentDayFormat) == -1)
            {
                GlobalTimeController.Instance.currentDay = 0;
                dataSaveLoad.Save(currentAccountNameKey + currentDayFormat, 0);
            }

            if (dataSaveLoad.GetSavedShort(totalAccountsAmountFormat) == -1)
                totalAccountsAmount = 1;
            else
                totalAccountsAmount = dataSaveLoad.GetSavedShort(totalAccountsAmountFormat);

            SaveLastAccount();
        }

        public void SaveLastAccount()
        {
            dataSaveLoad.Save(lastAccountIdFormat, currentAccountId);

            currentAccountNameKey = accountNameFormat + currentAccountId.ToString() + "-";
            dataSaveLoad.Save(currentAccountNameKey, currentAccountName);

            dataSaveLoad.Save(totalAccountsAmountFormat, totalAccountsAmount);
        }

        public string GetCurrentAccountName()
        {
            return currentAccountName;
        }

        public void SetAccountName(string textValue)
        {
            currentAccountName = textValue;
            SaveLastAccount();
        }

        public void CreateAccount(string textValue)
        {
            totalAccountsAmount++;
            currentAccountId = totalAccountsAmount;
            currentAccountName = textValue;
            SaveLastAccount();
        }

        public List<string> GetAccountsNames()
        {
            List<string> accountsNames = new List<string>();

            for (int i = 1; i <= totalAccountsAmount; i++)
                accountsNames.Add(dataSaveLoad.GetSavedString(accountNameFormat + i + "-"));

            return accountsNames;
        }

        public void LoadAccount(string targetAccountName)
        {
            int value = -1;
            List<string> accountsNames = GetAccountsNames();

            for (int i = 0; i < accountsNames.Count; i++)
                if (accountsNames[i] == targetAccountName)
                    value = i + 1;

            currentAccountId = (short)value;
            dataSaveLoad.Save(lastAccountIdFormat, currentAccountId);

            LoadLastAccount();
        }
    }
}
