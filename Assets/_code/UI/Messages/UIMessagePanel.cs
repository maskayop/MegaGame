using UnityEngine;
using static MegaGame.BaseCharacter;

namespace MegaGame.UI
{
    public class UIMessagePanel : MonoBehaviour
    {
        [SerializeField] GameObject warningMessagePrefab;
        [SerializeField] RectTransform container;

        [Header("Messages")]
        [SerializeField] Data_Message tooFarFromPort;
        [SerializeField] Data_Message wrongTargetPort;

        [Header("Enemy")]
        [SerializeField] string playerColorFormat = "<color=green>";
        [SerializeField] string enemyColorFormat = "<color=red>";
        [SerializeField] string neutralColorFormat = "<color=yellow>";

        void Start()
        {
            Init();
        }

        public void Init()
        {
            foreach (Transform t in container.transform)
                Destroy(t.gameObject);
        }

        public void SpawnTooFarFromPortMessage()
        {
            GameObject messo = Instantiate(warningMessagePrefab, container);
            messo.GetComponent<UIMessageObject>().SetText(tooFarFromPort.GetMessageText());
        }

        public void SpawnWrongTargetPortMessage(Island target)
        {
            GameObject messo = Instantiate(warningMessagePrefab, container);
            messo.GetComponent<UIMessageObject>().SetText(wrongTargetPort.GetMessageText() + GetTextOwnerColorFormat(target.owner) + target.islandData.islandName.GetLocalizedString());
        }

        string GetTextOwnerColorFormat(Owner targetOwner)
        {
            if (targetOwner == Owner.player)
                return playerColorFormat;
            else if (targetOwner == Owner.enemy)
                return enemyColorFormat;
            else if (targetOwner == Owner.neutral)
                return neutralColorFormat;
            else
                return "<color=white>";
        }
    }
}
