using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CommentSectionController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<string> CommentUploadEvent;
    [SerializeField] private TextMeshProUGUI _inputText;
    

    public void UploadComment()
    {
        CommentUploadEvent.Invoke( _inputText.text  );
    }

    public void EreaseKeyboardText()
    {
        // TODO: Delete old keyboard system
        //_keyboardManager.outputField.text = "";
        _inputText.text = "";
    }

}
