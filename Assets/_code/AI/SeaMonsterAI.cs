using UnityEngine;

namespace MegaGame
{
    public class SeaMonsterAI : MonoBehaviour
    {
        [SerializeField] bool monsterType;
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] short speedDrop = 2;
        [SerializeField] float navMeshAgentRadiusMultiplier = 2;
        [SerializeField] short minDay = 100;

        [Header("Chances")]
        [SerializeField] short playerChance = 2;
        [SerializeField] short enemyChance = 2;

        [Header("Debug")]
        [SerializeField] bool sleep = false;

        float currentDecisionTime = 0;
        short randomChance = 0;

        GameController gameController;
        ObjectsManager objectsManager;
        GlobalTimeController globalTime;

        Warship currentShip;
        Warship currentVictim;

        void Start()
        {
            gameController = GameController.Instance;
            objectsManager = ObjectsManager.Instance;
            globalTime = GlobalTimeController.Instance;
        }

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                return;

            if (globalTime.currentDay < minDay)
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
            if (sleep)
                return;

            if (objectsManager.allShips.Count == 0)
                return;

            short shipId = (short)Random.Range(0, objectsManager.allShips.Count);
            currentShip = objectsManager.allShips[shipId].GetComponent<Warship>();

            if (currentShip.owner == BaseCharacter.Owner.player)
                randomChance = (short)Random.Range(0, playerChance);
            else
                randomChance = (short)Random.Range(0, enemyChance);

            if (randomChance == 0)
                currentVictim = currentShip;

            if (monsterType)
                ReleaseTheNafaivel();
            else
                ReleaseTheNekark();
        }

        void ReleaseTheNafaivel()
        {
            if (!currentVictim)
                return;

            if (!currentVictim.GetAnimationBehavior())
                return;

            if (!currentVictim.GetAnimationBehavior().CanBeAnimatedByNafaivel())
                return;

            currentVictim.KilledByNafaivel = true;

            currentVictim.GetNavMeshAgent().speed = 0;
            currentVictim.GetNavMeshAgent().radius *= navMeshAgentRadiusMultiplier;
            currentVictim.speed = 0;

            currentVictim.Kill();

            if (currentVictim.owner == BaseCharacter.Owner.player)
                GameplayObjectsBuilder.Instance.SpawnNafaivelWarningCircle(currentVictim);
        }

        void ReleaseTheNekark()
        {
            if (!currentVictim)
                return;

            if (!currentVictim.GetAnimationBehavior())
                return;

            if (!currentVictim.GetAnimationBehavior().CanBeAnimatedByNekark())
                return;

            currentVictim.KilledByNekark = true;

            short droppedSpeed = (short)Mathf.FloorToInt(currentVictim.currentSpeed / speedDrop);
            currentVictim.GetNavMeshAgent().speed = droppedSpeed;
            currentVictim.GetNavMeshAgent().radius *= navMeshAgentRadiusMultiplier;
            currentVictim.speed = droppedSpeed;

            currentVictim.Kill();

            if (currentVictim.owner == BaseCharacter.Owner.player)
                GameplayObjectsBuilder.Instance.SpawnNekarkWarningCircle(currentVictim);
        }
    }
}
