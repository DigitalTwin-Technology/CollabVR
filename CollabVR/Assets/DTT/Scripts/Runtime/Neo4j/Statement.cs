using System.Collections.Generic;
using Newtonsoft.Json;

namespace DTT.Scripts.Runtime.Neo4j
{

public struct Statement
{
    public string statement;
    public Dictionary < string, dynamic > parameters;

    public override string ToString()
    {
        return JsonConvert.SerializeObject( this );
    }
}

}
