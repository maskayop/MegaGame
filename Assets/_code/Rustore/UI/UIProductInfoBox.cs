using RuStore.PayClient;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIProductInfoBox : UIMessageBox
    {
        [SerializeField] Transform _resizibleView;

        public void Show(Product product)
        {
            var json = RustoreDataSerializer.SerializeToJson(product, true).Trim('{', '}');
            var jsonWithLinks = RustoreJsonLinkExtractor.AddLinksToJson(json);

            Show(message: jsonWithLinks);

            StartCoroutine(WaitAndUpdate());
        }

        IEnumerator WaitAndUpdate()
        {
            yield return new WaitForEndOfFrame();

            LayoutRebuilder.ForceRebuildLayoutImmediate(_resizibleView as RectTransform);
        }
    }
}
