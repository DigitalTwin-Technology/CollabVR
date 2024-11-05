using System;
using System.Collections;
using System.Collections.Generic;
using IFC.Test;
using UnityEngine;
using UnityEngine.UI;

public class OnClickButtonBarcelonaOffice : MonoBehaviour
{
    public IFCModelController controller;
    public IfcLoaderTest loader;

    public GameObject globeMenu;
    public GameObject toolsMenu;
    public GameObject affordanceCallout;

    private void Awake()
    {
        Button button = GetComponent < Button >();

        Debug.Log( button );
        
        if( button != null )
        {
            button.onClick.AddListener(TaskOnClick);
            Debug.Log( "Button added Listener" );
        }
    }


    private void TaskOnClick()
    {
        controller.SetActiveInstance( true );
        loader.LoadDemo6();
        globeMenu.SetActive( false );
        toolsMenu.SetActive( true );
        affordanceCallout.SetActive( false );
        
    }
}
