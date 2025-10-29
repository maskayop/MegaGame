using UnityEngine;

namespace Vopere
{
	public class DestroyAfterTime : MonoBehaviour
	{
		[SerializeField] float time = 1;
		[SerializeField] bool destroyAtStart = false;

		public void DestroyGameObject()
		{
			Destroy(gameObject, time);
		}

		void Start()
		{
			if (destroyAtStart)
				DestroyGameObject();
		}
	}
}
