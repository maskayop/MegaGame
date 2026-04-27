using UnityEngine;

namespace MegaGame.UI
{
    public class UIShipSelectionButton : UIBaseSwitchButton
    {
        [Header("Properties")]
        [SerializeField] int id;

        UIShipsSelection shipsSelectionUI;

        protected override void OnInit()
        {
            shipsSelectionUI = UIShipsSelection.Instance;
        }

        public void SetMaxBuildingShip()
        {
            shipsSelectionUI.SetMaxBuildingShip(id);
        }

        public void SetBuildDefenderShip()
        {
            shipsSelectionUI.SetBuildDefenderShip();
        }

        public void SetBuildShipsX2()
        {
            shipsSelectionUI.SetBuildShipsX2();
        }
    }
}
