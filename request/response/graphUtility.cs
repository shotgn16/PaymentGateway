using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentGateway.request.response
{
    public class graphUtility
    {
        public class RootModel
        {
            [JsonPropertyName("@odata.context")] public string OdataContext { get; set; }
            [JsonPropertyName("value")] public List<OdataValue> Value { get; set; }
        }

        public class OdataValue
        {
            [JsonPropertyName("displayName")] public string DisplayName { get; set; }
            [Newtonsoft.Json.JsonExtensionData] public Dictionary<string, JToken> AdditionalProperties { get; set; }
        }
    }
}

//Credit: Pobiega#2671 @ Discord