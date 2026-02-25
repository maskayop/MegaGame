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

        }
    }
}
