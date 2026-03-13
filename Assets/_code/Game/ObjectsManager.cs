using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class ObjectsManager : MonoBehaviour
    {
        public static ObjectsManager Instance { get; private set; }

        public List<GameObject> allShips = new List<GameObject>();
        public List<GameObject> playerShips = new List<GameObject>();
        public List<GameObject> enemyShips = new List<GameObject>();
        public List<GameObject> pirateShips = new List<GameObject>();

        int allCharactersCount = 0;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create ObjectsManager");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (allShips.Count != allCharactersCount)
            {
                playerShips.Clear();
                enemyShips.Clear();
                pirateShips.Clear();

                for (int i = 0; i < allShips.Count; i++)
                {
                    if (allShips[i].GetComponent<Warship>().owner == BaseCharacter.Owner.player)
                        playerShips.Add(allShips[i]);
                    else if (allShips[i].GetComponent<Warship>().owner == BaseCharacter.Owner.enemy)
                        enemyShips.Add(allShips[i]);
                    else if (allShips[i].GetComponent<Warship>().owner == BaseCharacter.Owner.neutral)
                        pirateShips.Add(allShips[i]);
                }
            }

            allCharactersCount = allShips.Count;
        }

        public void Init()
        {
            for (int i = 0; i < allShips.Count; i++)
                Destroy(allShips[i].gameObject);

            allShips.Clear();
        }
    }
}
