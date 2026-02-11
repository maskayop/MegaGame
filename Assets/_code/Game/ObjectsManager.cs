using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class ObjectsManager : MonoBehaviour
    {
        public static ObjectsManager Instance { get; private set; }

        public List<GameObject> allCharacters = new List<GameObject>();
        public List<GameObject> playerShips = new List<GameObject>();
        public List<GameObject> enemyShips = new List<GameObject>();

        short allCharactersCount = 0;

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
            if (allCharacters.Count != allCharactersCount)
            {
                playerShips.Clear();
                enemyShips.Clear();

                for (int i = 0; i < allCharacters.Count; i++)
                {
                    if (allCharacters[i].GetComponent<Warship>().owner == BaseCharacter.Owner.player)
                        playerShips.Add(allCharacters[i]);
                    else if (allCharacters[i].GetComponent<Warship>().owner == BaseCharacter.Owner.enemy)
                        enemyShips.Add(allCharacters[i]);
                }
            }

            allCharactersCount = (short)allCharacters.Count;
        }

        public void Init()
        {
            for (int i = 0; i < allCharacters.Count; i++)
                Destroy(allCharacters[i].gameObject);

            allCharacters.Clear();
        }
    }
}
