using UnityEngine;

namespace MegaGame.UI
{
    public class UIMessagePanel : MonoBehaviour
    {
        [SerializeField] GameObject warningMessagePrefab;
        [SerializeField] RectTransform container;

        [Header("Messages")]
        [SerializeField] Data_Message tooFarFromPort;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            foreach(Transform t in container.transform)
                Destroy(t.gameObject);
        }

        public void SpawnMessage()
        {
            GameObject mgo = Instantiate(warningMessagePrefab, container);
        }

        public void SpawnTooFarFromPortMessage()
        {
            GameObject mgo = Instantiate(warningMessagePrefab, container);
            mgo.GetComponent<UIMessageObject>().SetText(tooFarFromPort.GetMessageText());
        }
    }
}
