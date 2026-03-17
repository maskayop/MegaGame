using UnityEngine;
using static MegaGame.GameController;

namespace MegaGame
{
    public class AdditionalSceneObjects : MonoBehaviour
    {
        public static AdditionalSceneObjects Instance;

        [Header("Start Game")]
        [SerializeField] ModelButton startGameModelButton;
        [SerializeField] float offsetY = 0;

        [Header("End Game")]
        [SerializeField] GameObject victoryPanel;
        public AdditionalCamera victoryAdditionalCamera;

        [SerializeField] GameObject defeatPanel;
        public AdditionalCamera defeatAdditionalCamera;

        [Header("Shop")]
        [SerializeField] GameObject shopPanel;
        public AdditionalCamera shopAdditionalCamera;

        GameController gameController;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create AdditionalSceneObjects");
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
            if (gameController.gameState == GameState.menu)
                startGameModelButton.gameObject.SetActive(false);

            if (gameController.CampaignIsEnded)
                return;

            if (gameController.gameState != GameState.battle)
                return;
        }

        public void Init()
        {
            gameController = GameController.Instance;

            HideAllPanels();
        }

        public void ShowVictoryPanel(bool state)
        {
            victoryPanel.SetActive(state);
        }

        public void ShowDefeatPanel(bool state)
        {
            defeatPanel.SetActive(state);
        }

        public void ShowShopPanel(bool state)
        {
            shopPanel.SetActive(state);
        }

        public void HideAllPanels()
        {
            ShowVictoryPanel(false);
            ShowDefeatPanel(false);
            ShowShopPanel(false);
        }

        public void ShowStartGameModelButton(bool state)
        {
            startGameModelButton.gameObject.SetActive(state);
        }

        public void PlaceStartGameModelButtonBetweenPorts()
        {
            if (!startGameModelButton)
                return;

            startGameModelButton.transform.position = gameController.CalculatePositionBetweenPorts();
            startGameModelButton.transform.position += new Vector3(0, offsetY, 0);
        }
    }
}
