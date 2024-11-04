// Copyright (c) 2024  DigitalTwin Technology GmbH
// https://www.digitaltwin.technology/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DTT.Scripts.Runtime.IFC._4DVC
{
    // Unity no serialize Interfaces :(
    public abstract class ITaskDisplayer : MonoBehaviour
    {
        public abstract void Set(TaskDisplayerParameters parameters);
        public abstract Button GetButton();
    }
}