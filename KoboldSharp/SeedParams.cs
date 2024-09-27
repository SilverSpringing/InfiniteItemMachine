using System.Collections.Generic;
using Newtonsoft.Json;

namespace KoboldSharp
{
    public class SeedParams
    {
        [JsonProperty("value")]
        public long Seed { get; set; }

        public SeedParams(long seed = 3475097509890965500)
        {
            Seed = seed;
        }
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}