using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] short shipSpawnChance = 1;
        [SerializeField] short onLowMoneySpawnChanceMultiplier = 1;

        public List<Village> unsafeVillagesInRadius = new List<Village>();

        List<Port> portsWithoutFort = new List<Port>();
        List<Port> portsWithoutTrader = new List<Port>();

        short villagesCount;

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
                    UpdateUnsafeVillagesInRadius();

                villagesCount = gameController.EnemyVillagesCount;

                MakeDecision();
            }
        }

        void MakeDecision()
        {
            short buildRandom = (short)Random.Range(0, 2);

            if (buildRandom == 0)
            {
                if (Strint.Subtraction(resourcesController.EnemyMoney, gameplayObjectsBuilder.GetSettlementBuildingCost(1)) >= 0)
                {
                    for (short i = 0; i < gameController.enemyPorts.Count; i++)
                    {
                        if (gameController.enemyPorts[i].GetSettlementConstructions())
                        {
                            if (!gameController.enemyPorts[i].GetSettlementConstructions().fortIsBuilt)
                                portsWithoutFort.Add(gameController.enemyPorts[i]);

                            if (!gameController.enemyPorts[i].GetSettlementConstructions().traderIsBuilt)
                                portsWithoutTrader.Add(gameController.enemyPorts[i]);
                        }
                    }

                    short buildType = (short)Random.Range(0, 2);

                    if (buildRandom == 0)
                        TryBuildFort();
                    else
                        TryBuildTrader();

                    return;
                }
            }

            if (resourcesController.GetEnemyMoney() >= gameplayObjectsBuilder.smallShipBuildingPrice)
            {
                if (currentPort.targetEnemies.Count > 0)
                    SpawnShip();
                else
                {
                    short r = -1;

                    if (resourcesController.GetEnemyRevenue() - resourcesController.GetEnemyMaintenance() >= 0)
                        r = (short)Random.Range(0, shipSpawnChance);
                    else
                        r = (short)Random.Range(0, shipSpawnChance * onLowMoneySpawnChanceMultiplier);

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
                short villageR = (short)Random.Range(0, 2);

                if (villageR == 0 && unsafeVillagesInRadius.Count != 0)
                {
                    gameplayObjectsBuilder.TryCreateEnemyShip(unsafeVillagesInRadius[0], 1);
                    UpdateUnsafeVillagesInRadius();
                    return;
                }
            }

            short r = (short)Random.Range(0, gameController.possibleTargetSettlementForEnemy.Count + 1);

            if (r != 0)
                gameplayObjectsBuilder.TryCreateEnemyShip(GetRandomPossibleSettlement(), 0);
            else
                gameplayObjectsBuilder.TryCreateEnemyShip(gameController.enemyOpposingPorts.antagonPort, 0);
        }

        BaseSettlement GetRandomPossibleSettlement()
        {
            short r = (short)Random.Range(0, gameController.possibleTargetSettlementForEnemy.Count);

            if (gameController.possibleTargetSettlementForEnemy.Count != 0)
                return gameController.possibleTargetSettlementForEnemy[r];
            else
                return null;
        }

        void UpdateUnsafeVillagesInRadius()
        {
            unsafeVillagesInRadius.Clear();
            float distance = 0;

            for (int i = 0; i < gameController.enemyVillages.Count; i++)
            {
                distance = Vector3.Distance(gameController.enemyVillages[i].transform.position, gameController.enemyOpposingPorts.protagonPort.transform.position);

                if (distance <= gameController.distanceForPossibleTargets)
                    if (!gameController.enemyVillages[i].Island.DefenderShip || gameController.enemyVillages[i].Island.DefenderShip && gameController.enemyVillages[i].Island.DefenderShip.owner != BaseCharacter.Owner.enemy)
                        unsafeVillagesInRadius.Add(gameController.enemyVillages[i]);
            }
        }

        void TryBuildFort()
        {
            short r = (short)Random.Range(0, portsWithoutFort.Count);
            portsWithoutFort[r].GetSettlementConstructions().TryBuildFort();
        }

        void TryBuildTrader()
        {
            short r = (short)Random.Range(0, portsWithoutFort.Count);
            portsWithoutTrader[r].GetSettlementConstructions().TryBuildTrader();
        }
    }
}
