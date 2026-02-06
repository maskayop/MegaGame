using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class GameplayObjectsBuilder : MonoBehaviour
    {
        public static GameplayObjectsBuilder Instance { get; private set; }

        [Header("Prices")]
        public int smallShipBuildingCost = 10;

        string smallShipCost;

        GameController gameController;
        ScenePrefabsManager scenePrefabsManager;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create GameplayObjectsBuilder");
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
            if (gameController.CampaignIsEnded)
                return;

            if (gameController.gameState != GameController.GameState.battle)
                return;

            SelectObject();
        }

        public void Init()
        {
            gameController = GameController.Instance;
            scenePrefabsManager = ScenePrefabsManager.Instance;

            smallShipCost = Strint.GetString(smallShipBuildingCost);
        }

        void SelectObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = CameraController.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 1000000, 1 << 9))
                {
                    Port port = hit.collider.GetComponentInParent<Port>();

                    if (port && port.owner == BaseCharacter.Owner.enemy)
                        if (port == gameController.currentEnemyPort)
                            CreatePlayerShip(gameController.currentEnemyPort.transform);

                    Village village = hit.collider.GetComponentInParent<Village>();

                    if (village && village.owner != BaseCharacter.Owner.player)
                        CreatePlayerShip(village.transform);
                }
            }
        }

        public void CreatePlayerShip(Transform targetPosition)
        {
            if (Strint.Subtraction(gameController.playerMoney, smallShipCost) < 0)
                return;

            BuildShip(scenePrefabsManager.GetShipPrefab(true), gameController.currentPlayerPort.transform, targetPosition);
            gameController.RemoveMoneyFromPlayer(smallShipBuildingCost);
        }

        public void CreateEnemyShip()
        {
            if (Strint.Subtraction(gameController.enemyMoney, smallShipCost) < 0)
                return;

            BuildShip(scenePrefabsManager.GetShipPrefab(false), gameController.currentEnemyPort.transform, gameController.currentPlayerPort.transform);
            gameController.RemoveMoneyFromEnemy(smallShipBuildingCost);
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
    }
}
