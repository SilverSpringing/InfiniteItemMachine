using System.Collections.Generic;
using Newtonsoft.Json;

namespace KoboldSharp
{
    public class ImageOutput
    {
        [JsonProperty("images")]
        public string[] Images { get; set; }

        [JsonProperty("parameters")]
        public List<JsonArrayAttribute> Parameters { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

    }
}