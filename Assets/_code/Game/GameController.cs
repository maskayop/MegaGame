using UnityEngine;

namespace MegaGame
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        [Header("Money")]
        public int playerPiastres;
        public int enemyPiastres;

        [Header("Prices")]
        [SerializeField] int shipCost = 10;

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

        void Update()
        {
            SelectObject();
            UpdateMoney();
        }

        public void Init()
        {
            
        }

        public void CreatePlayerShip()
        {
            if (playerPiastres - shipCost < 0)
                return;

            BuildShip(shipPlayerPrefab, playerPort.transform, enemyPort.transform);

            playerPiastres -= shipCost;
        }

        public void CreateEnemyShip()
        {
            if (enemyPiastres - shipCost < 0)
                return;

            BuildShip(shipEnemyPrefab, enemyPort.transform, playerPort.transform);

            enemyPiastres -= shipCost;
        }

        public void BuildShip(GameObject shipOwner, Transform buildingPosition, Transform targetPosition)
        {
            GameObject ship = Instantiate(shipOwner, buildingPosition.position, buildingPosition.rotation);
            Character character = ship.GetComponent<Character>();
            character.targetPosition = targetPosition;
        }

        void SelectObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = CameraController.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 1000000, 1 << 6))
                {
                    Port port = hit.collider.GetComponentInParent<Port>();

                    if (port)
                        port.OnClickAction();
                }
            }
        }

        void UpdateMoney()
        {

        }
    }
}
