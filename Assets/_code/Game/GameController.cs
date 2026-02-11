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

        [HideInInspector]
        public string playerMoney;
        [HideInInspector]
        public string enemyMoney;

        [Header("Ports")]
        public Port currentPlayerPort;
        public Port currentEnemyPort;

        [Header("3D Model Buttons")]
        [SerializeField] ModelButton startGameModelButton;
        [SerializeField] float offsetY = 0;

        [Header("Islands and Settlements")]
        public List<Island> allIslands = new List<Island>();

        public List<Port> allPorts = new List<Port>();
        public List<Village> allVillages = new List<Village>();

        public List<Port> playerPorts = new List<Port>();
        public List<Village> playerVillages = new List<Village>();

        public List<Port> enemyPorts = new List<Port>();
        public List<Village> enemyVillages = new List<Village>();

        public List<Port> allPossibleTargetPorts = new List<Port>();

        [Header("Enemy's Targets")]
        public short distanceForEnemyPossibleTargets = 100;
        public List<Village> possibleTargetVillagesForEnemy = new List<Village>();

        short playerPortsCount;
        public short PlayerPortsCount { get { return playerPortsCount; } }

        short playerVillagesCount;
        public short PlayerVillagesCount { get { return playerVillagesCount; } }

        short enemyPortsCount;
        public short EnemyPortsCount { get { return enemyPortsCount; } }

        short enemyVillagesCount;
        public short EnemyVillagesCount { get { return enemyVillagesCount; } }

        bool isVictory = false;
        public bool IsVictory { get { return isVictory; } set { isVictory = value; } }

        bool campaignIsEnded = false;
        public bool CampaignIsEnded { get { return campaignIsEnded; } set { campaignIsEnded = value; } }

        List<Island> startIslands = new List<Island>();
        List<Island> neutralIslands = new List<Island>();

        ObjectsManager objectsManager;
        CameraController cameraController;
        GlobalTimeController globalTime;
        GameDataSaver gameDataSaver;

        int currentDay = 0;
        public int CurrentDay { get { return currentDay; } set { currentDay = value; } }

        short playerRevenue = 0;
        short playerMaintenance = 0;

        short enemyRevenue = 0;
        short enemyMaintenance = 0;

        short playerShipsCount = 0;
        short enemyShipsCount = 0;

        short playerStartIslandId;
        public short PlayerStartIslandId { get { return playerStartIslandId; } set { playerStartIslandId = value; } }

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
            if (campaignIsEnded)
                return;

            if (gameState != GameState.battle)
                return;

            UpdateGameState();
            UpdateMoney();
        }

        public void Init()
        {
            globalTime = GlobalTimeController.Instance;
            objectsManager = ObjectsManager.Instance;
            cameraController = CameraController.Instance;
            gameDataSaver = GameDataSaver.Instance;
            gameDataSaver.Init();

            gameDataSaver.LoadLastAccount();

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

            gameDataSaver.LoadLastAccount();
            gameDataSaver.LoadGameData();

            if (campaignIsEnded)
            {
                EndCampaign();
                return;
            }

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
                if (allPorts[i].Island.islandData.id == playerStartIslandId)
                    allPorts[i].owner = BaseCharacter.Owner.player;

            UpdateSettlementsLists();
            PrepareNewBattle();

            campaignIsEnded = false;
        }

        public void UpdateSettlementsLists()
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
                for (int p = 0; p < playerPorts[i].Island.possibleTargets.Count; p++)
                    if (playerPorts[i].Island.possibleTargets[p].owner != BaseCharacter.Owner.player)
                        allPossibleTargetPorts.Add(playerPorts[i].Island.possibleTargets[p].ports[0]);

            playerVillages.Clear();
            enemyVillages.Clear();

            for (int i = 0; i < allVillages.Count; i++)
                if (allVillages[i].owner == BaseCharacter.Owner.player)
                    playerVillages.Add(allVillages[i]);
                else if (allVillages[i].owner == BaseCharacter.Owner.enemy)
                    enemyVillages.Add(allVillages[i]);

            playerVillagesCount = (short)playerVillages.Count;
            enemyVillagesCount = (short)enemyVillages.Count;

            possibleTargetVillagesForEnemy.Clear();

            if (currentEnemyPort)
            {
                for (int i = 0; i < allVillages.Count; i++)
                    if (allVillages[i].owner != BaseCharacter.Owner.enemy)
                        if (Vector3.Distance(allVillages[i].transform.position, currentEnemyPort.transform.position) <= distanceForEnemyPossibleTargets)
                            possibleTargetVillagesForEnemy.Add(allVillages[i]);
            }
        }

        void UpdatePortState(Port port, BaseCharacter.Owner owner)
        {
            port.owner = owner;
            port.Init();

            port.Island.owner = owner;
            port.Island.UpdateIslandState();
        }

        public void PrepareNewBattle()
        {
            ObjectsManager.Instance.Init();
            neutralIslands.Clear();

            for (int i = 0; i < allIslands.Count; i++)
            {
                SetIslandId(allIslands[i]);

                allIslands[i].UpdateIslandState();

                if (allIslands[i].owner == BaseCharacter.Owner.neutral)
                    neutralIslands.Add(allIslands[i]);
            }

            if (allPossibleTargetPorts.Count == 0)
            {
                isVictory = false;
                EndCampaign();
                return;
            }

            CalculateCurrentPorts();

            if (!currentEnemyPort)
            {
                isVictory = true;
                EndCampaign();
                return;
            }
            else
            {
                UpdatePortState(currentEnemyPort, BaseCharacter.Owner.enemy);
                currentEnemyPort.SetVisualAsTarget(true, BaseCharacter.Owner.enemy);
            }

            if (!currentPlayerPort)
            {
                isVictory = false;
                EndCampaign();
                return;
            }

            UpdateSettlementsLists();
            PlaceCameraBetweenPorts();
            PlaceStartGameModelButtonBetweenPorts();
            SetGameStateAsWorld();

            UpdatePlayerShips();
            UpdateEnemyShips();
            UpdateRevenues();

            startGameModelButton.gameObject.SetActive(true);

            gameDataSaver.SaveGameData();
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
                for (int i = 0; i < target.Island.possibleTargets.Count; i++)
                    if (target.Island.possibleTargets[i].owner == BaseCharacter.Owner.player)
                        return target.Island.possibleTargets[i].ports[0];
            }
            else
            {
                for (int i = target.Island.possibleTargets.Count - 1; i >= 0; i--)
                    if (target.Island.possibleTargets[i].owner == BaseCharacter.Owner.player)
                        return target.Island.possibleTargets[i].ports[0];
            }

            return port;
        }

        public void StartBattle()
        {
            UpdateSettlementsLists();
            SetGameStateAsBattle();
            PlaceCameraBetweenPorts();
            startGameModelButton.gameObject.SetActive(false);
        }

        public void EndBattle()
        {
            ObjectsManager.Instance.Init();
            gameDataSaver.SaveGameData();
            UpdateSettlementsLists();
            SetGameStateAsWorld();
            startGameModelButton.gameObject.SetActive(false);
        }

        public void EndCampaign()
        {
            campaignIsEnded = true;

            startGameModelButton.gameObject.SetActive(false);
            UIMainCanvas.Instance.ShowEndCampaignWindow();

            gameDataSaver.SaveGameData();
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

        void SetIslandId(Island island)
        {
            int maxId = 0;

            for (int i = 0; i < allIslands.Count; i++)
            {
                if (allIslands[i].islandData.id > maxId)
                    maxId = allIslands[i].islandData.id;
            }

            if (island.islandData.id == -1)
                island.islandData.SetId(maxId + 1);
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
                UpdatePlayerShips();

            if (enemyShipsCount != objectsManager.enemyShips.Count)
                UpdateEnemyShips();

            if (globalTime.currentDay != currentDay)
                UpdateRevenues();
        }

        void UpdatePlayerShips()
        {
            playerMaintenance = 0;

            for (int i = 0; i < objectsManager.playerShips.Count; i++)
                if (objectsManager.playerShips[i])
                    playerMaintenance += objectsManager.playerShips[i].GetComponent<Warship>().maintenance;

            playerShipsCount = (short)objectsManager.playerShips.Count;
        }

        void UpdateEnemyShips()
        {
            enemyMaintenance = 0;

            for (int i = 0; i < objectsManager.enemyShips.Count; i++)
                if (objectsManager.enemyShips[i])
                    enemyMaintenance += objectsManager.enemyShips[i].GetComponent<Warship>().maintenance;

            enemyShipsCount = (short)objectsManager.enemyShips.Count;
        }

        void UpdateRevenues()
        {
            playerRevenue = 0;
            enemyRevenue = 0;

            for (int i = 0; i < playerPorts.Count; i++)
                playerRevenue += playerPorts[i].revenue;

            for (int i = 0; i < playerVillages.Count; i++)
                playerRevenue += playerVillages[i].revenue;

            for (int i = 0; i < enemyPorts.Count; i++)
                enemyRevenue += enemyPorts[i].revenue;

            for (int i = 0; i < enemyVillages.Count; i++)
                enemyRevenue += enemyVillages[i].revenue;

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
    }
}
