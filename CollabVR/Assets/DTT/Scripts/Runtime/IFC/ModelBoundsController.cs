using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

namespace BimViz
{

public class ModelBoundsController : MonoBehaviour
{
    public Vector3 MinPosition { get; private set; } = Vector3.zero;

    public Vector3 MaxPosition { get; private set; } = Vector3.zero;

    public Vector3 Center { get; private set; } = Vector3.zero;

    public float Width => MaxPosition.x - MinPosition.x;

    public float Height => MaxPosition.y - MinPosition.y;

    public float Depth => MaxPosition.z - MinPosition.z;

    public float Diagonal => ( MaxPosition - MinPosition ).magnitude;

    public void OnModelLoaded( GameObject model )
    {
        
        MinPosition = Vector3.positiveInfinity;
        MaxPosition = Vector3.negativeInfinity;

        //foreach ( GameObject modelVisualizer in models )
        {
            GetGlobalBounds( model, out Vector3 min, out Vector3 max );

            MaxPosition = Vector3.Max( max, MaxPosition );
            MinPosition = Vector3.Min( min, MinPosition );
        }

        Vector3 offset = 0.05f * ( MaxPosition - MinPosition );
        MinPosition -= offset;
        MaxPosition += offset;

        Center = ( MinPosition + MaxPosition ) * 0.5f;
        
        Debug.Log( "Bounds: Min: " + MinPosition + " Max: " + MaxPosition + " Center: " + Center + " Width: " + Width + " Height: " + Height + " Depth: " + Depth + " Diagonal: " + Diagonal );
    }

    #region Unity
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube( Center, MaxPosition - MinPosition );
    }

    #endregion

    #region Public

    public static void GetGlobalBounds( GameObject root, out Vector3 min, out Vector3 max )
    {
        Renderer[] renderers = root.GetComponentsInChildren < Renderer >( true );

        min = Vector3.positiveInfinity;
        max = Vector3.negativeInfinity;

        foreach ( Renderer renderer in renderers )
        {
            min = Vector3.Min( min, renderer.bounds.min );
            max = Vector3.Max( max, renderer.bounds.max );
        }
    }

    public static Bounds GetGlobalBounds( GameObject root )
    {
        Renderer[] renderers = root.GetComponentsInChildren < Renderer >();

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;

        foreach ( Renderer renderer in renderers )
        {
            min = Vector3.Min( min, renderer.bounds.min );
            max = Vector3.Max( max, renderer.bounds.max );
        }

        return new Bounds( ( min + max ) * 0.5f, max - min );
    }

    public static void GetBounds( GameObject go, out Vector3 min, out Vector3 max )
    {
        Renderer renderer = go.GetComponent < Renderer >();

        min = Vector3.positiveInfinity;
        max = Vector3.negativeInfinity;

        if ( renderer != null )
        {
            min = Vector3.Min( min, renderer.bounds.min );
            max = Vector3.Max( max, renderer.bounds.max );
        }
    }

    public Vector3 GetGlobalBoundSize(GameObject root)
    {
        Bounds bounds = GetGlobalBounds( root );
        return bounds.size;
    }

    #endregion
}

}
