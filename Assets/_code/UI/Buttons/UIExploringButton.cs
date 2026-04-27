namespace MegaGame.UI
{
    public class UIExploringButton : UIBaseSwitchButton
    {
        GameController gameController;

        bool isSelected;

        protected override void OnInit()
        {
            gameController = GameController.Instance;

            Select(false);
        }

        public void Switch()
        {
            Select(!isSelected);
        }

        public override void Select(bool state)
        {
            base.Select(state);

            isSelected = state;
            gameController.SetGameModeAsExploring(state);
        }
    }
}
