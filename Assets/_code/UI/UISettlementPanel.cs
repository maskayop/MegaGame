using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

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
        [SerializeField] TextMeshProUGUI maxHealthRegenerationText;
        [SerializeField] TextMeshProUGUI revenueText;

        GameController gameController;

        int settlementId;
        int constructionId;

        bool isOpen;
        public bool IsOpen { get { return isOpen; } }

        Island currentIsland;
        SettlementConstructions currentSettlementConstructions;

        LocalizedString localizedStringIslandName;

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

        void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        void OnLocaleChanged(Locale newLocale)
        {
            islandNameText.text = localizedStringIslandName.GetLocalizedString();
        }

        public void Open(Island island)
        {
            isOpen = true;

            currentIsland = island;
            currentSettlementConstructions = currentIsland.settlement.GetSettlementConstructions();

            settlementPanel.gameObject.SetActive(true);
            localizedStringIslandName = island.islandData.islandName;
            islandNameText.text = localizedStringIslandName.GetLocalizedString();

            if (island.settlement && island.settlement as Port)
            {
                OnPortSelected(island.settlement.GetComponent<Port>().isBigPort);
                UpdateTexts();
            }

            animator.Play(startAnimationState);

            buildQuestionPanel.gameObject.SetActive(false);
            EnableConstructionButtons(true);
            EnableConstructionImages(false);
        }

        void UpdateTexts()
        {
            maxDamageText.text = currentIsland.settlement.MaxDamage.ToString();
            maxHealthText.text = currentIsland.settlement.MaxHealth.ToString();
            maxHealthRegenerationText.text = currentIsland.settlement.MaxRegeneration.ToString();
            revenueText.text = currentIsland.settlement.revenue.ToString();
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
            settlementId = id;
        }

        public void TryBuildConstruction(int id)
        {
            constructionId = id;

            buildQuestionPanel.gameObject.SetActive(true);
            buildQuestionPanelTransform.position = GetCurrentConstructionTransform().position;

            if (currentIsland && currentIsland.settlement)
                buildQuestionPanel.SetPrices(currentIsland.settlement.GetSettlementConstructions(), id);

            EnableConstructionButtons(false);

            if (constructionId == 0)
                settlements[settlementId].fortImage.SetActive(true);
            else if (constructionId == 1)
                settlements[settlementId].tradeImage.SetActive(true);
        }

        void EnableConstructionButtons(bool state)
        {
            if (!currentIsland || currentIsland && !currentIsland.settlement)
                return;

            for (int i = 0; i < settlements.Count; i++)
            {
                settlements[i].fortButton.SetActive(state);
                settlements[i].tradeButton.SetActive(state);

                if (currentSettlementConstructions.fortIsBuilt)
                    settlements[i].fortButton.SetActive(false);

                if (currentSettlementConstructions.traderIsBuilt)
                    settlements[i].tradeButton.SetActive(false);
            }
        }

        void EnableConstructionImages(bool state)
        {
            if (!currentIsland || currentIsland && !currentIsland.settlement)
                return;

            for (int i = 0; i < settlements.Count; i++)
            {
                settlements[i].fortImage.SetActive(state);
                settlements[i].tradeImage.SetActive(state);

                if (currentSettlementConstructions.fortIsBuilt)
                    settlements[i].fortImage.SetActive(true);

                if (currentSettlementConstructions.traderIsBuilt)
                    settlements[i].tradeImage.SetActive(true);
            }
        }

        public void OnBuildQuestionPanelClosed()
        {
            EnableConstructionButtons(true);
            EnableConstructionImages(false);
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

        public void TryBuildConstruction()
        {
            buildQuestionPanel.Close();

            if (!currentIsland || currentIsland && !currentIsland.settlement)
                return;

            SettlementConstructions settlementConstructions = currentIsland.settlement.GetSettlementConstructions();

            if (constructionId == 0)
                settlementConstructions.TryBuildFort();
            else if (constructionId == 1)
                settlementConstructions.TryBuildTrader();

            UpdateTexts();
        }
    }
}
