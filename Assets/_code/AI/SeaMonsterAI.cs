using UnityEngine;

namespace MegaGame
{
    public class SeaMonsterAI : MonoBehaviour
    {
        [SerializeField] bool monsterType;
        [SerializeField] int speedDrop = 2;
        [SerializeField] int minDay = 100;
        [SerializeField] float timeForDecision = 1.0f;
        [SerializeField] float navMeshAgentRadiusMultiplier = 2;

        [Header("Chances")]
        [SerializeField] int playerChance = 2;
        [SerializeField] int enemyChance = 2;

        [SerializeField] Data_Item seaMonstersPacificationItem;
        [SerializeField] int chanceWithItemMultiplier = 4;

        [Header("Debug")]
        [SerializeField] bool sleep = false;

        float currentDecisionTime = 0;
        int randomChance = 0;

        GameController gameController;
        ObjectsManager objectsManager;
        GlobalTimeController globalTime;
        GameShop gameShop;

        Warship currentShip;
        Warship currentVictim;

        void Start()
        {
            gameController = GameController.Instance;
            objectsManager = ObjectsManager.Instance;
            globalTime = GlobalTimeController.Instance;
            gameShop = GameShop.Instance;
        }

        void Update()
        {
            if (gameController.gameState != GameController.GameState.battle)
                return;

            if (Tutorial.Instance)
                if (Tutorial.Instance.isTutorial)
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

            int shipId = Random.Range(0, objectsManager.allShips.Count);
            currentShip = objectsManager.allShips[shipId].GetComponent<Warship>();

            if (currentShip.owner == BaseCharacter.Owner.player)
            {
                if (gameShop)
                {
                    if (gameShop.CheckForPurchasing(seaMonstersPacificationItem))
                        randomChance = Random.Range(0, playerChance * chanceWithItemMultiplier);
                    else
                        randomChance = Random.Range(0, playerChance);
                }
            }
            else
                randomChance = Random.Range(0, enemyChance);

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

            int droppedSpeed = Mathf.FloorToInt(currentVictim.currentSpeed / speedDrop);
            currentVictim.GetNavMeshAgent().speed = droppedSpeed;
            currentVictim.GetNavMeshAgent().radius *= navMeshAgentRadiusMultiplier;
            currentVictim.speed = droppedSpeed;

            currentVictim.Kill();

            if (currentVictim.owner == BaseCharacter.Owner.player)
                GameplayObjectsBuilder.Instance.SpawnNekarkWarningCircle(currentVictim);
        }
    }
}
