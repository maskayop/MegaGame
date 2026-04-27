namespace MegaGame.UI
{
    public class UISendSpiesButton : UIBaseSwitchButton
    {
        protected override void OnInit()
        {
            Select(false);
        }

        public override void Select(bool state)
        {
            base.Select(state);

            button.interactable = !state;
        }
    }
}
