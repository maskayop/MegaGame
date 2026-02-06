using UnityEngine;

namespace MegaGame
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] int shipSpawnChance = 1;

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
            gameplayObjectsBuilder.CreateEnemyShip();
        }
    }
}
