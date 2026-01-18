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
            if (!gameController.IsBattle)
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
            if (gameController.enemyPiastres >= gameController.shipCost)
            {
                int r = Random.Range(0, shipSpawnChance);

                if (r == 0)
                    SpawnShip();
            }
        }

        void SpawnShip()
        {
            gameController.CreateEnemyShip();
        }
    }
}
