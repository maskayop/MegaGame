using UnityEngine;

namespace MegaGame
{
    public class GlobalTimeController : MonoBehaviour
    {
        public static GlobalTimeController Instance { get; private set; }

        public int currentDay = 1;
        public float dayLenght = 10;
        public float currentTime = 0;

        [Space(20)]
        public bool useTimeDeceleration = true;
        [Range(0.001f, 1f)]
        public float multiplier = 0.01f;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create GlobalTimeController");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Update()
        {
            if (GameController.Instance.gameState != GameController.GameState.battle)
                return;

            currentTime += Time.deltaTime;

            if (useTimeDeceleration)
                dayLenght = multiplier * currentDay;

            if (currentTime >= dayLenght)
            {
                currentDay++;
                currentTime = 0;
            }
        }
    }
}
