namespace MegaGame
{
    public class Fortress : BaseSettlement
    {
        protected override void Attack()
        {
            if (Tutorial.Instance)
                if (Tutorial.Instance.isTutorial)
                    if (Tutorial.Instance.currentChapter < 9)
                        return;

            base.Attack();
        }
    }
}
