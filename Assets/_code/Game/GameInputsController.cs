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

        bool canClick = true;
        public bool CanClick { get { return canClick; } set { canClick = value; } }

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
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
                canClick = false;

            if (!canClick)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = CameraController.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 1000000, 1 << 9))
                {
                    Island island = hit.collider.GetComponent<Island>();

                    Port port = hit.collider.GetComponent<Port>();
                    Village village = hit.collider.GetComponent<Village>();
                    Fortress fortress = hit.collider.GetComponent<Fortress>();

                    if (port)
                        island = port.Island;
                    else if (village)
                        island = village.Island;
                    else if (fortress)
                        island = fortress.Island;

                    if (island && island.owner != BaseCharacter.Owner.player)
                    {
                        if (island.pirateLair)
                            TryCreatePlayerShip(island.pirateLair, true);
                        else if (island.settlement)
                        {
                            if (island.settlement as Port)
                            {
                                port = (Port)island.settlement;

                                if (GetCurrentTutorialChapter() != -1)
                                {
                                    if (GetCurrentTutorialChapter() != 5 && GetCurrentTutorialChapter() != 10)
                                        return;
                                }

                                if (port == gameController.playerOpposingPorts.antagonPort)
                                    TryCreatePlayerShip(gameController.playerOpposingPorts.antagonPort, true);
                                else
                                    gameplayObjectsBuilder.SpawnWrongTargetPortMessage(gameController.playerOpposingPorts.antagonPort, port);

                                return;
                            }

                            if (island.settlement as Village)
                            {
                                village = (Village)island.settlement;

                                if (GetCurrentTutorialChapter() != -1)
                                {
                                    if (GetCurrentTutorialChapter() != 8 && GetCurrentTutorialChapter() != 10)
                                        return;

                                    if (GetCurrentTutorialChapter() == 8 && village != UITutorialWindow.Instance.GetCurrentTargetVillage())
                                        return;
                                }

                                TryCreatePlayerShip(village, true);
                                return;
                            }

                            if (island.settlement as Fortress)
                            {
                                fortress = (Fortress)island.settlement;

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
                        }
                        return;
                    }
                    else
                    {
                        if (island.settlement as Port)
                        {
                            port = (Port)island.settlement;
                            if (tutorial)
                                if (tutorial.isTutorial)
                                    return;

                            currentSelectedPort = port;
                            UISettlementPanel.Instance.Open(port.Island);

                            return;
                        }

                        if (island.settlement as Village)
                        {
                            village = (Village)island.settlement;
                            TryCreatePlayerShip(village, false);
                            return;
                        }
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
