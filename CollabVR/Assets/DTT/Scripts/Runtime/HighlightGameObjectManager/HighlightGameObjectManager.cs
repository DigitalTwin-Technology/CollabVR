using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace BimViz.Highlight
{

public class HighlightGameObjectManager : MonoBehaviour
{
    [SerializeField]
    private Shader highlightShader;

    private readonly Dictionary < GameObject, HighlightData > _highlightedObjects = new();

    private void Awake()
    {
        HighlightData[] highlightDatas = Resources.FindObjectsOfTypeAll < HighlightData >();

        foreach ( HighlightData highlightData in highlightDatas )
        {
            highlightData.Create( highlightShader );
        }
    }

    public void HighlightGameObject( GameObject go, HighlightData highlightData, bool overrideHighlight = true )
    {
        if ( !go.TryGetComponent( out Renderer rend ) )
        {
            return;
        }

        if ( _highlightedObjects.ContainsKey( go ) )
        {
            if ( overrideHighlight )
            {
                _highlightedObjects[go].RemoveOutlineMaterial( rend );
                _highlightedObjects.Remove( go );
            }
            else
            {
                return;
            }
        }

        _highlightedObjects.Add( go, highlightData );

        highlightData.AddOutlineMaterial( rend );
    }

    public void SetMultipleSelection( List < GameObject > objects, HighlightData highlightData )
    {
        for ( int i = 0; i < objects.Count; ++i )
        {
            if ( _highlightedObjects.ContainsKey( objects[i] ) )
            {
                continue;
            }

            _highlightedObjects.Add( objects[i], highlightData );
            highlightData.AddOutlineMaterial( objects[i].GetComponent < Renderer >() );
        }
    }

    public void AddToMultipleSelection( GameObject obj, HighlightData highlightData )
    {
        if ( _highlightedObjects.ContainsKey( obj ) )
        {
            return;
        }

        _highlightedObjects.Add( obj, highlightData );

        highlightData.AddOutlineMaterial( obj.GetComponent < Renderer >() );
    }

    public void ResetMultipleSelection()
    {
        foreach ( ( GameObject go, HighlightData data ) in _highlightedObjects )
        {
            data.RemoveOutlineMaterial( go.GetComponent < Renderer >() );
        }

        _highlightedObjects.Clear();
    }

    public void ResetSelection( GameObject go )
    {
        if ( !_highlightedObjects.ContainsKey( go ) )
        {
            return;
        }

        _highlightedObjects[go].RemoveOutlineMaterial( go.GetComponent < Renderer >() );
        _highlightedObjects.Remove( go );
    }

    public void ResetMultipleSelection( HighlightData highlightData )
    {
        List < GameObject > toRemove = new List < GameObject >();

        foreach ( ( GameObject go, HighlightData data ) in _highlightedObjects.Where( x => x.Value == highlightData ) )
        {
            data.RemoveOutlineMaterial( go.GetComponent < Renderer >() );
            toRemove.Add( go );
        }

        foreach ( GameObject go in toRemove )
        {
            _highlightedObjects.Remove( go );
        }
    }
}

}
