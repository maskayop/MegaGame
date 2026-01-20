using System.Collections.Generic;
using UnityEngine;

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
            playerPort = allIslands[Random.Range(0, allIslands.Count)].ports[0];
            InitPort(playerPort, BaseCharacter.Owner.player);

            enemyPort = FindClosestPortToTargetPort(playerPort);
            InitPort(enemyPort, BaseCharacter.Owner.enemy);

            Vector3 cameraPosition = Vector3.zero;
            cameraPosition += playerPort.transform.position + enemyPort.transform.position;
            cameraPosition /= 2;
            cameraPosition.y = 0;
            CameraController.Instance.transform.position = cameraPosition;

            playerPiastres = 0;
            enemyPiastres = 0;
        }

        void InitPort(Port port, BaseCharacter.Owner owner)
        {
            port.island.owner = owner;
            port.owner = owner;
            port.Init();
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

        Port FindClosestPortToTargetPort(Port target)
        {
            float distance = 1000000;
            Port port = null;

            for (int i = 0; i < allPorts.Count; i++)
            {
                float tempDistance = Vector3.Distance(allPorts[i].transform.position, target.transform.position);

                if (allPorts[i] != target)
                {
                    if (tempDistance < distance)
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
        }

        void UpdateGameState()
        {
            if (!playerPort || !enemyPort)
                return;

            if (playerPort.currentHealth <= 0)
            {
                EndBattle();
                isVictory = false;
            }
            else if (enemyPort.currentHealth <= 0)
            {
                EndBattle();
                isVictory = true;
            }
        }

        public void PrepareNewBattle()
        {
            ObjectsManager.Instance.Init();
        }
    }
}
