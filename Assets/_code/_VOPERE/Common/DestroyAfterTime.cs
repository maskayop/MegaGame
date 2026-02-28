using UnityEngine;

namespace Vopere.Common
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] float time = 1;
        [SerializeField] bool destroyAtStart = false;

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
