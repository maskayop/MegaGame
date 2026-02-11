using UnityEngine;

namespace MegaGame
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] short shipSpawnChance = 1;
        [SerializeField] short villageTargetChance = 1;

        Port currentPort;

        GameController gameController;
        GameplayObjectsBuilder gameplayObjectsBuilder;

        float currentDecisionTime = 0;

        void Start()
        {
            gameController = GameController.Instance;
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;
        }

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                return;

            currentPort = gameController.currentEnemyPort;

            currentDecisionTime -= Time.deltaTime;

            if (currentDecisionTime <= 0)
            {
                currentDecisionTime = timeForDecision;
                MakeDecision();
            }
        }

        void MakeDecision()
        {
            if (gameController.GetEnemyMoney() >= gameplayObjectsBuilder.smallShipBuildingCost)
            {
                if (currentPort.targetEnemies.Count > 0)
                    SpawnShip();
                else if (gameController.currentPlayerPort.currentHealth / gameController.currentPlayerPort.health < 0.5f)
                    SpawnShip();
                else
                {
                    int r = Random.Range(0, shipSpawnChance);

                    if (r == 0)
                        SpawnShip();
                }
            }
        }

        void SpawnShip()
        {
            int r = Random.Range(0, villageTargetChance);

            if (gameController.possibleTargetVillagesForEnemy.Count == 0)
                r = -1;

            if (r == 0)
                gameplayObjectsBuilder.CreateEnemyShip(GetRandomPossibleVillage());
            else
                gameplayObjectsBuilder.CreateEnemyShip(gameController.currentPlayerPort);
        }

        Village GetRandomPossibleVillage()
        {
            return gameController.possibleTargetVillagesForEnemy[Random.Range(0, gameController.possibleTargetVillagesForEnemy.Count)];
        }
    }
}
