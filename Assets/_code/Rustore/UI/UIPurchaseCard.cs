using RuStore.PayClient;
using System;
using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIPurchaseCard : MonoBehaviour, IProductCard<IPurchase>
    {
        [SerializeField] TextMeshProUGUI purchaseIdText;
        [SerializeField] TextMeshProUGUI invoiceIdText;
        [SerializeField] TextMeshProUGUI productIdText;
        [SerializeField] TextMeshProUGUI orderIdText;
        [SerializeField] TextMeshProUGUI amountText;
        [SerializeField] TextMeshProUGUI timeText;
        [SerializeField] TextMeshProUGUI statusText;

        public static event EventHandler OnConfirmPurchase;
        public static event EventHandler OnCancelPurchase;
        public static event EventHandler OnGetPurchase;

        IPurchase purchase = null;

        public void SetData(IPurchase INpurchase)
        {
            purchase = INpurchase;

            if (purchaseIdText != null) purchaseIdText.text = INpurchase.purchaseId.value;
            if (invoiceIdText != null) invoiceIdText.text = INpurchase.invoiceId.value;
            if (orderIdText != null) orderIdText.text = INpurchase.orderId?.value;
            if (amountText != null) amountText.text = INpurchase.amountLabel.value;
            if (timeText != null) timeText.text = BuildLocalDateTimeString(INpurchase.purchaseTime);
            if (statusText != null) statusText.text = INpurchase.status.ToString();

            if (productIdText != null)
            {
                if (INpurchase is ProductPurchase productPurchase) SetProductPurchaseData(productPurchase);
                if (INpurchase is SubscriptionPurchase subscriptionPurchase) SetSubscriptionPurchaseData(subscriptionPurchase);
            }
        }

        void SetProductPurchaseData(ProductPurchase purchase)
        {
            productIdText.text = purchase.productId.value;
        }

        void SetSubscriptionPurchaseData(SubscriptionPurchase purchase)
        {
            productIdText.text = purchase.productId.value;
        }

        string BuildLocalDateTimeString(DateTime? utcDateTime)
        {
            if (utcDateTime == null) return null;

            var localDateTime = utcDateTime.Value.ToLocalTime();

            var offset = TimeZoneInfo.Local.GetUtcOffset(localDateTime);
            var signString = offset >= TimeSpan.Zero ? "+" : "-";
            var utcOffsetString = $"(UTC{signString}{offset:hh\\:mm})";

            var zoneId = new AndroidJavaObject("java.util.TimeZone")
                .CallStatic<AndroidJavaObject>("getDefault")
                .Call<string>("getID");
            var parts = zoneId.Split('/');
            var localZoneName = (parts.Length > 1 ? parts[^1] : zoneId).Replace("_", " ");

            return string.Format("{0} {1} {2}", localDateTime.ToString(), utcOffsetString, localZoneName);
        }

        public IPurchase GetData() => purchase;

        public void ConfirmPurchase() => OnConfirmPurchase?.Invoke(this, EventArgs.Empty);

        public void CancelPurchase() => OnCancelPurchase?.Invoke(this, EventArgs.Empty);

        public void GetPurchase() => OnGetPurchase?.Invoke(this, EventArgs.Empty);
    }
}
