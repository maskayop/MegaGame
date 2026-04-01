namespace MegaGame
{
    public class Fortress : BaseSettlement
    {
        protected override void Attack()
        {
            if (Tutorial.Instance)
                if (Tutorial.Instance.isTutorial)
                    if (Tutorial.Instance.currentChapter == 6 || Tutorial.Instance.currentChapter == 7 ||
                        Tutorial.Instance.currentChapter == 8)
                        return;

            base.Attack();
        }
    }
}
