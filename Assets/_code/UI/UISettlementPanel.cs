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
            {
                Close();
                return;
            }
        }

        public void Init()
        {
            gameController = GameController.Instance;

            CloseAllSettlements();
            Close();
        }

        public void Open(Island island)
        {
            settlementPanel.gameObject.SetActive(true);
            islandNameText.text = island.islandData.islandName.GetLocalizedString();

            if (island.settlement && island.settlement as Port)
                OnPortSelected(island.settlement.GetComponent<Port>().isBigPort);
        }

        public void Close()
        {
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
    }
}
