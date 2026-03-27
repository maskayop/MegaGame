using RuStore.PayClient;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIProductTypeView : MonoBehaviour
    {
        [SerializeField] Toggle _allTypes;
        [SerializeField] Toggle _consumable;
        [SerializeField] Toggle _nonConsumable;
        [SerializeField] Toggle _subscription;

        ProductType? _state = null;

        public delegate void OnValueChangedEventHandler(object sender, ProductType? e);
        public event OnValueChangedEventHandler onValueChangedEvent;

        void Start()
        {

            _allTypes.onValueChanged.AddListener((isOn) => { if (isOn) SetState(null); });
            _consumable.onValueChanged.AddListener((isOn) => { if (isOn) SetState(ProductType.CONSUMABLE_PRODUCT); });
            _nonConsumable.onValueChanged.AddListener((isOn) => { if (isOn) SetState(ProductType.NON_CONSUMABLE_PRODUCT); });
            _subscription.onValueChanged.AddListener((isOn) => { if (isOn) SetState(ProductType.SUBSCRIPTION); });
        }

        void SetState(ProductType? value)
        {
            _state = value;
            onValueChangedEvent?.Invoke(this, value);
        }

        public ProductType? GetState() => _state;
    }
}
