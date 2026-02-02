using UnityEngine;

namespace MegaGame
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] int shipSpawnChance = 1;

        Port currentPort;

        GameController gameController;
        float currentDecisionTime = 0;

        void Start()
        {
            gameController = GameController.Instance;
        }

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                return;

            currentPort = gameController.enemyPort;

            currentDecisionTime -= Time.deltaTime;

            if (currentDecisionTime <= 0)
            {
                currentDecisionTime = timeForDecision;
                MakeDecision();
            }
        }

        void MakeDecision()
        {
            if (gameController.GetEnemyMoney() >= gameController.smallShipBuildingCost)
            {
                if (currentPort.targetEnemies.Count > 0)
                    SpawnShip();
                else if (gameController.playerPort.currentHealth / gameController.playerPort.health < 0.5f)
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
            gameController.CreateEnemyShip();
        }
    }
}
