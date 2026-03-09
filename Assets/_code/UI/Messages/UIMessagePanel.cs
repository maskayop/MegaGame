using UnityEngine;

namespace MegaGame.UI
{
    public class UIMessagePanel : MonoBehaviour
    {
        [SerializeField] RectTransform container;

        [Header("Message Prefabs")]
        [SerializeField] GameObject warningMessagePrefab;
        [SerializeField] GameObject nekarkMessagePrefab;
        [SerializeField] GameObject fortConstructionMessagePrefab;
        [SerializeField] GameObject traderConstructionMessagePrefab;

        [Header("Messages")]
        [SerializeField] Data_Message tooFarFromPort;
        [SerializeField] Data_Message wrongTargetPort;
        [SerializeField] Data_Message nekark;
        [SerializeField] Data_Message fortConstruction;
        [SerializeField] Data_Message traderConstruction;

        UIColors uiColors;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            uiColors = UIColors.Instance;

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
            messo.GetComponent<UIMessageObject>().SetText(wrongTargetPort.GetMessageText() + uiColors.GetTextOwnerColorString(target.owner)
                + target.islandData.islandName.GetLocalizedString());
        }

        public void SpawnNekarkMessage()
        {
            GameObject messo = Instantiate(nekarkMessagePrefab, container);
            messo.GetComponent<UIMessageObject>().SetText(nekark.GetMessageText());
        }

        public void SpawnFortConstructionMessage(Port port)
        {
            GameObject messo = Instantiate(fortConstructionMessagePrefab, container);

            UIMessageObject messageObject = messo.GetComponent<UIMessageObject>();
            messageObject.SetText(uiColors.GetTextOwnerColorString(port.owner) + port.Island.islandData.islandName.GetLocalizedString()
                + " " + uiColors.GetDefaultColorString() + fortConstruction.GetMessageText());
            messageObject.SetImageColor(uiColors.GetTextOwnerColor(port.owner));
        }

        public void SpawnTraderConstructionMessage(Port port)
        {
            GameObject messo = Instantiate(traderConstructionMessagePrefab, container);

            UIMessageObject messageObject = messo.GetComponent<UIMessageObject>();
            messageObject.SetText(uiColors.GetTextOwnerColorString(port.owner) + port.Island.islandData.islandName.GetLocalizedString()
                + " " + uiColors.GetDefaultColorString() + traderConstruction.GetMessageText());
            messageObject.SetImageColor(uiColors.GetTextOwnerColor(port.owner));
        }
    }
}
