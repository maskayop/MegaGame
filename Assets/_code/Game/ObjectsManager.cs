using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class ObjectsManager : MonoBehaviour
    {
        public static ObjectsManager Instance { get; private set; }

        public List<GameObject> allCharacters = new List<GameObject>();

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

        public void Init()
        {
            for (int i = 0; i < allCharacters.Count; i++)
                Destroy(allCharacters[i].gameObject);

            allCharacters.Clear();
        }
    }
}
