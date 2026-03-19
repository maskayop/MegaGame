using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class ScenePrefabsManager : MonoBehaviour
    {
        public static ScenePrefabsManager Instance { get; private set; }

        [Header("Attacking Ships")]
        [SerializeField] GameObject smallShipPlayerPrefab;
        [SerializeField] GameObject smallShipEnemyPrefab;

        [SerializeField] GameObject mediumShipPlayerPrefab;
        [SerializeField] GameObject mediumShipEnemyPrefab;

        [SerializeField] GameObject bigShipPlayerPrefab;
        [SerializeField] GameObject bigShipEnemyPrefab;

        [SerializeField] GameObject megaShipPlayerPrefab;
        [SerializeField] GameObject megaShipEnemyPrefab;

        [Header("Defender Ships")]
        [SerializeField] GameObject defenderShipPlayerPrefab;
        [SerializeField] GameObject defenderShipEnemyPrefab;

        [Header("Trader Ships")]
        [SerializeField] GameObject traderShipPlayerPrefab;
        [SerializeField] GameObject traderShipEnemyPrefab;

        [Header("Pirate Ships")]
        [SerializeField] GameObject smallPirateShipPrefab;
        [SerializeField] GameObject mediumPirateShipPrefab;
        [SerializeField] GameObject bigPirateShipPrefab;

        [Header("FX")]
        [SerializeField] GameObject FXTargetEnemy;
        [SerializeField] GameObject FXTargetEnemyReject;
        [SerializeField] GameObject FXTargetPlayer;

        [Header("Widgets")]
        [SerializeField] GameObject distanceCircle;
        [SerializeField] GameObject rightTargetCircle;
        [SerializeField] GameObject nekarkWarningCircle;
        [SerializeField] GameObject nafaivelWarningCircle;
        [SerializeField] GameObject traderProfitWidget;

        [SerializeField] string distanceCircleAnimationState;

        GameObject constantDistanceCircle;

        GameController gameController;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create ScenePrefabsManager");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            gameController = GameController.Instance;

            if (constantDistanceCircle == null)
            {
                constantDistanceCircle = Instantiate(distanceCircle);
                constantDistanceCircle.transform.localScale = Vector3.one * gameController.distanceForPossibleTargets;
                constantDistanceCircle.SetActive(false);

                if (constantDistanceCircle.GetComponent<DestroyAfterTime>())
                    constantDistanceCircle.GetComponent<DestroyAfterTime>().destroyAtStart = false;
            }
        }

        public void SpawnAsTargetFX(Vector3 position, bool targetIsEnemy)
        {
            if (targetIsEnemy)
                Instantiate(FXTargetEnemy, position, Quaternion.identity);
            else
                Instantiate(FXTargetPlayer, position, Quaternion.identity);
        }

        public void SpawnAsTargetReject(Vector3 position)
        {
            Instantiate(FXTargetEnemyReject, position, Quaternion.identity);
        }

        public GameObject GetAttackingShipPrefab(bool isPlayer, int shipLevel)
        {
            if (isPlayer)
            {
                if (shipLevel == 1)
                    return smallShipPlayerPrefab;
                else if (shipLevel == 2)
                    return mediumShipPlayerPrefab;
                else if (shipLevel == 3)
                    return bigShipPlayerPrefab;
                else if (shipLevel == 4)
                    return megaShipPlayerPrefab;
            }
            else
            {
                if (shipLevel == 1)
                    return smallShipEnemyPrefab;
                else if (shipLevel == 2)
                    return mediumShipEnemyPrefab;
                else if (shipLevel == 3)
                    return bigShipEnemyPrefab;
                else if (shipLevel == 4)
                    return megaShipEnemyPrefab;
            }

            return null;
        }

        public GameObject GetDefenderShipPrefab(bool isPlayer)
        {
            if (isPlayer)
                return defenderShipPlayerPrefab;
            else
                return defenderShipEnemyPrefab;
        }

        public GameObject GetTraderShipPrefab(bool isPlayer)
        {
            if (isPlayer)
                return traderShipPlayerPrefab;
            else
                return traderShipEnemyPrefab;
        }

        public GameObject GetRandomPirateShipPrefab()
        {
            int r = Random.Range(0, 3);

            if (r == 0)
                return smallPirateShipPrefab;
            else if (r == 1)
                return mediumPirateShipPrefab;
            else if (r == 2)
                return bigPirateShipPrefab;
            else
                return null;
        }

        public void SpawnDistanceCircle(Vector3 position, int radius)
        {
            GameObject circle = Instantiate(distanceCircle, position, Quaternion.identity);
            circle.transform.localScale = Vector3.one * radius;
        }

        public void SpawnRightTargetCircle(Vector3 position)
        {
            GameObject circle = Instantiate(rightTargetCircle, position, Quaternion.identity);
        }

        public void SpawnNekarkWarningCircle(Vector3 position)
        {
            GameObject circle = Instantiate(nekarkWarningCircle, position, Quaternion.identity);
        }

        public void SpawnNafaivelWarningCircle(Vector3 position)
        {
            GameObject circle = Instantiate(nafaivelWarningCircle, position, Quaternion.identity);
        }

        public void SpawnTraderProfitWidget(Vector3 position, int value)
        {
            if (value == 0)
                return;

            GameObject widget = Instantiate(traderProfitWidget, position, Quaternion.identity);
            widget.GetComponent<ValueWidget>().SetText("+ " + value.ToString());
        }

        public void ShowConstantDistanceCircle()
        {
            constantDistanceCircle.SetActive(true);
            constantDistanceCircle.transform.position = gameController.playerOpposingPorts.protagonPort.transform.position;
            constantDistanceCircle.GetComponent<Animator>().Play(distanceCircleAnimationState);
        }
    }
}
