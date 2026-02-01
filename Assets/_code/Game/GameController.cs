using MegaGame.UI;
using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        public enum GameState { world, battle }
        public GameState gameState;

        string playerMoney;
        string enemyMoney;

        [Header("Prices")]
        public int smallShipBuildingCost = 10;

        string smallShipCost;

        [Header("Ports")]
        public Port playerPort;
        public Port enemyPort;

        [Header("Gameplay")]
        [SerializeField] GameObject shipPlayerPrefab;
        [SerializeField] GameObject shipEnemyPrefab;

        [Header("3D Model Buttons")]
        [SerializeField] ModelButton startGameModelButton;
        [SerializeField] float offsetY = 0;

        [Header("Islands and Ports")]
        public List<Island> allIslands = new List<Island>();
        public List<Port> allPorts = new List<Port>();

        List<Port> playerPorts = new List<Port>();
        List<Port> enemyPorts = new List<Port>();

        short playerPortsCount;
        public short PlayerPortsCount { get { return playerPortsCount; } }

        short enemyPortsCount;
        public short EnemyPortsCount { get { return enemyPortsCount; } }

        bool isVictory = false;
        public bool IsVictory { get { return isVictory; } }

        bool isBattle = false;
        public bool IsBattle { get { return isBattle; } }

        List<Island> neutralIslands = new List<Island>();

        ObjectsManager objectsManager;
        CameraController cameraController;
        GlobalTimeController globalTime;

        int currentDay = 0;

        int playerRevenue = 0;
        int playerMaintenance = 0;

        int enemyRevenue = 0;
        int enemyMaintenance = 0;

        short playerShipsCount = 0;
        short enemyShipsCount = 0;

        // Save Load Data
        DataSaveLoad dataSaveLoad;

        string islandOwnerFormat = " IO";

        string playerMoneyFormat = "PP";
        string enemyMoneyFormat = "EP";

        string currentDayFormat = "CD";

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
            dataSaveLoad = DataSaveLoad.Instance;

            globalTime = GlobalTimeController.Instance;
            currentDay = globalTime.currentDay;

            objectsManager = ObjectsManager.Instance;
            cameraController = CameraController.Instance;

            smallShipCost = Strint.GetString(smallShipBuildingCost);

            if (dataSaveLoad.GetSavedInt(currentDayFormat) < 0)
            {
                GlobalTimeController.Instance.currentDay = 0;
                dataSaveLoad.Save(currentDayFormat, 0);
            }

            startGameModelButton = FindFirstObjectByType<ModelButton>();

            SetGameStateAsWorld();
        }

        void Update()
        {
            SelectObject();
            UpdateGameState();
            UpdateMoney();
        }

        public void Init()
        {
            LoadGameData();

            neutralIslands.Clear();

            for (int i = 0; i < allIslands.Count; i++)
            {
                SetIslandId(allIslands[i], i);

                allIslands[i].UpdateIslandState();

                if (allIslands[i].owner == BaseCharacter.Owner.neutral)
                    neutralIslands.Add(allIslands[i]);
            }

            int r = Random.Range(0, neutralIslands.Count);

            if (neutralIslands.Count == 0)
            {
                EndCampaign();
                return;
            }

            playerPort = neutralIslands[r].ports[0];
            UpdatePortState(playerPort, BaseCharacter.Owner.player);
            playerPort.SetVisualAsTarget(true, BaseCharacter.Owner.player);

            enemyPort = FindClosestNeutralPortToTargetPort(playerPort);

            if (enemyPort != null)
            {
                UpdatePortState(enemyPort, BaseCharacter.Owner.enemy);
                enemyPort.SetVisualAsTarget(true, BaseCharacter.Owner.enemy);
            }
            else
            {
                EndCampaign();
                return;
            }

            SaveGameData();
            UpdatePortsLists();

            PlaceCameraBetweenPorts();
            PlaceStartGameModelButtonBetweenPorts();
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

            BuildShip(shipPlayerPrefab, playerPort.transform, enemyPort.transform);
            RemoveMoneyFromPlayer(smallShipBuildingCost);
        }

        public void CreateEnemyShip()
        {
            if (Strint.Subtraction(enemyMoney, smallShipCost) < 0)
                return;

            BuildShip(shipEnemyPrefab, enemyPort.transform, playerPort.transform);
            RemoveMoneyFromEnemy(smallShipBuildingCost);
        }

        public void BuildShip(GameObject shipOwner, Transform buildingPosition, Transform targetPosition)
        {
            GameObject ship = Instantiate(shipOwner, buildingPosition.position, buildingPosition.rotation);
            Character character = ship.GetComponent<Character>();
            character.destinationPosition = targetPosition;
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

        Port FindClosestNeutralPortToTargetPort(Port target)
        {
            float distance = 1000000;
            Port port = null;

            for (int i = 0; i < allPorts.Count; i++)
            {
                float tempDistance = Vector3.Distance(allPorts[i].transform.position, target.transform.position);

                if (allPorts[i] != target)
                {
                    if (tempDistance < distance && allPorts[i].owner == BaseCharacter.Owner.neutral)
                    {
                        distance = tempDistance;
                        port = allPorts[i];
                    }
                }
            }

            return port;
        }

        public void StartBattle()
        {
            Init();
            isBattle = true;
            SetGameStateAsBattle();

            startGameModelButton.gameObject.SetActive(false);
        }

        public void EndBattle()
        {
            isBattle = false;
            PrepareNewBattle();
            SaveGameData();
            SetGameStateAsWorld();

            startGameModelButton.gameObject.SetActive(true);
        }

        public void EndCampaign()
        {
            UIMainCanvas.Instance.ShowEndCampaignWindow();
        }

        void UpdateGameState()
        {
            if (!playerPort || !enemyPort)
                return;

            if (playerPort.currentHealth <= 0)
            {
                EndBattle();
                UpdatePortState(playerPort, BaseCharacter.Owner.enemy);
                isVictory = false;
            }
            else if (enemyPort.currentHealth <= 0)
            {
                EndBattle();
                UpdatePortState(enemyPort, BaseCharacter.Owner.player);
                isVictory = true;
            }
        }

        public void PrepareNewBattle()
        {
            ObjectsManager.Instance.Init();
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
            for (int i = 0; i < allIslands.Count; i++)
            {
                if (allIslands[i].owner == BaseCharacter.Owner.player)
                    dataSaveLoad.Save(allIslands[i].islandData.id + islandOwnerFormat, 0);
                else if (allIslands[i].owner == BaseCharacter.Owner.enemy)
                    dataSaveLoad.Save(allIslands[i].islandData.id + islandOwnerFormat, 1);
                else if (allIslands[i].owner == BaseCharacter.Owner.neutral)
                    dataSaveLoad.Save(allIslands[i].islandData.id + islandOwnerFormat, 2);
                else if (allIslands[i].owner == BaseCharacter.Owner.mixed)
                    dataSaveLoad.Save(allIslands[i].islandData.id + islandOwnerFormat, 3);
            }

            dataSaveLoad.Save(playerMoneyFormat, playerMoney);
            dataSaveLoad.Save(enemyMoneyFormat, enemyMoney);

            dataSaveLoad.Save("Player Money", Strint.GetInt(playerMoney));
            dataSaveLoad.Save("Enemy Money", Strint.GetInt(enemyMoney));

            dataSaveLoad.Save(currentDayFormat, GlobalTimeController.Instance.currentDay);
        }

        void LoadGameData()
        {
            for (int i = 0; i < allIslands.Count; i++)
            {
                if (dataSaveLoad.GetSavedInt(allIslands[i].islandData.id + islandOwnerFormat) == 0)
                    allIslands[i].owner = BaseCharacter.Owner.player;
                else if (dataSaveLoad.GetSavedInt(allIslands[i].islandData.id + islandOwnerFormat) == 1)
                    allIslands[i].owner = BaseCharacter.Owner.enemy;
                else if (dataSaveLoad.GetSavedInt(allIslands[i].islandData.id + islandOwnerFormat) == 2)
                    allIslands[i].owner = BaseCharacter.Owner.neutral;
                else if (dataSaveLoad.GetSavedInt(allIslands[i].islandData.id + islandOwnerFormat) == 3)
                    allIslands[i].owner = BaseCharacter.Owner.mixed;
            }

            playerMoney = dataSaveLoad.GetSavedString(playerMoneyFormat);
            enemyMoney = dataSaveLoad.GetSavedString(enemyMoneyFormat);

            GlobalTimeController.Instance.currentDay = dataSaveLoad.GetSavedInt(currentDayFormat);
        }

        public void RemoveMoneyFromPlayer(int value)
        {
            playerMoney = Strint.GetString(Strint.Subtraction(playerMoney, Strint.GetString(value)));
        }

        public void RemoveMoneyFromEnemy(int value)
        {
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
            if (!playerPort || !enemyPort || !CameraController.Instance)
                return;

            CameraController.Instance.transform.position = CalculatePositionBetweenPorts();
        }

        void PlaceStartGameModelButtonBetweenPorts()
        {
            if (!playerPort || !enemyPort || !CameraController.Instance || !startGameModelButton)
                return;

            startGameModelButton.transform.position = CalculatePositionBetweenPorts();
            startGameModelButton.transform.position += new Vector3(0, offsetY, 0);
        }

        Vector3 CalculatePositionBetweenPorts()
        {
            Vector3 newPosition = Vector3.zero;
            newPosition += playerPort.transform.position + enemyPort.transform.position;
            newPosition /= 2;
            newPosition.y = 0;

            return newPosition;
        }

        void UpdatePortsLists()
        {
            playerPorts.Clear();
            enemyPorts.Clear();

            for (int i = 0; i < allPorts.Count; i++)
                if (allPorts[i].owner == BaseCharacter.Owner.player)
                    playerPorts.Add(allPorts[i]);
                else if (allPorts[i].owner == BaseCharacter.Owner.enemy)
                    enemyPorts.Add(allPorts[i]);

            playerPortsCount = (short)playerPorts.Count;
            enemyPortsCount = (short)enemyPorts.Count;
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
    }
}
