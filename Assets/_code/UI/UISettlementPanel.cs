using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    [Serializable]
    public class UISettlement
    {
        public string name;
        public GameObject settlementPanel;

        public GameObject fortImage;
        public GameObject fortButton;

        public GameObject tradeImage;
        public GameObject tradeButton;

        public bool isBuilt = false;

        public void OpenPanel()
        {
            settlementPanel.SetActive(true);
        }

        public void ClosePanel()
        {
            settlementPanel.SetActive(false);
        }
    }

    public class UISettlementPanel : MonoBehaviour
    {
        public static UISettlementPanel Instance { get; private set; }

        [SerializeField] GameObject settlementPanel;
        [SerializeField] TextMeshProUGUI islandNameText;

        [SerializeField] List<UISettlement> settlements = new List<UISettlement>();

        [SerializeField] UIBuildConstructionQuestionPanel buildQuestionPanel;

        RectTransform buildQuestionPanelTransform;

        [Header("Animation")]
        [SerializeField] Animator animator;
        [SerializeField] string startAnimationState;

        [Header("Characteristics")]
        [SerializeField] TextMeshProUGUI maxDamageText;
        [SerializeField] TextMeshProUGUI maxHealthText;
        [SerializeField] TextMeshProUGUI currentHealthText;

        GameController gameController;

        short settlementId;
        short constructionId;

        bool isOpen;
        public bool IsOpen { get { return isOpen; } }

        Island currentIsland;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UISettlementPanel");
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
            if (gameController.gameState != GameController.GameState.battle)
            {
                Close();
                return;
            }

            if (currentIsland && currentIsland.settlement)
                currentHealthText.text = currentIsland.settlement.currentHealth.ToString();
        }

        public void Init()
        {
            gameController = GameController.Instance;

            buildQuestionPanelTransform = buildQuestionPanel.GetComponent<RectTransform>();

            CloseAllSettlements();
            Close();
        }

        public void Open(Island island)
        {
            isOpen = true;

            currentIsland = island;

            settlementPanel.gameObject.SetActive(true);
            islandNameText.text = island.islandData.islandName.GetLocalizedString();

            if (island.settlement && island.settlement as Port)
            {
                OnPortSelected(island.settlement.GetComponent<Port>().isBigPort);

                maxDamageText.text = currentIsland.settlement.MaxDamage.ToString();
                maxHealthText.text = currentIsland.settlement.MaxHealth.ToString();
            }

            animator.Play(startAnimationState);

            buildQuestionPanel.gameObject.SetActive(false);
            EnableAllConstructionButtons(true);
            EnableAllConstructionImages(false);
        }

        public void Close()
        {
            isOpen = false;

            settlementPanel.gameObject.SetActive(false);
        }

        void CloseAllSettlements()
        {
            for (int i = 0; i < settlements.Count; i++)
                settlements[i].ClosePanel();
        }

        void OnPortSelected(bool isBigPort)
        {
            CloseAllSettlements();

            if (isBigPort)
                settlements[1].OpenPanel();
            else
                settlements[0].OpenPanel();
        }

        public void SelectSettlement(int id)
        {
            settlementId = (short)id;
        }

        public void TryBuildConstruction(int id)
        {
            constructionId = (short)id;

            buildQuestionPanel.gameObject.SetActive(true);
            buildQuestionPanelTransform.position = GetCurrentConstructionTransform().position;

            EnableAllConstructionButtons(false);

            if (constructionId == 0)
                settlements[settlementId].fortImage.SetActive(true);
            else if (constructionId == 1)
                settlements[settlementId].tradeImage.SetActive(true);
        }

        void EnableAllConstructionButtons(bool state)
        {
            for (int i = 0; i < settlements.Count; i++)
            {
                settlements[i].fortButton.SetActive(state);
                settlements[i].tradeButton.SetActive(state);
            }
        }

        void EnableAllConstructionImages(bool state)
        {
            for (int i = 0; i < settlements.Count; i++)
            {
                settlements[i].fortImage.SetActive(state);
                settlements[i].tradeImage.SetActive(state);
            }
        }

        public void OnBuildQuestionPanelClosed()
        {
            EnableAllConstructionButtons(true);
            EnableAllConstructionImages(false);
        }

        RectTransform GetCurrentConstructionTransform()
        {
            if (constructionId == 0)
                return settlements[settlementId].fortButton.GetComponent<RectTransform>();
            else if (constructionId == 1)
                return settlements[settlementId].tradeButton.GetComponent<RectTransform>();
            else
                return null;
        }
    }
}
