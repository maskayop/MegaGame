using UnityEngine;

namespace MegaGame
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] short shipSpawnChance = 1;
        [SerializeField] short onLowMoneySpawnChanceMultiplier = 1;

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
                MakeDecision();
            }
        }

        void MakeDecision()
        {
            if (resourcesController.GetEnemyMoney() >= gameplayObjectsBuilder.smallShipBuildingCost)
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
            short r = (short)Random.Range(0, gameController.possibleTargetSettlementForEnemy.Count + 1);

            if (r != 0)
                gameplayObjectsBuilder.TryCreateEnemyShip(GetRandomPossibleSettlement());
            else
                gameplayObjectsBuilder.TryCreateEnemyShip(gameController.enemyOpposingPorts.antagonPort);
        }

        BaseSettlement GetRandomPossibleSettlement()
        {
            short r = (short)Random.Range(0, gameController.possibleTargetSettlementForEnemy.Count);

            if (gameController.possibleTargetSettlementForEnemy.Count != 0)
                return gameController.possibleTargetSettlementForEnemy[r];
            else
                return null;
        }
    }
}
