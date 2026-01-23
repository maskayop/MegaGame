using UnityEngine;
using TMPro;

namespace MegaGame
{
	public class NameWidget : MonoBehaviour
	{
		[SerializeField] TextMeshPro text;
		[SerializeField] float minScale;
		[SerializeField] bool useLookAt = true;

		[Header("Colors")]
		[SerializeField] Color playerColor = Color.white;
		[SerializeField] Color enemyColor = Color.white;
		[SerializeField] Color neutralColor = Color.white;

        void Update()
		{
			if (useLookAt)
				transform.LookAt(CameraController.Instance.mainCamera.transform.position);
			else
			{
				transform.rotation = CameraController.Instance.mainCamera.transform.rotation;
				transform.Rotate(180, 0, 180);
            }

			transform.localScale = Vector3.one * Mathf.Clamp(CameraController.Instance.GetCameraZoom(), minScale, 1.0f);
		}

		public void SetText(string nameText)
		{
            text.text = nameText;
		}

		public void SetColor(BaseCharacter.Owner owner)
		{
			if (owner == BaseCharacter.Owner.player)
				text.color = playerColor;
			else if (owner == BaseCharacter.Owner.enemy)
				text.color = enemyColor;
			else if (owner == BaseCharacter.Owner.neutral)
				text.color = neutralColor;

        }
	}
}
