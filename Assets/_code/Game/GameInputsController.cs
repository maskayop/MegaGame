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

        Port currentSelectedPort;
        public Port CurrentSelectedPort { get { return currentSelectedPort; } }

        bool buildShipsX2 = false;
        public bool BuildShipsX2 { get { return buildShipsX2; } set { buildShipsX2 = value; } }

        Tutorial tutorial;

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

            if (gameController.gameState == GameController.GameState.menu)
                return;

            if (UIGameShop.Instance.IsOpen)
                return;

            if (Input.GetMouseButtonDown(2) && !tutorial.isTutorial)
                PlaceCamera();

            if (gameController.gameState != GameController.GameState.battle)
                return;

            if (!UISettlementPanel.Instance.IsOpen)
                SelectObject();
        }

        public void Init()
        {
            gameController = GameController.Instance;
            gameplayObjectsBuilder = GameplayObjectsBuilder.Instance;
            cameraController = CameraController.Instance;
            tutorial = Tutorial.Instance;
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
                        if (GetCurrentTutorialChapter() != -1)
                        {
                            if (GetCurrentTutorialChapter() != 5 && GetCurrentTutorialChapter() != 10)
                                return;
                        }

                        if (port == gameController.playerOpposingPorts.antagonPort)
                            TryCreatePlayerShip(gameController.playerOpposingPorts.antagonPort, true);
                        else if (port.owner != BaseCharacter.Owner.player)
                            gameplayObjectsBuilder.SpawnWrongTargetPortMessage(gameController.playerOpposingPorts.antagonPort, port);
                        else if (port.owner == BaseCharacter.Owner.player)
                        {
                            if (tutorial)
                                if (tutorial.isTutorial)
                                    return;

                            currentSelectedPort = port;
                            UISettlementPanel.Instance.Open(port.Island);
                        }

                        return;
                    }

                    Village village = hit.collider.GetComponent<Village>();

                    if (village)
                    {
                        if (GetCurrentTutorialChapter() != -1)
                        {
                            if (GetCurrentTutorialChapter() != 8 && GetCurrentTutorialChapter() != 10)
                                return;

                            if (GetCurrentTutorialChapter() == 8 && village != UITutorialWindow.Instance.GetCurrentTargetVillage())
                                return;
                        }

                        if (village.owner == BaseCharacter.Owner.player)
                            TryCreatePlayerShip(village, false);
                        else
                            TryCreatePlayerShip(village, true);

                        return;
                    }

                    Fortress fortress = hit.collider.GetComponent<Fortress>();

                    if (fortress && fortress.owner != BaseCharacter.Owner.player)
                    {
                        if (GetCurrentTutorialChapter() != -1)
                        {
                            if (GetCurrentTutorialChapter() != 9)
                                return;

                            if (GetCurrentTutorialChapter() == 9 && fortress != UITutorialWindow.Instance.GetCurrentTargetFortress())
                                return;
                        }

                        TryCreatePlayerShip(fortress, true);
                        return;
                    }

                    Island island = hit.collider.GetComponent<Island>();

                    if (island && island.owner != BaseCharacter.Owner.player)
                    {
                        TryCreatePlayerShip(island.pirateLair, true);
                        return;
                    }
                }
            }
        }

        void TryCreatePlayerShip(BaseSettlement targetSettlement, bool isAttackingShipType)
        {
            gameplayObjectsBuilder.TryCreatePlayerShip(targetSettlement, isAttackingShipType);

            if (BuildShipsX2)
                gameplayObjectsBuilder.TryCreatePlayerShip(targetSettlement, isAttackingShipType);
        }

        void PlaceCameraToCurrentPort()
        {
            if (gameController.playerOpposingPorts.protagonPort)
                cameraController.transform.position = gameController.playerOpposingPorts.protagonPort.transform.position;
            else
                cameraController.transform.position = Vector3.zero;
        }

        public void PlaceCamera()
        {
            if (gameController.gameState == GameController.GameState.battle)
                PlaceCameraToCurrentPort();
            else if (gameController.gameState == GameController.GameState.world)
                gameController.PlaceCameraBetweenPorts();
        }

        int GetCurrentTutorialChapter()
        {
            if (tutorial)
                if (tutorial.isTutorial)
                    return tutorial.currentChapter;

            return -1;
        }
    }
}
