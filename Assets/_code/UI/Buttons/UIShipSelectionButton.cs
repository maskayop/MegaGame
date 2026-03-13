using UnityEngine;

namespace MegaGame.UI
{
    public class UIShipSelectionButton : MonoBehaviour
    {
        [SerializeField] int id;

        [Header("Visual")]
        [SerializeField] GameObject imageOff;
        [SerializeField] GameObject imageOn;

        UIShipsSelection shipsSelectionUI;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            shipsSelectionUI = UIShipsSelection.Instance;
        }

        public void SetMaxBuildingShip()
        {
            shipsSelectionUI.SetMaxBuildingShip(id);
        }

        public void Select(bool state)
        {
            imageOn.SetActive(state);
            imageOff.SetActive(!state);
        }
    }
}
