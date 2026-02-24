using UnityEngine;

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

        [Header("Defender Ships")]
        [SerializeField] GameObject defenderShipPlayerPrefab;
        [SerializeField] GameObject defenderShipEnemyPrefab;

        [Header("FX")]
        [SerializeField] GameObject FXTargetEnemy;
        [SerializeField] GameObject FXTargetEnemyReject;
        [SerializeField] GameObject FXTargetPlayer;

        [Header("Widgets")]
        [SerializeField] GameObject distanceCircle;
        [SerializeField] GameObject rightTargetCircle;

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

        public GameObject GetAttackingShipPrefab(bool isPlayer, short shipLevel)
        {
            if (isPlayer)
            {
                if (shipLevel == 1)
                    return smallShipPlayerPrefab;
                else if (shipLevel == 2)
                    return mediumShipPlayerPrefab;
                else if (shipLevel == 3)
                    return bigShipPlayerPrefab;
            }
            else
            {
                if (shipLevel == 1)
                    return smallShipEnemyPrefab;
                else if (shipLevel == 2)
                    return mediumShipEnemyPrefab;
                else if (shipLevel == 3)
                    return bigShipEnemyPrefab;
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

        public void SpawnDistanceCircle(Vector3 position, short radius)
        {
            GameObject circle = Instantiate(distanceCircle, position, Quaternion.identity);
            circle.transform.localScale = Vector3.one * radius;
        }

        public void SpawnRightTargetCircle(Vector3 position)
        {
            GameObject circle = Instantiate(rightTargetCircle, position, Quaternion.identity);
        }
    }
}
