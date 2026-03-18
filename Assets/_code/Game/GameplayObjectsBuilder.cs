using MegaGame.UI;
using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;
using static MegaGame.BaseCharacter;

namespace MegaGame
{
    public class GameplayObjectsBuilder : MonoBehaviour
    {
        public static GameplayObjectsBuilder Instance { get; private set; }

        [Header("Ships Prices")]
        [SerializeField] int smallShipBuildingPrice = 10;
        [SerializeField] int mediumShipBuildingPrice = 30;
        [SerializeField] int bigShipBuildingPrice = 60;
        [SerializeField] int megaShipBuildingPrice = 100;

        [Header("Enemy Ships Building Limits")]
        [SerializeField] int enemyDefenderShipBuildMinDay = 50;
        [SerializeField] int enemyMediumShipBuildMinDay = 100;
        [SerializeField] int enemyBigShipBuildMinDay = 200;
        [SerializeField] int enemyMegaShipBuildMinDay = 500;

        [Header("Buildings Prices")]
        [SerializeField] int traderPrice = 500;
        [SerializeField] int smallPortFortressPrice = 700;
        [SerializeField] int bigPortFortressPrice = 1000;

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
        GlobalTimeController globalTime;

        int maxBuildingShip = 0;

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
            globalTime = GlobalTimeController.Instance;

            smallShipCost = Strint.GetString(smallShipBuildingPrice);
            mediumShipCost = Strint.GetString(mediumShipBuildingPrice);
            bigShipCost = Strint.GetString(bigShipBuildingPrice);
            megaShipCost = Strint.GetString(megaShipBuildingPrice);

            traderCost = Strint.GetString(traderPrice);
            smallPortFortressCost = Strint.GetString(smallPortFortressPrice);
            bigPortFortressCost = Strint.GetString(bigPortFortressPrice);
        }

        public void TryCreatePlayerShip(BaseSettlement targetSettlement, bool isAttackingShipType)
        {
            if (targetSettlement as Village && targetSettlement.owner == Owner.player) { }
            else if (Vector3.Distance(gameController.playerOpposingPorts.protagonPort.transform.position, targetSettlement.transform.position) > gameController.distanceForPossibleTargets)
            {
                SpawnTooFarFromPortMessage(targetSettlement);
                return;
            }

            if (Strint.Subtraction(resourcesController.PlayerMoney, smallShipCost) < 0)
                return;

            int shipLevel = GetBuildingShipLevel(resourcesController.PlayerMoney, false, maxBuildingShip);

            if (isAttackingShipType) // Attacking Ship
                BuildShip(scenePrefabsManager.GetAttackingShipPrefab(true, shipLevel), gameController.playerOpposingPorts.protagonPort.transform, targetSettlement);
            else if (!isAttackingShipType) // Defender Ship
            {
                if (!targetSettlement.Island.DefenderShip)
                {
                    if (Strint.Subtraction(resourcesController.PlayerMoney, mediumShipCost) < 0)
                        return;

                    shipLevel = 2;
                    BuildShip(scenePrefabsManager.GetDefenderShipPrefab(true), FindClosestPortToVillage((Village)targetSettlement, Owner.player).transform, targetSettlement);
                }
                else if (targetSettlement.Island.DefenderShip && targetSettlement.Island.DefenderShip.owner == Owner.enemy)
                    BuildShip(scenePrefabsManager.GetAttackingShipPrefab(true, shipLevel), gameController.playerOpposingPorts.protagonPort.transform, targetSettlement);
                else
                    return;
            }

            resourcesController.RemoveMoneyFromPlayer(GetShipBuildingCost(shipLevel));
        }

        public void TryCreateEnemyShip(BaseSettlement targetSettlement, bool isAttackingShipType)
        {
            if (!targetSettlement)
                return;

            if (Strint.Subtraction(resourcesController.EnemyMoney, smallShipCost) < 0)
                return;

            int maxShipLevel = 0;

            if (globalTime.currentDay >= 0 && globalTime.currentDay < enemyMediumShipBuildMinDay)
                maxShipLevel = 1;
            else if (globalTime.currentDay >= enemyMediumShipBuildMinDay && globalTime.currentDay < enemyBigShipBuildMinDay)
                maxShipLevel = 2;
            else if (globalTime.currentDay >= enemyBigShipBuildMinDay && globalTime.currentDay < enemyMegaShipBuildMinDay)
                maxShipLevel = 3;
            else if (globalTime.currentDay >= enemyMegaShipBuildMinDay)
                maxShipLevel = 4;

            int currentShipLevel = GetBuildingShipLevel(resourcesController.EnemyMoney, true, maxShipLevel);

            if (isAttackingShipType) // Attacking Ship
                BuildShip(scenePrefabsManager.GetAttackingShipPrefab(false, currentShipLevel), gameController.enemyOpposingPorts.protagonPort.transform, targetSettlement);
            else if (!isAttackingShipType) // Defender Ship
            {
                if (!targetSettlement.Island.DefenderShip)
                {
                    if (Strint.Subtraction(resourcesController.EnemyMoney, mediumShipCost) < 0)
                        return;

                    if (globalTime.currentDay >= enemyDefenderShipBuildMinDay)
                    {
                        currentShipLevel = 2;
                        BuildShip(scenePrefabsManager.GetDefenderShipPrefab(false), FindClosestPortToVillage((Village)targetSettlement, Owner.enemy).transform, targetSettlement);
                    }
                    else
                        return;
                }
                else if (targetSettlement.Island.DefenderShip && targetSettlement.Island.DefenderShip.owner == Owner.player)
                    BuildShip(scenePrefabsManager.GetAttackingShipPrefab(false, currentShipLevel), gameController.enemyOpposingPorts.protagonPort.transform, targetSettlement);
                else
                    return;
            }

            resourcesController.RemoveMoneyFromEnemy(GetShipBuildingCost(currentShipLevel));
        }

        public void BuildShip(GameObject shipObject, Transform buildingPosition, BaseSettlement targetSettlement)
        {
            GameObject ship = Instantiate(shipObject, buildingPosition.position, buildingPosition.rotation);

            Warship character = ship.GetComponent<Warship>();

            character.SetDestinationSettlementPosition(targetSettlement);

            if (character.owner == Owner.player)
            {
                if (targetSettlement as PirateLair)
                    scenePrefabsManager.SpawnAsTargetFX(targetSettlement.Island.transform.position, true);
                else
                    scenePrefabsManager.SpawnAsTargetFX(targetSettlement.transform.position, true);
            }
            else if (character.owner == Owner.enemy)
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

        int GetBuildingShipLevel(string money, bool isRandom, int maxShipLevel)
        {
            int maxValue = 0;

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
                int r = Random.Range(1, maxValue + 1);
                return r;
            }
            else
                return maxValue;
        }

        public int GetShipBuildingCost(int shipLevel)
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

        public string GetSettlementBuildingCost(int id)
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

        public void SetMaxBuildingShip(int id)
        {
            maxBuildingShip = id + 1;
        }

        Port FindClosestPortToVillage(Village targetVillage, Owner owner)
        {
            if (!targetVillage)
                return null;

            if (owner == Owner.player)
                return GetClosestPortToPoint(targetVillage.transform.position, gameController.playerPorts);
            else if (owner == Owner.enemy)
                return GetClosestPortToPoint(targetVillage.transform.position, gameController.enemyPorts);
            else
                return null;
        }

        Port GetClosestPortToPoint(Vector3 targetPosition, List<Port> fractionPorts)
        {
            if (fractionPorts.Count == 0)
                return null;

            float distance = 0;
            float currentDistance = float.MaxValue;
            int id = -1;

            for (int i = 0; i < fractionPorts.Count; i++)
            {
                distance = Vector3.Distance(targetPosition, fractionPorts[i].transform.position);

                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    id = i;
                }
            }

            return fractionPorts[id];
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

        public void SpawnNafaivelWarningCircle(Warship ship)
        {
            scenePrefabsManager.SpawnNafaivelWarningCircle(ship.transform.position);
            UIMainCanvas.Instance.SpawnNafaivelMessage();
        }
    }
}
