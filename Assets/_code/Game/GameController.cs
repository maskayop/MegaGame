using UnityEngine;

namespace MegaGame
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        [Header("Ports")]
        [SerializeField] Port playerPort;
        [SerializeField] Port enemyPort;

        [Header("Gameplay")]
        [SerializeField] GameObject shipPlayerPrefab;
        [SerializeField] GameObject shipEnemyPrefab;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create GameController");
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
            
        }

        public void CreatePlayerShip()
        {
            BuildShip(shipPlayerPrefab, playerPort.transform, enemyPort.transform);
        }

        public void CreateEnemyShip()
        {
            BuildShip(shipEnemyPrefab, enemyPort.transform, playerPort.transform);
        }

        public void BuildShip(GameObject shipOwner, Transform buildingPosition, Transform targetPosition)
        {
            GameObject ship = Instantiate(shipOwner, buildingPosition.position, buildingPosition.rotation);
            Character character = ship.GetComponent<Character>();
            character.targetPosition = targetPosition;
        }
    }
}
