using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Ashvin._4DVC
{

public class ObjectTask : Task
{
    // Ids of Ifc Objects Operates On
    public string[] Ids;

    // ???????????????????
    [JsonProperty( "id" )]
    public string Id
    {
        set
        {
            Ids = Ids == null ? new[] { value } : Ids.Append( value ).ToArray();
        }
    }

    public bool Valid { get; set; } = false;

    public List < GameObject > GameObject { get; set; } = new();

    public List < MeshRenderer > MeshRenderer { get; set; } = new();

    #region Public

    public override void SetActive( bool enable )
    {
        foreach ( MeshRenderer meshRenderer in MeshRenderer )
        {
            meshRenderer.enabled = enable;
        }
    }

    #endregion
}

}
