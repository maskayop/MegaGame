using UnityEngine;

namespace MegaGame
{
    public class Port : MonoBehaviour
    {
        public enum Owner { player, enemy, neutral }

        public Owner owner;

        [Header("Money")]
        public int piastresPerDay = 1;

        [Header("Health")]
        public float health = 100;
        public float currentHealth = 100;

        int currentDay = 0;

        GlobalTimeController globalTime;

        void Start()
        {
            currentHealth = health;
            globalTime = GlobalTimeController.Instance;
        }

        void Update()
        {
            if (currentHealth < 0)
            {
                Kill();
                return;
            }

            if (globalTime.currentDay != currentDay)
            {
                GameController.Instance.piastres += piastresPerDay;
                currentDay = globalTime.currentDay;
            }
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
