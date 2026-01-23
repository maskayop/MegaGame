using MegaGame.UI;
using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        [Header("Money")]
        public int playerPiastres;
        public int enemyPiastres;

        [Header("Prices")]
        public int shipCost = 10;

        [Header("Ports")]
        public Port playerPort;
        public Port enemyPort;

        [Header("Gameplay")]
        [SerializeField] GameObject shipPlayerPrefab;
        [SerializeField] GameObject shipEnemyPrefab;

        [Header("Islands and Ports")]
        public List<Island> allIslands = new List<Island>();
        public List<Port> allPorts = new List<Port>();

        bool isVictory = false;
        public bool IsVictory { get { return isVictory; } }

        bool isBattle = false;
        public bool IsBattle { get { return isBattle; } }

        List<Island> neutralIslands = new List<Island>();

        // Save Load Data
        string islandOwnerFormat = " IO";

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

        void Update()
        {
            SelectObject();
            UpdateGameState();
        }

        public void Init()
        {
            LoadGameData();

            playerPiastres = 0;
            enemyPiastres = 0;

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
            playerPort.SetAsTarget(true, BaseCharacter.Owner.player);

            enemyPort = FindClosestNeutralPortToTargetPort(playerPort);

            if (enemyPort != null)
            {
                UpdatePortState(enemyPort, BaseCharacter.Owner.enemy);
                enemyPort.SetAsTarget(true, BaseCharacter.Owner.enemy);
            }
            else
            {
                EndCampaign();
                return;
            }

            SaveGameData();

            Vector3 cameraPosition = Vector3.zero;
            cameraPosition += playerPort.transform.position + enemyPort.transform.position;
            cameraPosition /= 2;
            cameraPosition.y = 0;
            CameraController.Instance.transform.position = cameraPosition;
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
            if (playerPiastres - shipCost < 0)
                return;

            BuildShip(shipPlayerPrefab, playerPort.transform, enemyPort.transform);

            playerPiastres -= shipCost;
        }

        public void CreateEnemyShip()
        {
            if (enemyPiastres - shipCost < 0)
                return;

            BuildShip(shipEnemyPrefab, enemyPort.transform, playerPort.transform);

            enemyPiastres -= shipCost;
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
        }

        public void EndBattle()
        {
            isBattle = false;
            PrepareNewBattle();
            SaveGameData();
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
                    DataSaveLoad.Instance.Save(allIslands[i].islandData.id + islandOwnerFormat, 0);
                else if (allIslands[i].owner == BaseCharacter.Owner.enemy)
                    DataSaveLoad.Instance.Save(allIslands[i].islandData.id + islandOwnerFormat, 1);
                else if (allIslands[i].owner == BaseCharacter.Owner.neutral)
                    DataSaveLoad.Instance.Save(allIslands[i].islandData.id + islandOwnerFormat, 2);
                else if (allIslands[i].owner == BaseCharacter.Owner.mixed)
                    DataSaveLoad.Instance.Save(allIslands[i].islandData.id + islandOwnerFormat, 3);
            }
        }

        void LoadGameData()
        {
            for (int i = 0; i < allIslands.Count; i++)
            {
                if (DataSaveLoad.Instance.GetSavedInt(allIslands[i].islandData.id + islandOwnerFormat) == 0)
                    allIslands[i].owner = BaseCharacter.Owner.player;
                else if (DataSaveLoad.Instance.GetSavedInt(allIslands[i].islandData.id + islandOwnerFormat) == 1)
                    allIslands[i].owner = BaseCharacter.Owner.enemy;
                else if (DataSaveLoad.Instance.GetSavedInt(allIslands[i].islandData.id + islandOwnerFormat) == 2)
                    allIslands[i].owner = BaseCharacter.Owner.neutral;
                else if (DataSaveLoad.Instance.GetSavedInt(allIslands[i].islandData.id + islandOwnerFormat) == 3)
                    allIslands[i].owner = BaseCharacter.Owner.mixed;
            }
        }
    }
}
