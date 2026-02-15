using UnityEngine;

namespace MegaGame
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] short shipSpawnChance = 1;

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
                    short r = (short)Random.Range(0, shipSpawnChance);

                    if (r == 0)
                        SpawnShip();
                }
            }
        }

        void SpawnShip()
        {
            short r = (short)Random.Range(0, gameController.possibleTargetVillagesForEnemy.Count
                + gameController.possibleTargetFortressesForEnemy.Count + 1);

            if (r != 0)
            {
                short s = (short)Random.Range(0, 2);

                if (s == 0)
                    gameplayObjectsBuilder.TryCreateEnemyShip(GetRandomPossibleVillage());
                else if (s == 1)
                    gameplayObjectsBuilder.TryCreateEnemyShip(GetRandomPossibleFortress());
            }
            else
                gameplayObjectsBuilder.TryCreateEnemyShip(gameController.enemyOpposingPorts.antagonPort);
        }

        Village GetRandomPossibleVillage()
        {
            short r = (short)Random.Range(0, gameController.possibleTargetVillagesForEnemy.Count);

            if (gameController.possibleTargetVillagesForEnemy.Count != 0)
                return gameController.possibleTargetVillagesForEnemy[r];
            else
                return null;
        }

        Fortress GetRandomPossibleFortress()
        {
            short r = (short)Random.Range(0, gameController.possibleTargetFortressesForEnemy.Count);

            if (gameController.possibleTargetFortressesForEnemy.Count != 0)
                return gameController.possibleTargetFortressesForEnemy[r];
            else
                return null;
        }
    }
}
