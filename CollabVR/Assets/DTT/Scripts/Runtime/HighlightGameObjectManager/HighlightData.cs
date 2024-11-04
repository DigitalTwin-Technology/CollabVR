using UnityEngine;

namespace BimViz.Highlight
{

[CreateAssetMenu( fileName = "HighlightGameObject", menuName = "ScriptableObject/HighlightGameObject" )]
public class HighlightData : ScriptableObject
{
    [SerializeField]
    private Color m_HighlightColor = Color.yellow;

    [SerializeField]
    private int m_RenderQueue = 3000;

    private Material m_HighlightMaterial;
    private static readonly int s_EmissionColor = Shader.PropertyToID( "_EmissionColor" );

    public Color Color => m_HighlightColor;

    #region Public

    public void Create( Shader shader )
    {
        
        m_HighlightMaterial = new Material( shader ) { renderQueue = m_RenderQueue };
        m_HighlightMaterial.SetColor( s_EmissionColor, m_HighlightColor );
    }

    internal void RemoveOutlineMaterial( Renderer rend )
    {
        if ( rend == null )
        {
            return;
        }

        HighlightObject smo = rend.GetComponent < HighlightObject >();

        if ( smo == null )
        {
            return;
        }

        //TODO: Warning, DestroyImmediate due the split screen that creates a copy on the same frame
        DestroyImmediate( smo.OutlineMeshTransform.gameObject );
        DestroyImmediate( smo );
    }

    internal void AddOutlineMaterial( Renderer rend )
    {
        GameObject go = new GameObject( "HighlightGameObject" );
        go.transform.SetParent( rend.transform );
        go.layer = rend.gameObject.layer;
        MeshRenderer meshRenderer = go.AddComponent < MeshRenderer >();
        MeshFilter meshFilter = go.AddComponent < MeshFilter >();

        //go.layer = m_MeshRenderer.layer;
        ResetLocalTransform(go.transform);
        rend.gameObject.AddComponent < HighlightObject >().OutlineMeshTransform = go.transform;

        //Create a copy mesh from mesh collider
        Mesh mesh = rend.gameObject.GetComponent < MeshCollider >().sharedMesh;
        meshFilter.sharedMesh = mesh;

        Material[] mats = new Material[mesh.subMeshCount];

        for ( int i = 0; i < mats.Length; ++i )
        {
            mats[i] = m_HighlightMaterial;
        }

        meshRenderer.sharedMaterials = mats;
    }

    private void ResetLocalTransform(Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    #endregion
}

}
