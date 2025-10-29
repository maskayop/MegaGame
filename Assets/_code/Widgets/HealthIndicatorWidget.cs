using UnityEngine;

namespace MegaGame
{
	public class HealthIndicatorWidget : MonoBehaviour
	{
		[SerializeField] MeshRenderer meshRenderer;
		[SerializeField] string floatValueName;

		float value;

		void Update()
		{
			transform.LookAt(CameraController.Instance.mainCamera.transform.position);
			meshRenderer.material.SetFloat(floatValueName, value);
		}

		public void SetValue(float floatValue)
		{
			value = floatValue;
		}
	}
}
