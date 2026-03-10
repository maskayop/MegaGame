using MegaGame.UI;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class GameplayObjectsBuilder : MonoBehaviour
    {
        public static GameplayObjectsBuilder Instance { get; private set; }

        [Header("Ships Prices")]
        public short smallShipBuildingPrice = 10;
        public short mediumShipBuildingPrice = 30;
        public short bigShipBuildingPrice = 60;
        public short megaShipBuildingPrice = 100;

        [Header("Buildings Prices")]
        public short traderPrice = 500;
        public short smallPortFortressPrice = 700;
        public short bigPortFortressPrice = 1000;

        string smallShipCost;
        string mediumShipCost;
        string bigShipCost;
        string megaShipCost;

        string traderCost;
        string smallPortFortressCost;
        string bigPortFortressCost;

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

            smallShipCost = Strint.GetString(smallShipBuildingPrice);
            mediumShipCost = Strint.GetString(mediumShipBuildingPrice);
            bigShipCost = Strint.GetString(bigShipBuildingPrice);
            megaShipCost = Strint.GetString(megaShipBuildingPrice);

            traderCost = Strint.GetString(traderPrice);
            smallPortFortressCost = Strint.GetString(smallPortFortressPrice);
            bigPortFortressCost = Strint.GetString(bigPortFortressPrice);
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
                {
                    if (Strint.Subtraction(resourcesController.PlayerMoney, mediumShipCost) < 0)
                        return;

                    shipLevel = 2;
                    BuildShip(scenePrefabsManager.GetDefenderShipPrefab(true), gameController.playerOpposingPorts.protagonPort.transform, targetSettlement);
                }
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
                {
                    if (Strint.Subtraction(resourcesController.EnemyMoney, mediumShipCost) < 0)
                        return;

                    shipLevel = 2;
                    BuildShip(scenePrefabsManager.GetDefenderShipPrefab(false), gameController.enemyOpposingPorts.protagonPort.transform, targetSettlement);
                }
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

            character.SetDestinationSettlementPosition(targetSettlement);

            if (character.owner == BaseCharacter.Owner.player)
                scenePrefabsManager.SpawnAsTargetFX(targetSettlement.transform.position, true);
            else if (character.owner == BaseCharacter.Owner.enemy)
                scenePrefabsManager.SpawnAsTargetFX(targetSettlement.transform.position, false);

            if (character as DefenderShip)
                targetSettlement.Island.DefenderShip = character as DefenderShip;
        }

        public void TryCreatePlayerTraderShip(Transform buildingPosition, out TraderShip outShip)
        {
            GameObject ship = Instantiate(scenePrefabsManager.GetTraderShipPrefab(true), buildingPosition.position, buildingPosition.rotation);
            outShip = ship.GetComponent<TraderShip>();
        }

        public void TryCreateEnemyTraderShip(Transform buildingPosition, out TraderShip outShip)
        {
            GameObject ship = Instantiate(scenePrefabsManager.GetTraderShipPrefab(false), buildingPosition.position, buildingPosition.rotation);
            outShip = ship.GetComponent<TraderShip>();
        }

        public void TryCreatePirateShip(Transform buildingPosition)
        {
            GameObject ship = Instantiate(scenePrefabsManager.GetRandomPirateShipPrefab(), buildingPosition.position, buildingPosition.rotation);
            ship.GetComponent<PirateShip>().HomePosition = buildingPosition;
        }

        short GetBuildingShipLevel(string money, bool isRandom, short maxShipLevel)
        {
            short maxValue = 0;

            if (Strint.GetInt(money) >= Strint.GetInt(smallShipCost) && Strint.GetInt(money) < Strint.GetInt(mediumShipCost))
                maxValue = 1;
            else if (Strint.GetInt(money) >= Strint.GetInt(mediumShipCost) && Strint.GetInt(money) < Strint.GetInt(bigShipCost))
                maxValue = 2;
            else if (Strint.GetInt(money) >= Strint.GetInt(bigShipCost) && Strint.GetInt(money) < Strint.GetInt(megaShipCost))
                maxValue = 3;
            else if (Strint.GetInt(money) >= Strint.GetInt(megaShipCost))
                maxValue = 4;

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

        int GetCurrentBuildingShipCost(short shipLevel)
        {
            if (shipLevel == 1)
                return Strint.GetInt(smallShipCost);
            else if (shipLevel == 2)
                return Strint.GetInt(mediumShipCost);
            else if (shipLevel == 3)
                return Strint.GetInt(bigShipCost);
            else if (shipLevel == 4)
                return Strint.GetInt(megaShipCost);
            else
                return 0;
        }

        public string GetSettlementBuildingCost(short id)
        {
            if (id == 1)
                return traderCost;
            else if (id == 2)
                return smallPortFortressCost;
            else if (id == 3)
                return bigPortFortressCost;
            else
                return "";
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

        public void SpawnNekarkWarningCircle(Warship ship)
        {
            scenePrefabsManager.SpawnNekarkWarningCircle(ship.transform.position);
            UIMainCanvas.Instance.SpawnNekarkMessage();
        }
    }
}
