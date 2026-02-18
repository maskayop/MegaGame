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
                    Port port = hit.collider.GetComponent<Port>();

                    if (port && port == gameController.playerOpposingPorts.antagonPort)
                        TryCreatePlayerShip(gameController.playerOpposingPorts.antagonPort, 0);

                    Village village = hit.collider.GetComponent<Village>();

                    if (village)
                    {
                        if (village.owner != BaseCharacter.Owner.player)
                            TryCreatePlayerShip(village, 0);
                        else
                            TryCreatePlayerShip(village, 1);
                    }

                    Fortress fortress = hit.collider.GetComponent<Fortress>();

                    if (fortress && fortress.owner != BaseCharacter.Owner.player)
                        TryCreatePlayerShip(fortress, 0);
                }
            }
        }

        public void TryCreatePlayerShip(BaseSettlement targetSettlement, int shipType)
        {
            if (Vector3.Distance(gameController.playerOpposingPorts.protagonPort.transform.position, targetSettlement.transform.position) > gameController.distanceForPossibleTargets)
            {
                scenePrefabsManager.SpawnAsTargetReject(targetSettlement.transform.position);
                scenePrefabsManager.SpawnDistanceCircle(gameController.playerOpposingPorts.protagonPort.transform.position, gameController.distanceForPossibleTargets);
                UIMainCanvas.Instance.SpawnTooFarFromPortMessage();
                return;
            }

            if (Strint.Subtraction(resourcesController.PlayerMoney, smallShipCost) < 0)
                return;

            if (shipType == 0) // Attacking Ship
                BuildShip(scenePrefabsManager.GetAttackingShipPrefab(true), gameController.playerOpposingPorts.protagonPort.transform, targetSettlement);
            else if (shipType == 1) // Defender Ship
            {
                if (!targetSettlement.Island.DefenderShip)
                    BuildShip(scenePrefabsManager.GetDefenderShipPrefab(true), gameController.playerOpposingPorts.protagonPort.transform, targetSettlement);
                else
                    return;
            }

            resourcesController.RemoveMoneyFromPlayer(smallShipBuildingCost);
        }

        public void TryCreateEnemyShip(BaseSettlement targetSettlement, int shipType)
        {
            if (!targetSettlement)
                return;

            if (Strint.Subtraction(resourcesController.EnemyMoney, smallShipCost) < 0)
                return;

            if (shipType == 0) // Attacking Ship
                BuildShip(scenePrefabsManager.GetAttackingShipPrefab(false), gameController.enemyOpposingPorts.protagonPort.transform, targetSettlement);
            else if (shipType == 1) // Defender Ship
            {
                if (!targetSettlement.Island.DefenderShip)
                    BuildShip(scenePrefabsManager.GetDefenderShipPrefab(false), gameController.enemyOpposingPorts.protagonPort.transform, targetSettlement);
                else
                    return;
            }

            resourcesController.RemoveMoneyFromEnemy(smallShipBuildingCost);
        }

        public void BuildShip(GameObject shipObject, Transform buildingPosition, BaseSettlement targetSettlement)
        {
            GameObject ship = Instantiate(shipObject, buildingPosition.position, buildingPosition.rotation);

            Warship character = ship.GetComponent<Warship>();

            character.SetDestinationPosition(targetSettlement);

            if (character.owner == BaseCharacter.Owner.player)
                scenePrefabsManager.SpawnAsTargetFX(targetSettlement.transform.position, true);
            else if (character.owner == BaseCharacter.Owner.enemy)
                scenePrefabsManager.SpawnAsTargetFX(targetSettlement.transform.position, false);

            if (character as DefenderShip)
                targetSettlement.Island.DefenderShip = character as DefenderShip;
        }
    }
}
