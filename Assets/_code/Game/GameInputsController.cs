using MegaGame.UI;
using UnityEngine;

namespace MegaGame
{
    public class GameInputsController : MonoBehaviour
    {
        public static GameInputsController Instance { get; private set; }

        GameController gameController;
        GameplayObjectsBuilder gameplayObjectsBuilder;
        CameraController cameraController;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create GameInputsController");
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
            if (gameController.CampaignIsEnded)
                return;

            if (gameController.gameState != GameController.GameState.battle)
                return;

            SelectObject();

            if (Input.GetMouseButtonDown(2))
            {
                if (gameController.playerOpposingPorts.protagonPort)
                    cameraController.transform.position = gameController.playerOpposingPorts.protagonPort.transform.position;
                else
                    cameraController.transform.position = Vector3.zero;
            }
        }

        public void Init()
        {
            gameController = GameController.Instance;
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;
            cameraController = CameraController.Instance;
        }

        void SelectObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = CameraController.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 1000000, 1 << 9))
                {
                    Port port = hit.collider.GetComponent<Port>();

                    if (port)
                    {
                        if (port == gameController.playerOpposingPorts.antagonPort)
                            gameplayObjectsBuilder.TryCreatePlayerShip(gameController.playerOpposingPorts.antagonPort, 0);
                        else if (port.owner != BaseCharacter.Owner.player)
                            gameplayObjectsBuilder.SpawnWrongTargetPortMessage(gameController.playerOpposingPorts.antagonPort, port);

                        return;
                    }

                    Village village = hit.collider.GetComponent<Village>();

                    if (village)
                    {
                        if (village.owner != BaseCharacter.Owner.player)
                            gameplayObjectsBuilder.TryCreatePlayerShip(village, 0);
                        else
                            gameplayObjectsBuilder.TryCreatePlayerShip(village, 1);

                        return;
                    }

                    Fortress fortress = hit.collider.GetComponent<Fortress>();

                    if (fortress && fortress.owner != BaseCharacter.Owner.player)
                    {
                        gameplayObjectsBuilder.TryCreatePlayerShip(fortress, 0);
                        return;
                    }
                }
            }
        }
    }
}
