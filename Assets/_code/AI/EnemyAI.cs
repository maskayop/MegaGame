using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] int shipSpawnChance = 1;
        [SerializeField] int onLowMoneySpawnChanceMultiplier = 1;

        public List<Village> unsafeVillages = new List<Village>();

        List<Port> portsWithoutFort = new List<Port>();
        List<Port> portsWithoutTrader = new List<Port>();

        [Header("Debug")]
        [SerializeField] bool canBuildShips = true;

        int villagesCount;

        Port currentPort;

        GameController gameController;
        GameplayObjectsBuilder gameplayObjectsBuilder;
        ResourcesController resourcesController;

        float currentDecisionTime = 0;

        void Start()
        {
            gameController = GameController.Instance;
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;
            resourcesController = ResourcesController.Instance;
        }

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                return;

            currentPort = gameController.enemyOpposingPorts.protagonPort;

            currentDecisionTime -= Time.deltaTime;

            if (currentDecisionTime <= 0)
            {
                currentDecisionTime = timeForDecision;

                if (villagesCount != gameController.EnemyVillagesCount)
                    UpdateUnsafeVillages();

                villagesCount = gameController.EnemyVillagesCount;

                MakeDecision();
            }
        }

        void MakeDecision()
        {
            int buildRandom = Random.Range(0, 2);

            if (buildRandom == 0)
            {
                if (Strint.Subtraction(resourcesController.EnemyMoney, gameplayObjectsBuilder.GetSettlementBuildingCost(1)) >= 0)
                {
                    portsWithoutFort.Clear();
                    portsWithoutTrader.Clear();

                    for (int i = 0; i < gameController.enemyPorts.Count; i++)
                    {
                        if (gameController.enemyPorts[i].GetSettlementConstructions())
                        {
                            if (!gameController.enemyPorts[i].GetSettlementConstructions().fortIsBuilt)
                                portsWithoutFort.Add(gameController.enemyPorts[i]);

                            if (!gameController.enemyPorts[i].GetSettlementConstructions().traderIsBuilt)
                                portsWithoutTrader.Add(gameController.enemyPorts[i]);
                        }
                    }

                    int buildType = Random.Range(0, 2);

                    if (buildType == 0)
                        TryBuildFort();
                    else
                        TryBuildTrader();

                    return;
                }
            }

            if (!canBuildShips)
                return;

            if (resourcesController.GetEnemyMoney() >= gameplayObjectsBuilder.GetShipBuildingCost(1))
            {
                if (currentPort.targetEnemies.Count > 0)
                    SpawnShip();
                else
                {
                    int r = -1;

                    if (resourcesController.GetEnemyRevenue() - resourcesController.GetEnemyMaintenance() >= 0)
                        r = Random.Range(0, shipSpawnChance);
                    else
                        r = Random.Range(0, shipSpawnChance * onLowMoneySpawnChanceMultiplier);

                    if (r == 0)
                        SpawnShip();
                }
            }
        }

        void SpawnShip()
        {
            float distanceBetweenProtagonPorts =
                Vector3.Distance(gameController.playerOpposingPorts.protagonPort.transform.position, gameController.enemyOpposingPorts.protagonPort.transform.position);

            if (distanceBetweenProtagonPorts <= gameController.distanceForPossibleTargets)
            {
                int randomVillage = Random.Range(0, 2);

                if (randomVillage == 0 && unsafeVillages.Count != 0)
                {
                    gameplayObjectsBuilder.TryCreateEnemyShip(unsafeVillages[0], false);
                    UpdateUnsafeVillages();
                    return;
                }
            }

            int r = Random.Range(0, gameController.possibleTargetSettlementForEnemy.Count + 1);

            if (r != 0)
                gameplayObjectsBuilder.TryCreateEnemyShip(GetRandomPossibleSettlement(), true);
            else
                gameplayObjectsBuilder.TryCreateEnemyShip(gameController.enemyOpposingPorts.antagonPort, true);
        }

        BaseSettlement GetRandomPossibleSettlement()
        {
            int r = Random.Range(0, gameController.possibleTargetSettlementForEnemy.Count);

            if (gameController.possibleTargetSettlementForEnemy.Count != 0)
                return gameController.possibleTargetSettlementForEnemy[r];
            else
                return null;
        }

        void UpdateUnsafeVillages()
        {
            unsafeVillages.Clear();
            float distance = 0;

            for (int i = 0; i < gameController.enemyVillages.Count; i++)
            {
                distance = Vector3.Distance(gameController.enemyVillages[i].transform.position, gameController.enemyOpposingPorts.protagonPort.transform.position);

                if (distance <= gameController.distanceForPossibleTargets)
                    if (!gameController.enemyVillages[i].Island.DefenderShip ||
                        gameController.enemyVillages[i].Island.DefenderShip && gameController.enemyVillages[i].Island.DefenderShip.owner != BaseCharacter.Owner.enemy)
                        unsafeVillages.Add(gameController.enemyVillages[i]);
            }
        }

        void TryBuildFort()
        {
            if (portsWithoutFort.Count == 0)
                return;

            int r = Random.Range(0, portsWithoutFort.Count);
            portsWithoutFort[r].GetSettlementConstructions().TryBuildFort();
        }

        void TryBuildTrader()
        {
            if (portsWithoutTrader.Count == 0)
                return;

            int r = Random.Range(0, portsWithoutTrader.Count);
            portsWithoutTrader[r].GetSettlementConstructions().TryBuildTrader();
        }
    }
}
