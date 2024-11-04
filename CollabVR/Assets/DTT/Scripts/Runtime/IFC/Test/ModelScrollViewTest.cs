using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Guid = BimViz.Guid;

namespace IFC.Test
{
    public class ModelScrollViewTest : MonoBehaviour
    {
        [SerializeField] private IfcDataRequester _ifcDataRequester;
        [SerializeField] private DisplayDataTest _displayDataTest;
        [SerializeField] private IfcLoaderTest _ifLoaderTest;

        private void Awake()
        {
            _ifLoaderTest.IfcLoadedEvent.AddListener(OnIfcLoaded);
        }

        private void OnDestroy()
        {
            _ifLoaderTest.IfcLoadedEvent.RemoveListener(OnIfcLoaded);
        }

        public void OnIfcLoaded(GameObject root)
        {
            // var content = _button.transform.parent;
            // foreach(Transform child in root.transform.GetChild(0))
            // {
            //     var button = Instantiate(_button, content);
            //     button.gameObject.SetActive(true);
            //     button.gameObject.name = button.GetComponentInChildren<TextMeshProUGUI>().text = child.name;
            //
            //     button.onClick.AddListener(() =>
            //         _ifcDataRequester.RequestData(Guid.GetLongId(child.name), _displayDataTest.DisplayData));
            // }
        }
    }
}