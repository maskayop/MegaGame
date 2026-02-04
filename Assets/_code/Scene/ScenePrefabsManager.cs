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
        [SerializeField] GameObject FXTargetPlayer;

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

        public void SpawnPortAsTargetFX(Vector3 position, bool targetIsEnemy)
        {
            if (targetIsEnemy)
                Instantiate(FXTargetEnemy, position, Quaternion.identity);
            else
                Instantiate(FXTargetPlayer, position, Quaternion.identity);
        }

        public GameObject GetShipPrefab(bool isPlayer)
        {
            if (isPlayer)
                return shipPlayerPrefab;
            else
                return shipEnemyPrefab;
        }
    }
}
