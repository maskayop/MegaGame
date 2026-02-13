using MegaGame.UI;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class GameplayObjectsBuilder : MonoBehaviour
    {
        public static GameplayObjectsBuilder Instance { get; private set; }

        [Header("Prices")]
        public short smallShipBuildingCost = 10;

        string smallShipCost;

        GameController gameController;
        ScenePrefabsManager scenePrefabsManager;
        ResourcesController resourcesController;

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
            resourcesController = ResourcesController.Instance;

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
                            TryCreatePlayerShip(gameController.currentEnemyPort);

                    Village village = hit.collider.GetComponentInParent<Village>();

                    if (village && village.owner != BaseCharacter.Owner.player)
                        TryCreatePlayerShip(village);

                    Fortress fortress = hit.collider.GetComponentInParent<Fortress>();

                    if (fortress && fortress.owner != BaseCharacter.Owner.player)
                        TryCreatePlayerShip(fortress);
                }
            }
        }

        public void TryCreatePlayerShip(BaseSettlement targetSettlement)
        {
            if (Vector3.Distance(gameController.currentPlayerPort.transform.position, targetSettlement.transform.position) > gameController.distanceForPossibleTargets)
            {
                scenePrefabsManager.SpawnAsTargetReject(targetSettlement.transform.position);
                UIMainCanvas.Instance.SpawnTooFarFromPortMessage();
                return;
            }

            if (Strint.Subtraction(resourcesController.PlayerMoney, smallShipCost) < 0)
                return;

            BuildShip(scenePrefabsManager.GetShipPrefab(true), gameController.currentPlayerPort.transform, targetSettlement);
            resourcesController.RemoveMoneyFromPlayer(smallShipBuildingCost);
        }

        public void TryCreateEnemyShip(BaseSettlement targetSettlement)
        {
            if (Strint.Subtraction(resourcesController.EnemyMoney, smallShipCost) < 0)
                return;

            BuildShip(scenePrefabsManager.GetShipPrefab(false), gameController.currentEnemyPort.transform, targetSettlement);
            resourcesController.RemoveMoneyFromEnemy(smallShipBuildingCost);
        }

        public void BuildShip(GameObject shipOwner, Transform buildingPosition, BaseSettlement targetSettlement)
        {
            GameObject ship = Instantiate(shipOwner, buildingPosition.position, buildingPosition.rotation);
            Warship character = ship.GetComponent<Warship>();
            character.SetDestinationPosition(targetSettlement);

            if (character.owner == BaseCharacter.Owner.player)
                scenePrefabsManager.SpawnAsTargetFX(targetSettlement.transform.position, true);
            else if (character.owner == BaseCharacter.Owner.enemy)
                scenePrefabsManager.SpawnAsTargetFX(targetSettlement.transform.position, false);
        }
    }
}
