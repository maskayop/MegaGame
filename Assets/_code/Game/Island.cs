using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class Island : MonoBehaviour
    {
        public enum Owner { player, enemy, neutral, mixed }
        public Owner owner;

        public List<Port> ports = new List<Port>();

        [Header("Prefabs")]
        [SerializeField] GameObject neutralPortPrefab;
        [SerializeField] GameObject playerPortPrefab;
        [SerializeField] GameObject enemyPortPrefab;

        List<Vector3> positions = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();

        void Awake()
        {
            SetThisIslandToPorts();            
        }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            for (int i = 0; i < ports.Count; i++)
            {
                positions.Add(ports[i].transform.position);
                rotations.Add(ports[i].transform.rotation);
            }
        }

        void SetThisIslandToPorts()
        {
            for (int i = 0; i < ports.Count; i++)
                ports[i].island = this;
        }

        public void CreatePorts()
        {
            for (int i = 0; i < ports.Count; i++)
                DestroyImmediate(ports[i].gameObject);

            ports.Clear();

            for (int i = 0; i < positions.Count; i++)
            {
                GameObject go = null;

                if (owner == Owner.neutral)
                    go = Instantiate(neutralPortPrefab, positions[i], rotations[i], transform);
                else if (owner == Owner.player)
                    go = Instantiate(playerPortPrefab, positions[i], rotations[i], transform);
                else if (owner == Owner.enemy)
                    go = Instantiate(enemyPortPrefab, positions[i], rotations[i], transform);

                ports.Add(go.GetComponent<Port>());
            }

            SetThisIslandToPorts();

            if (owner == Owner.player)
                GameController.Instance.playerPort = ports[0];
            else if (owner == Owner.enemy)
                GameController.Instance.enemyPort = ports[0];
        }
    }
}
