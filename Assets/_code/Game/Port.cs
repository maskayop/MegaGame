using UnityEngine;

namespace MegaGame
{
    public class Port : MonoBehaviour
    {
        public enum Owner { player, enemy }

        public Owner owner;

        public void OnClickAction()
        {
            if (owner == Owner.enemy)
                GameController.Instance.CreatePlayerShip();
            else if (owner == Owner.player)
                GameController.Instance.CreateEnemyShip();
        }
    }
}
