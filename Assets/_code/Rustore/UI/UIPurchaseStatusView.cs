#nullable enable

using RuStore.PayClient;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIPurchaseStatusView : MonoBehaviour
    {
        [SerializeField] Toggle? _allStatuses;
        [SerializeField] Toggle? _paid;
        [SerializeField] Toggle? _confirmed;
        [SerializeField] Toggle? _active;
        [SerializeField] Toggle? _paused;

        Enum? _state = null;

        public delegate void OnValueChangedEventHandler(object sender, Enum? e);
        public event OnValueChangedEventHandler? onValueChangedEvent;

        void Start()
        {
            _allStatuses?.onValueChanged.AddListener((isOn) => { if (isOn) SetState(null); });

            _paid?.onValueChanged.AddListener((isOn) => { if (isOn) SetState(ProductPurchaseStatus.PAID); });
            _confirmed?.onValueChanged.AddListener((isOn) => { if (isOn) SetState(ProductPurchaseStatus.CONFIRMED); });

            _active?.onValueChanged.AddListener((isOn) => { if (isOn) SetState(SubscriptionPurchaseStatus.ACTIVE); });
            _paused?.onValueChanged.AddListener((isOn) => { if (isOn) SetState(SubscriptionPurchaseStatus.PAUSED); });
        }

        void SetState(Enum? value)
        {
            _state = value;
            onValueChangedEvent?.Invoke(this, value);
        }

        public Enum? GetState() => _state;
    }
}
