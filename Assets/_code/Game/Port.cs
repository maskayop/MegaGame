using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class Port : MonoBehaviour
    {
        public enum Owner { player, enemy, neutral }
        public Owner owner;

        [SerializeField] HealthIndicatorWidget healthIndicatorWidget;

        [Header("Money")]
        public int piastresPerDay = 1;

        [Header("Health")]
        public float health = 100;
        public float currentHealth = 100;
        public float healthRegeneration = 1;

        [Header("Info")]
        public List<Character> targetEnemies = new List<Character>();

        [HideInInspector] public Island island;

        int currentDay = 0;
        GlobalTimeController globalTime;
        bool isCaptured = false;

        void Start()
        {
            Init();            
        }

        public void Init()
        {
            currentHealth = health;
            isCaptured = false;
            globalTime = GlobalTimeController.Instance;
        }

        void Update()
        {
            if (isCaptured)
                return;

            if (currentHealth < 0)
            {
                Kill();
                return;
            }

            UpdateHealthWidget();
            UpdateProperties();
        }

        public void OnClickAction()
        {
            if (owner == Owner.enemy)
                GameController.Instance.CreatePlayerShip();
        }

        void Kill()
        {
            isCaptured = true;
        }
        
        void UpdateHealthWidget()
        {
            if (!healthIndicatorWidget)
                return;

            if (currentHealth != health)
            {
                healthIndicatorWidget.SetValue(currentHealth / health);
                healthIndicatorWidget.gameObject.SetActive(true);
            }
            else
                healthIndicatorWidget.gameObject.SetActive(false);
        }

        void UpdateProperties()
        {
            if (globalTime.currentDay != currentDay)
            {
                if (owner == Owner.player)
                    GameController.Instance.playerPiastres += piastresPerDay;
                else if (owner == Owner.enemy)
                    GameController.Instance.enemyPiastres += piastresPerDay;

                currentHealth += healthRegeneration;
                currentHealth = Mathf.Clamp(currentHealth, 0, health);

                currentDay = globalTime.currentDay;
            }
        }
    }
}
