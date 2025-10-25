using UnityEngine;
using UnityEngine.AI;

namespace MegaGame
{
    public class Character : MonoBehaviour
    {
        [SerializeField] LayerMask clickableLayers;

        public Transform targetPosition;

        NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();        
        }

        void Update()
        {
            agent.destination = targetPosition.position;

            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayers))
                    SetDestination(hit.point, hit.collider.gameObject);
            }
        }

        void SetDestination(Vector3 clickPoint, GameObject clickedObject)
        {
            targetPosition.position = clickPoint;
        }
    }
}
