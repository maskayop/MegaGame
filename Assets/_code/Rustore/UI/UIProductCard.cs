using RuStore.PayClient;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIProductCard : MonoBehaviour, IProductCard<Product>
    {
        [SerializeField] RawImage productRawImage;
        [SerializeField] TextMeshProUGUI productIdText;
        [SerializeField] TextMeshProUGUI productTitleText;
        [SerializeField] TextMeshProUGUI productTypeText;
        [SerializeField] TextMeshProUGUI productAmountText;
        [SerializeField] TextMeshProUGUI productPriceText;

        public static event EventHandler OnBuyProduct;
        public static event EventHandler OnInfoProduct;

        Product product = null;

        public void SetData(Product INproduct)
        {
            product = INproduct;

            StartCoroutine(LoadImage(INproduct.imageUrl.value));

            if (productIdText != null) productIdText.text = INproduct.productId.value;
            if (productTitleText != null) productTitleText.text = INproduct.title.value;
            if (productTypeText != null) productTypeText.text = INproduct.type.ToString();
            if (productAmountText != null) productAmountText.text = INproduct.amountLabel.value;
            if (productPriceText != null) productPriceText.text = INproduct.price?.value.ToString();
        }

        IEnumerator LoadImage(string url)
        {
            if (string.IsNullOrEmpty(url)) yield break;

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                    productRawImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }

        public void TryBuyProduct()
        {
            OnBuyProduct?.Invoke(this, EventArgs.Empty);
        }

        public void InfoProduct()
        {
            OnInfoProduct?.Invoke(this, EventArgs.Empty);
        }

        public Product GetData()
        {
            return product;
        }
    }
}
