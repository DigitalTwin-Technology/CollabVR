using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace IFC.Test
{
    public class DisplayDataTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

        public void DisplayData(Dictionary<string, string> data)
        {
            if(data == null)
                return;
            _textMeshProUGUI.text = string.Join("\n", data.Select(d => $"{d.Key}: {d.Value}"));
        }
    }
}