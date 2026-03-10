using MegaGame.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static MegaGame.BaseCharacter;

namespace MegaGame
{
    [Serializable]
    public class OpposingPorts
    {
        public Port protagonPort;
        public Port antagonPort;
    }

    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        public enum GameState { world, battle, menu }
        public GameState gameState;

        [Header("Current Fighting Ports")]
        public OpposingPorts playerOpposingPorts;
        public OpposingPorts enemyOpposingPorts;

        [Header("3D Model Button")]
        [SerializeField] float offsetY = 0;

        ModelButton startGameModelButton;

        [Header("Islands and Settlements")]
        public List<Island> allIslands = new List<Island>();

        public List<Port> allPorts = new List<Port>();
        public List<Village> allVillages = new List<Village>();
        public List<Fortress> allFortresses = new List<Fortress>();

        public List<Port> playerPorts = new List<Port>();
        public List<Village> playerVillages = new List<Village>();
        public List<Fortress> playerFortresses = new List<Fortress>();

        public List<Port> enemyPorts = new List<Port>();
        public List<Village> enemyVillages = new List<Village>();
        public List<Fortress> enemyFortresses = new List<Fortress>();

        public List<Port> allPossiblePlayerTargetPorts = new List<Port>();
        public List<Port> allPossibleEnemyTargetPorts = new List<Port>();

        [Header("Distances")]
        public short distanceForPossibleTargets = 100;

        [Header("Enemy's Targets")]
        public List<BaseSettlement> possibleTargetSettlementForEnemy = new List<BaseSettlement>();

        short playerPortsCount;
        public short PlayerPortsCount { get { return playerPortsCount; } }

        short playerVillagesCount;
        public short PlayerVillagesCount { get { return playerVillagesCount; } }

        short playerFortressesCount;
        public short PlayerFortressesCount { get { return playerFortressesCount; } }

        short enemyPortsCount;
        public short EnemyPortsCount { get { return enemyPortsCount; } }

        short enemyVillagesCount;
        public short EnemyVillagesCount { get { return enemyVillagesCount; } }

        short enemyFortressesCount;
        public short EnemyFortressesCount { get { return enemyFortressesCount; } }

        bool isVictory = false;
        public bool IsVictory { get { return isVictory; } set { isVictory = value; } }

        bool campaignIsEnded = false;
        public bool CampaignIsEnded { get { return campaignIsEnded; } set { campaignIsEnded = value; } }

        List<Island> startIslands = new List<Island>();
        List<Island> neutralIslands = new List<Island>();

        CameraController cameraController;
        GameDataSaver gameDataSaver;
        ResourcesController resourcesController;

        short playerStartIslandId;
        public short PlayerStartIslandId { get { return playerStartIslandId; } set { playerStartIslandId = value; } }

        short enemyStartIslandId;
        public short EnemyStartIslandId { get { return enemyStartIslandId; } set { enemyStartIslandId = value; } }

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
            if (gameState == GameState.menu)
                startGameModelButton.gameObject.SetActive(false);

            if (campaignIsEnded)
                return;

            if (gameState != GameState.battle)
                return;

            UpdateGameState();
        }

        public void Init()
        {
            cameraController = CameraController.Instance;

            resourcesController = ResourcesController.Instance;
            resourcesController.Init();

            gameDataSaver = GameDataSaver.Instance;
            gameDataSaver.Init();

            campaignIsEnded = false;

            gameDataSaver.LoadLastAccount();

            startGameModelButton = FindFirstObjectByType<ModelButton>();
        }

        void InitializeScene()
        {
            playerPorts.Clear();
            enemyPorts.Clear();
            startIslands.Clear();
            allPossiblePlayerTargetPorts.Clear();
            allPossibleEnemyTargetPorts.Clear();

            playerOpposingPorts.protagonPort = null;
            playerOpposingPorts.antagonPort = null;

            enemyOpposingPorts.protagonPort = null;
            enemyOpposingPorts.antagonPort = null;

            for (int i = 0; i < allIslands.Count; i++)
            {
                allIslands[i].owner = Owner.neutral;
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
                short randPlayer = (short)UnityEngine.Random.Range(0, startIslands.Count);
                playerStartIslandId = (short)startIslands[randPlayer].islandData.id;

                short randEnemy = (short)UnityEngine.Random.Range(0, startIslands[randPlayer].possibleEnemyStartIsland.Count);
                enemyStartIslandId = (short)startIslands[randPlayer].possibleEnemyStartIsland[randEnemy].islandData.id;
            }

            for (int i = 0; i < allPorts.Count; i++)
            {
                if (allPorts[i].Island.islandData.id == playerStartIslandId)
                    allPorts[i].owner = Owner.player;
                else if (allPorts[i].Island.islandData.id == enemyStartIslandId)
                    allPorts[i].owner = Owner.enemy;
            }

            UpdateSettlementsLists();
            PrepareNewBattle();

            campaignIsEnded = false;
        }

        public void UpdateSettlementsLists()
        {
            playerPorts.Clear();
            enemyPorts.Clear();
            allPossiblePlayerTargetPorts.Clear();
            allPossibleEnemyTargetPorts.Clear();

            for (int i = 0; i < allPorts.Count; i++)
                if (allPorts[i].owner == Owner.player)
                    playerPorts.Add(allPorts[i]);
                else if (allPorts[i].owner == Owner.enemy)
                    enemyPorts.Add(allPorts[i]);

            playerPortsCount = (short)playerPorts.Count;
            enemyPortsCount = (short)enemyPorts.Count;

            for (int i = 0; i < playerPorts.Count; i++)
                for (int p = 0; p < playerPorts[i].Island.possibleTargets.Count; p++)
                    if (playerPorts[i].Island.possibleTargets[p].owner != Owner.player)
                        if (playerPorts[i].Island.possibleTargets[p].settlement as Port)
                            allPossiblePlayerTargetPorts.Add((Port)playerPorts[i].Island.possibleTargets[p].settlement);

            for (int i = 0; i < enemyPorts.Count; i++)
                for (int p = 0; p < enemyPorts[i].Island.possibleTargets.Count; p++)
                    if (enemyPorts[i].Island.possibleTargets[p].owner != Owner.enemy)
                        if (enemyPorts[i].Island.possibleTargets[p].settlement as Port)
                            allPossibleEnemyTargetPorts.Add((Port)enemyPorts[i].Island.possibleTargets[p].settlement);

            playerVillages.Clear();
            enemyVillages.Clear();

            for (int i = 0; i < allVillages.Count; i++)
                if (allVillages[i].owner == Owner.player)
                    playerVillages.Add(allVillages[i]);
                else if (allVillages[i].owner == Owner.enemy)
                    enemyVillages.Add(allVillages[i]);

            playerVillagesCount = (short)playerVillages.Count;
            enemyVillagesCount = (short)enemyVillages.Count;

            playerFortresses.Clear();
            enemyFortresses.Clear();

            for (int i = 0; i < allFortresses.Count; i++)
                if (allFortresses[i].owner == Owner.player)
                    playerFortresses.Add(allFortresses[i]);
                else if (allFortresses[i].owner == Owner.enemy)
                    enemyFortresses.Add(allFortresses[i]);

            playerFortressesCount = (short)playerFortresses.Count;
            enemyFortressesCount = (short)enemyFortresses.Count;

            possibleTargetSettlementForEnemy.Clear();

            if (enemyOpposingPorts.protagonPort)
            {
                for (int i = 0; i < allVillages.Count; i++)
                    if (allVillages[i].owner != Owner.enemy)
                        if (Vector3.Distance(allVillages[i].transform.position,
                            enemyOpposingPorts.protagonPort.transform.position) <= distanceForPossibleTargets)
                            possibleTargetSettlementForEnemy.Add(allVillages[i]);
            }

            if (enemyOpposingPorts.protagonPort)
            {
                for (int i = 0; i < allFortresses.Count; i++)
                    if (allFortresses[i].owner != Owner.enemy)
                        if (Vector3.Distance(allFortresses[i].transform.position,
                            enemyOpposingPorts.protagonPort.transform.position) <= distanceForPossibleTargets)
                            possibleTargetSettlementForEnemy.Add(allFortresses[i]);
            }
        }

        void UpdatePortState(Port port, Owner owner)
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

                if (allIslands[i].owner == Owner.neutral)
                    neutralIslands.Add(allIslands[i]);
            }

            if (PlayerPortsCount == 0)
            {
                isVictory = false;
                EndCampaign();
                return;
            }

            if (enemyPortsCount == 0)
            {
                isVictory = true;
                EndCampaign();
                return;
            }

            CalculateCurrentPorts();

            UpdateSettlementsLists();
            PlaceCameraBetweenPorts();
            PlaceStartGameModelButtonBetweenPorts();
            SetGameStateAsWorld();

            resourcesController.UpdatePlayerMaintenance();
            resourcesController.UpdateEnemyMaintenance();

            startGameModelButton.gameObject.SetActive(true);

            gameDataSaver.LoadAllIslandsCurrentHealth();
            gameDataSaver.SaveGameData();
        }

        void CalculateCurrentPorts()
        {
            CalculateOpposingPorts(playerOpposingPorts);

            if (playerOpposingPorts.antagonPort.owner == Owner.enemy)
            {
                enemyOpposingPorts.protagonPort = playerOpposingPorts.antagonPort;
                enemyOpposingPorts.antagonPort = playerOpposingPorts.protagonPort;

                return;
            }

            CalculateOpposingPorts(enemyOpposingPorts);

            if (enemyOpposingPorts.antagonPort.owner == Owner.player)
            {
                playerOpposingPorts.protagonPort = enemyOpposingPorts.antagonPort;
                playerOpposingPorts.antagonPort = enemyOpposingPorts.protagonPort;
            }

            for (int i = 0; i < allIslands.Count; i++)
                allIslands[i].UpdateIslandState();
        }

        void CalculateOpposingPorts(OpposingPorts fraction)
        {
            short portsCount = 0;
            short startIslandId = 0;
            List<Port> allPossibleTargetPorts = new List<Port>();
            Owner owner = Owner.neutral;

            if (fraction == playerOpposingPorts)
            {
                portsCount = (short)playerPorts.Count;
                startIslandId = playerStartIslandId;
                allPossibleTargetPorts = allPossiblePlayerTargetPorts;
                owner = Owner.player;
            }
            else if (fraction == enemyOpposingPorts)
            {
                portsCount = (short)enemyPorts.Count;
                startIslandId = enemyStartIslandId;
                allPossibleTargetPorts = allPossibleEnemyTargetPorts;
                owner = Owner.enemy;
            }

            if (portsCount <= 1)
            {
                Island currentIsland = null;

                for (int i = 0; i < allIslands.Count; i++)
                    if (allIslands[i].islandData.id == startIslandId)
                        currentIsland = allIslands[i];

                fraction.protagonPort = (Port)currentIsland.settlement;
            }

            short rand = (short)UnityEngine.Random.Range(0, allPossibleTargetPorts.Count);

            if (allPossibleTargetPorts.Count == 0)
                return;

            fraction.antagonPort = allPossibleTargetPorts[rand];

            if (portsCount > 1)
                fraction.protagonPort = FindPossibleProtagonPortToTargetPort(fraction.antagonPort, owner);

            fraction.protagonPort.owner = owner;

            UpdatePortState(fraction.protagonPort, fraction.protagonPort.owner);

            for (int i = 0; i < allIslands.Count; i++)
                allIslands[i].UpdateIslandState();

            fraction.protagonPort.SetVisualAsTarget(true, fraction.protagonPort.owner);
            fraction.antagonPort.SetVisualAsTarget(true, fraction.antagonPort.owner);
        }

        Port FindPossibleProtagonPortToTargetPort(Port target, Owner owner)
        {
            Port port = null;
            short rand = (short)UnityEngine.Random.Range(0, 2);

            if (rand == 0)
            {
                for (int i = 0; i < target.Island.possibleTargets.Count; i++)
                {
                    BaseSettlement settlement = target.Island.possibleTargets[i].settlement;

                    if (target.Island.possibleTargets[i].owner == owner)
                        if (settlement as Port)
                            return (Port)settlement;
                }
            }
            else
            {
                for (int i = target.Island.possibleTargets.Count - 1; i >= 0; i--)
                {
                    BaseSettlement settlement = target.Island.possibleTargets[i].settlement;

                    if (target.Island.possibleTargets[i].owner == owner)
                        if (settlement as Port)
                            return (Port)settlement;
                }
            }

            return port;
        }

        public void StartBattle()
        {
            for (int i = 0; i < allIslands.Count; i++)
                allIslands[i].UpdateIslandState();

            UpdateSettlementsLists();
            SetGameStateAsBattle();
            resourcesController.UpdateRevenues();

            if (playerPortsCount < enemyPortsCount)
                resourcesController.AddBattleStartMoneyToPlayer((short)(enemyPortsCount - playerPortsCount));

            if (playerPortsCount >= enemyPortsCount)
                resourcesController.AddBattleStartMoneyToEnemy((short)(playerPortsCount - enemyPortsCount + 1));

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
            if (!playerOpposingPorts.protagonPort && !enemyOpposingPorts.protagonPort)
                return;

            if (playerOpposingPorts.protagonPort.currentHealth <= 0)
            {
                UpdatePortState(playerOpposingPorts.protagonPort, Owner.enemy);
                isVictory = false;
                EndBattle();
            }
            else if (playerOpposingPorts.antagonPort.currentHealth <= 0)
            {
                UpdatePortState(playerOpposingPorts.antagonPort, Owner.player);
                isVictory = true;
                EndBattle();
            }

            if (enemyOpposingPorts.antagonPort.currentHealth <= 0)
            {
                UpdatePortState(enemyOpposingPorts.antagonPort, Owner.enemy);
                UpdateSettlementsLists();
                CalculateOpposingPorts(enemyOpposingPorts);
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

        public void PlaceCameraBetweenPorts()
        {
            if (!playerOpposingPorts.protagonPort || !playerOpposingPorts.antagonPort)
                return;

            if (!CameraController.Instance)
                return;

            CameraController.Instance.transform.position = CalculatePositionBetweenPorts();
        }

        void PlaceStartGameModelButtonBetweenPorts()
        {
            if (!playerOpposingPorts.protagonPort || !playerOpposingPorts.antagonPort)
                return;

            if (!startGameModelButton)
                return;

            startGameModelButton.transform.position = CalculatePositionBetweenPorts();
            startGameModelButton.transform.position += new Vector3(0, offsetY, 0);
        }

        Vector3 CalculatePositionBetweenPorts()
        {
            Vector3 newPosition = Vector3.zero;
            newPosition += playerOpposingPorts.protagonPort.transform.position + playerOpposingPorts.antagonPort.transform.position;
            newPosition /= 2;
            newPosition.y = 0;

            return newPosition;
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
