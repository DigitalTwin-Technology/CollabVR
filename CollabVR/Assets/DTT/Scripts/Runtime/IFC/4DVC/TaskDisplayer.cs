// Copyright (c) 2024  DigitalTwin Technology GmbH
// https://www.digitaltwin.technology/

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DTT.Scripts.Runtime.IFC._4DVC
{
    public class TaskDisplayer : ITaskDisplayer
    {
        [SerializeField] private TextMeshProUGUI _text;
        public Button _button;
        [SerializeField] private Image _taskTimeRect;
        [SerializeField] private float _rectWidth = 952;
        [SerializeField] private float _relationWidth = 105;

        // Sorry RectTransform.x = 0 and I don't have time
        private float RectWidth => _rectWidth;

        public override void Set(TaskDisplayerParameters parameters)
        {
            _text.text = parameters.Name;
            var taskTimeTransform = _taskTimeRect.GetComponent<RectTransform>();
            taskTimeTransform.anchoredPosition = new Vector2(parameters.NormalizedStart * RectWidth, 0f);
            taskTimeTransform.sizeDelta =
                new Vector2((parameters.NormalizedEnd - parameters.NormalizedStart) * RectWidth * _relationWidth, 0f);
        }

        public override Button GetButton()
        {
            return _button;
        }
    }
}