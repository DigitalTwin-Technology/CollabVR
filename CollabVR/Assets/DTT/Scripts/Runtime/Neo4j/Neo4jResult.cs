using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ashvin.Neo4j
{

public class Neo4jResult
{
    public class Error
    {
        [JsonProperty("code")]
        public string Code;
        
        [JsonProperty("message")]
        public string Message;
    }

    public class Result
    {
        [JsonProperty( "columns" )]
        public string[] Columns;

        [JsonProperty("data")]
        public Data[] Data;
    }

    public class Data
    {
        public string[] Row;
        public string[] Meta;
    }

    [JsonProperty( "results" )]
    public Result[] Results;

    [JsonProperty( "errors" )]
    public Error[] Errors;

    public override string ToString()
    {
        //TODO: Do it without JsonConvert
        return JsonConvert.SerializeObject( this, Formatting.Indented );
    }
}

}
