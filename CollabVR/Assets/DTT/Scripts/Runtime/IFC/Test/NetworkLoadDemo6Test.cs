using System;
using System.Collections;
using System.Collections.Generic;
using IFC.Test;
using UnityEngine;

public class NetworkLoadDemo6Test : MonoBehaviour
{

    private IfcLoaderTest _ifcLoaderTest;

    private void Awake()
    {
        _ifcLoaderTest = FindObjectOfType < IfcLoaderTest >();
        
        if( _ifcLoaderTest != null )
            _ifcLoaderTest.LoadDemo6ServerRcp();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
