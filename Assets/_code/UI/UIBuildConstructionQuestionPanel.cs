using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIBuildConstructionQuestionPanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI priceText;

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
                if (settlementConstructions.Settlement as Port)
                {
                    if (settlementConstructions.Settlement.GetComponent<Port>().isBigPort)
                        priceText.text = settlementConstructions.GetSettlementBuildingCost(3).ToString();
                    else
                        priceText.text = settlementConstructions.GetSettlementBuildingCost(2).ToString();
                }
            }
            else if (id == 1)
                priceText.text = settlementConstructions.GetSettlementBuildingCost(1).ToString();
        }
    }
}
