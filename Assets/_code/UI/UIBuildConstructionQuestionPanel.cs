using TMPro;
using UnityEngine;
using Vopere.Common;

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
                priceText.text = Strint.GetInt(settlementConstructions.FortCost).ToString();
            else if (id == 1)
                priceText.text = Strint.GetInt(settlementConstructions.TradeCost).ToString();
        }
    }
}
