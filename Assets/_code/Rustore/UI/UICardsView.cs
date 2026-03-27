using System.Collections.Generic;
using UnityEngine;

namespace MegaGame.UI
{
    public class UICardsView : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Transform content;

        GameObject[] items = { };

        public void SetData<T>(List<T> data)
        {
            foreach (var i in items)
                Destroy(i);

            var index = 0;
            items = new GameObject[data.Count];

            if (content == null) content = transform;

            foreach (var d in data)
            {
                var view = items[index++] = Instantiate(prefab, content).gameObject;
                view.GetComponent<IProductCard<T>>().SetData(d);
            }
        }
    }
}
