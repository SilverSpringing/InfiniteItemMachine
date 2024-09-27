using Newtonsoft.Json;

namespace KoboldSharp
{
    public class MemoryParams
    {
        [JsonProperty("value")]
        public string Memory { get; set; }
        
        public MemoryParams(string memory = "")
        {
            Memory = memory;
        }
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}