using UnityEngine;

namespace Vopere.Common.UI
{
	public class UIMoveToZeroOnAwake : MonoBehaviour
	{
		[SerializeField] RectTransform rectTransform;

		void Reset()
		{
			rectTransform = transform as RectTransform;
		}

		void Awake()
		{
			rectTransform.anchoredPosition3D = Vector3.zero;
		}
	}
}
