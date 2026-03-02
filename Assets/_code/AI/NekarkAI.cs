using MegaGame.UI;
using UnityEngine;

namespace MegaGame
{
    public class NekarkAI : MonoBehaviour
    {
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] float portsChanceMultiplier = 2.0f;
        [SerializeField] short speedDrop = 2;

        float currentDecisionTime = 0;

        GameController gameController;
        ObjectsManager objectsManager;

        Warship currentVictim;

        void Start()
        {
            gameController = GameController.Instance;
            objectsManager = ObjectsManager.Instance;
        }

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                return;

            currentDecisionTime -= Time.deltaTime;

            if (currentDecisionTime <= 0)
            {
                currentDecisionTime = timeForDecision;
                MakeDecision();
            }
        }

        void MakeDecision()
        {
            short owner = (short)Random.Range(0, 2);

            if (owner != 0)
            {
                short r = (short)Random.Range(0, objectsManager.playerShips.Count);

                if (objectsManager.playerShips.Count != 0)
                {
                    float portsCounts = (gameController.allPorts.Count - gameController.PlayerPortsCount) / portsChanceMultiplier;
                    int chance = Random.Range(0, Mathf.FloorToInt(portsCounts));

                    if (chance == 0)
                        currentVictim = objectsManager.playerShips[r].GetComponent<Warship>();
                    else
                        return;
                }
                else
                    return;
            }
            else
            {
                short r = (short)Random.Range(0, objectsManager.enemyShips.Count);

                if (objectsManager.enemyShips.Count != 0)
                {
                    float portsCounts = (gameController.allPorts.Count - gameController.EnemyPortsCount) / portsChanceMultiplier;
                    int chance = Random.Range(0, Mathf.FloorToInt(portsCounts));

                    if (chance == 0)
                        currentVictim = objectsManager.enemyShips[r].GetComponent<Warship>();
                    else
                        return;
                }
                else
                    return;
            }

            ReleaseTheNekark();
        }

        void ReleaseTheNekark()
        {
            if (!currentVictim)
                return;

            if (!currentVictim.GetAnimationBehavior().CanBeAnimatedByNekark())
                return;

            currentVictim.KilledByNekark = true;

            short droppedSpeed = (short)Mathf.FloorToInt(currentVictim.currentSpeed / speedDrop);
            currentVictim.GetNavMeshAgent().speed = droppedSpeed;
            currentVictim.speed = droppedSpeed;

            currentVictim.Kill();

            if (currentVictim.owner == BaseCharacter.Owner.player)
                UIMainCanvas.Instance.SpawnNekarkMessage();
        }
    }
}
