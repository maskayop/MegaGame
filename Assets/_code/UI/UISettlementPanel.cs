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
    }

    public class UISettlementPanel : MonoBehaviour
    {
        public static UISettlementPanel Instance { get; private set; }

        [SerializeField] GameObject settlementPanel;
        [SerializeField] TextMeshProUGUI islandNameText;

        [SerializeField] List<UISettlement> settlements = new List<UISettlement>();

        GameController gameController;

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
                return;
        }

        public void Init()
        {
            gameController = GameController.Instance;
        }

        public void Open(Island island)
        {
            settlementPanel.gameObject.SetActive(true);
            islandNameText.text = island.islandData.islandName.GetLocalizedString();
        }

        public void Close()
        {
            settlementPanel.gameObject.SetActive(false);
        }
    }
}
