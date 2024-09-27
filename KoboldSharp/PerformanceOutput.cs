using System.Collections.Generic;
using Newtonsoft.Json;

namespace KoboldSharp
{
    public class PerformanceOutput
    {
        [JsonProperty("last_process")]
        public int LastProcess = 0;
        [JsonProperty("last_eval")]
        public int LastEval = 0;
        [JsonProperty("last_token_count")]
        public int LastTokenCount = 0;
        [JsonProperty("stop_reason")]
        public int StopReason = 0;
        [JsonProperty("queue")]
        public int Queue = 0;
        [JsonProperty("total_gens")]
        public int TotalGens = 0;
        [JsonProperty("idle")]
        public int Idle = 0;
        [JsonProperty("hordeexitcounter")]
        public int ExitCounter = 0;
        [JsonProperty("uptime")]
        public int Uptime = 0;
    }
}