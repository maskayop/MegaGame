using UnityEngine;

namespace MegaGame
{
    public class Port : MonoBehaviour
    {
        public enum Owner { player, enemy, neutral }

        public Owner owner;

        [Header("Health")]
        public float health = 100;
        public float currentHealth = 100;

        void Start()
        {
            currentHealth = health;
        }

        void Update()
        {
            if (currentHealth < 0)
                Kill();
        }

        public void OnClickAction()
        {
            if (owner == Owner.enemy)
                GameController.Instance.CreatePlayerShip();
            else if (owner == Owner.player)
                GameController.Instance.CreateEnemyShip();
        }

        void Kill()
        {

        }
    }
}
