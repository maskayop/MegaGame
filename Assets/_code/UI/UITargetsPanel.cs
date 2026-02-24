using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MegaGame.BaseCharacter;
using static MegaGame.GameController;

namespace MegaGame.UI
{
    public class UITargetsPanel : MonoBehaviour
    {
        public static UITargetsPanel Instance { get; private set; }

        [SerializeField] GameObject targetsPanel;

        [Header("Player")]
        [SerializeField] TextMeshProUGUI playerPortText;
        [SerializeField] Image playerPortImage;

        [SerializeField] TextMeshProUGUI playerTargetText;
        [SerializeField] Image playerTargetImage;

        [Header("Enemy")]
        [SerializeField] TextMeshProUGUI enemyPortText;
        [SerializeField] Image enemyPortImage;

        [SerializeField] TextMeshProUGUI enemyTargetText;
        [SerializeField] Image enemyTargetImage;

        [Header("Colors")]
        [SerializeField] Color playerColor = Color.red;
        [SerializeField] Color enemyColor = Color.blue;
        [SerializeField] Color neutralColor = Color.yellow;

        GameController gameController;
        GameState currentGameState;

        RectTransform playerPortTransform;
        RectTransform playerTargetTransform;

        RectTransform enemyPortTransform;
        RectTransform enemyTargetTransform;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UITargetsPanel");
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

            targetsPanel.SetActive(false);

            playerPortTransform = playerPortImage.GetComponent<RectTransform>();
            playerTargetTransform = playerTargetImage.GetComponent<RectTransform>();

            enemyPortTransform = enemyPortImage.GetComponent<RectTransform>();
            enemyTargetTransform = enemyTargetImage.GetComponent<RectTransform>();
        }

        void Update()
        {
            if (gameController.CampaignIsEnded)
                return;

            if (gameController.gameState != currentGameState)
            {
                if (gameController.gameState == GameState.battle)
                    targetsPanel.SetActive(true);
                else
                    targetsPanel.SetActive(false);
            }

            currentGameState = gameController.gameState;

            if (gameController.gameState == GameState.battle)
            {
                UpdateTargetsNames();
                UpdateTargetsFills();
            }
        }

        void UpdateTargetsNames()
        {
            playerPortText.text = gameController.playerOpposingPorts.protagonPort.Island.islandData.islandName.GetLocalizedString();
            playerTargetText.text = gameController.playerOpposingPorts.antagonPort.Island.islandData.islandName.GetLocalizedString();

            playerPortText.color = GetTextOwnerColor(gameController.playerOpposingPorts.protagonPort.Island.owner);
            playerTargetText.color = GetTextOwnerColor(gameController.playerOpposingPorts.antagonPort.Island.owner);

            enemyPortText.text = gameController.enemyOpposingPorts.protagonPort.Island.islandData.islandName.GetLocalizedString();
            enemyTargetText.text = gameController.enemyOpposingPorts.antagonPort.Island.islandData.islandName.GetLocalizedString();

            enemyPortText.color = GetTextOwnerColor(gameController.enemyOpposingPorts.protagonPort.Island.owner);
            enemyTargetText.color = GetTextOwnerColor(gameController.enemyOpposingPorts.antagonPort.Island.owner);
        }

        void UpdateTargetsFills()
        {
            playerPortImage.color = GetTextOwnerColor(gameController.playerOpposingPorts.protagonPort.Island.owner);
            playerTargetImage.color = GetTextOwnerColor(gameController.playerOpposingPorts.antagonPort.Island.owner);

            playerPortTransform.anchorMax = new Vector2(gameController.playerOpposingPorts.protagonPort.Island.settlement.CurrentHealthNormalized, 1);
            playerPortTransform.offsetMax = Vector2.zero;
            playerTargetTransform.anchorMax = new Vector2(gameController.playerOpposingPorts.antagonPort.Island.settlement.CurrentHealthNormalized, 1);
            playerTargetTransform.offsetMax = Vector2.zero;

            enemyPortImage.color = GetTextOwnerColor(gameController.enemyOpposingPorts.protagonPort.Island.owner);
            enemyTargetImage.color = GetTextOwnerColor(gameController.enemyOpposingPorts.antagonPort.Island.owner);

            enemyPortTransform.anchorMax = new Vector2(gameController.enemyOpposingPorts.protagonPort.Island.settlement.CurrentHealthNormalized, 1);
            enemyPortTransform.offsetMax = Vector2.zero;
            enemyTargetTransform.anchorMax = new Vector2(gameController.enemyOpposingPorts.antagonPort.Island.settlement.CurrentHealthNormalized, 1);
            enemyTargetTransform.offsetMax = Vector2.zero;
        }

        public Color GetTextOwnerColor(Owner targetOwner)
        {
            if (targetOwner == Owner.player)
                return playerColor;
            else if (targetOwner == Owner.enemy)
                return enemyColor;
            else if (targetOwner == Owner.neutral)
                return neutralColor;
            else
                return Color.white;
        }
    }
}
