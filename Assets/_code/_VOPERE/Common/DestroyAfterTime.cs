using UnityEngine;

namespace Vopere.Common
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public bool destroyAtStart = false;

        [SerializeField] float time = 1;

        void Start()
        {
            if (destroyAtStart)
                DestroyGameObject();
        }

        public void DestroyGameObject()
        {
            Destroy(gameObject, time);
        }

        public void DestroyGameObjectAfterTime(float INtime)
        {
            Destroy(gameObject, INtime);
        }
    }
}
