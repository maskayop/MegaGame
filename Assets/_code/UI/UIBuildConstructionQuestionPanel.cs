using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIBuildConstructionQuestionPanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI buildingNameText;
        [SerializeField] TextMeshProUGUI priceText;
        [SerializeField] Button applyButton;

        [Header("Names")]
        [SerializeField] LocalizedString fortName;
        [SerializeField] LocalizedString traderName;

        ResourcesController resourcesController;

        int price;

        void Start()
        {
            resourcesController = ResourcesController.Instance;
        }

        void Update()
        {
            if (resourcesController.GetPlayerMoney() >= price)
                applyButton.interactable = true;
            else
                applyButton.interactable = false;
        }

        public void Close()
        {
            gameObject.SetActive(false);
            UISettlementPanel.Instance.OnBuildQuestionPanelClosed();
        }

        public void BuildConstruction()
        {
            UISettlementPanel.Instance.TryBuildConstruction();
            Close();
        }

        public void SetPrices(SettlementConstructions settlementConstructions, int id)
        {
            if (id == 0)
            {
                SetBuildingName(fortName);

                if (settlementConstructions.Settlement as Port)
                {
                    if (settlementConstructions.Settlement.GetComponent<Port>().isBigPort)
                        price = settlementConstructions.GetSettlementBuildingCost(3);
                    else
                        price = settlementConstructions.GetSettlementBuildingCost(2);
                }
            }
            else if (id == 1)
            {
                SetBuildingName(traderName);
                price = settlementConstructions.GetSettlementBuildingCost(1);
            }

            priceText.text = price.ToString();
        }

        void SetBuildingName(LocalizedString name)
        {
            buildingNameText.text = name.GetLocalizedString();
        }
    }
}
