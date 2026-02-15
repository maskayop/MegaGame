using UnityEngine;

namespace MegaGame
{
    public class ScenePrefabsManager : MonoBehaviour
    {
        public static ScenePrefabsManager Instance { get; private set; }

        [Header("Ships")]
        [SerializeField] GameObject shipPlayerPrefab;
        [SerializeField] GameObject shipEnemyPrefab;

        [Header("FX")]
        [SerializeField] GameObject FXTargetEnemy;
        [SerializeField] GameObject FXTargetEnemyReject;
        [SerializeField] GameObject FXTargetPlayer;

        [Header("Widgets")]
        [SerializeField] GameObject distanceCircle;

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

        public GameObject GetShipPrefab(bool isPlayer)
        {
            if (isPlayer)
                return shipPlayerPrefab;
            else
                return shipEnemyPrefab;
        }

        public void SpawnDistanceCircle(Vector3 position, short radius)
        {
            GameObject circle = Instantiate(distanceCircle, position, Quaternion.identity);
            circle.transform.localScale = Vector3.one * radius;
        }
    }
}
