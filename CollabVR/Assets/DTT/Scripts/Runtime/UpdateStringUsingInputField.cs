using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UpdateStringUsingInputField : MonoBehaviour
{

    [SerializeField]
    private InputField _inputField; 
    
    [SerializeField]
    private TMP_Text _text;
    
    
    
    public void UpdateString(TMP_Text tmpText)
    {
        _text.text = tmpText.text;
        _inputField.text = tmpText.text;
    }

    
}
