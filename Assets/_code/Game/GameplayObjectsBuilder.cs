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
        public short mediumShipBuildingCost = 20;
        public short bigShipBuildingCost = 50;

        string smallShipCost;
        string mediumShipCost;
        string bigShipCost;

        GameController gameController;
        ScenePrefabsManager scenePrefabsManager;
        ResourcesController resourcesController;

        short maxBuildingShip = 0;

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

        public void Init()
        {
            gameController = GameController.Instance;
            scenePrefabsManager = ScenePrefabsManager.Instance;
            resourcesController = ResourcesController.Instance;

            smallShipCost = Strint.GetString(smallShipBuildingCost);
            mediumShipCost = Strint.GetString(mediumShipBuildingCost);
            bigShipCost = Strint.GetString(bigShipBuildingCost);
        }

        public void TryCreatePlayerShip(BaseSettlement targetSettlement, short shipType)
        {
            if (Vector3.Distance(gameController.playerOpposingPorts.protagonPort.transform.position, targetSettlement.transform.position) > gameController.distanceForPossibleTargets)
            {
                SpawnTooFarFromPortMessage(targetSettlement);
                return;
            }

            if (Strint.Subtraction(resourcesController.PlayerMoney, smallShipCost) < 0)
                return;

            short shipLevel = GetBuildingShipLevel(resourcesController.PlayerMoney, false, maxBuildingShip);

            if (shipType == 0) // Attacking Ship
                BuildShip(scenePrefabsManager.GetAttackingShipPrefab(true, shipLevel), gameController.playerOpposingPorts.protagonPort.transform, targetSettlement);
            else if (shipType == 1) // Defender Ship
            {
                if (!targetSettlement.Island.DefenderShip)
                    BuildShip(scenePrefabsManager.GetDefenderShipPrefab(true), gameController.playerOpposingPorts.protagonPort.transform, targetSettlement);
                else if (targetSettlement.Island.DefenderShip && targetSettlement.Island.DefenderShip.owner == BaseCharacter.Owner.enemy)
                    BuildShip(scenePrefabsManager.GetAttackingShipPrefab(true, shipLevel), gameController.playerOpposingPorts.protagonPort.transform, targetSettlement);
                else
                    return;
            }

            resourcesController.RemoveMoneyFromPlayer(GetCurrentBuildingShipCost(shipLevel));
        }

        public void TryCreateEnemyShip(BaseSettlement targetSettlement, short shipType)
        {
            if (!targetSettlement)
                return;

            if (Strint.Subtraction(resourcesController.EnemyMoney, smallShipCost) < 0)
                return;

            short shipLevel = GetBuildingShipLevel(resourcesController.EnemyMoney, true, 3);

            if (shipType == 0) // Attacking Ship
                BuildShip(scenePrefabsManager.GetAttackingShipPrefab(false, shipLevel), gameController.enemyOpposingPorts.protagonPort.transform, targetSettlement);
            else if (shipType == 1) // Defender Ship
            {
                if (!targetSettlement.Island.DefenderShip)
                    BuildShip(scenePrefabsManager.GetDefenderShipPrefab(false), gameController.enemyOpposingPorts.protagonPort.transform, targetSettlement);
                else if (targetSettlement.Island.DefenderShip && targetSettlement.Island.DefenderShip.owner == BaseCharacter.Owner.player)
                    BuildShip(scenePrefabsManager.GetAttackingShipPrefab(false, shipLevel), gameController.enemyOpposingPorts.protagonPort.transform, targetSettlement);
                else
                    return;
            }

            resourcesController.RemoveMoneyFromEnemy(GetCurrentBuildingShipCost(shipLevel));
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

        short GetBuildingShipLevel(string money, bool isRandom, short maxShipLevel)
        {
            short maxValue = 0;

            if (Strint.GetInt(money) >= Strint.GetInt(smallShipCost) && Strint.GetInt(money) < Strint.GetInt(mediumShipCost))
                maxValue = 1;
            else if (Strint.GetInt(money) >= Strint.GetInt(mediumShipCost) && Strint.GetInt(money) < Strint.GetInt(bigShipCost))
                maxValue = 2;
            else if (Strint.GetInt(money) >= Strint.GetInt(bigShipCost))
                maxValue = 3;

            if (maxValue > maxShipLevel)
                maxValue = maxShipLevel;

            if (isRandom)
            {
                short r = (short)Random.Range(1, maxValue + 1);
                return r;
            }
            else
                return maxValue;
        }

        int GetCurrentBuildingShipCost(int shipLevel)
        {
            if (shipLevel == 1)
                return Strint.GetInt(smallShipCost);
            else if (shipLevel == 2)
                return Strint.GetInt(mediumShipCost);
            else if (shipLevel == 3)
                return Strint.GetInt(bigShipCost);
            else
                return 0;
        }

        public void SetMaxBuildingShip(short id)
        {
            maxBuildingShip = (short)(id + 1);
        }

        void SpawnTooFarFromPortMessage(BaseSettlement targetSettlement)
        {
            scenePrefabsManager.SpawnAsTargetReject(targetSettlement.transform.position);
            scenePrefabsManager.SpawnDistanceCircle(gameController.playerOpposingPorts.protagonPort.transform.position, gameController.distanceForPossibleTargets);
            UIMainCanvas.Instance.SpawnTooFarFromPortMessage();
        }

        public void SpawnWrongTargetPortMessage(BaseSettlement rightTargetSettlement, BaseSettlement wrongTargetSettlement)
        {
            scenePrefabsManager.SpawnAsTargetReject(wrongTargetSettlement.transform.position);
            scenePrefabsManager.SpawnRightTargetCircle(gameController.playerOpposingPorts.antagonPort.transform.position);
            UIMainCanvas.Instance.SpawnWrongTargetPortMessage(rightTargetSettlement.Island);
        }
    }
}
