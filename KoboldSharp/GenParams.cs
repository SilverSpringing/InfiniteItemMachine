using Newtonsoft.Json;
using System.Collections.Generic;

namespace KoboldSharp
{
    public class GenParams
    {
        [JsonProperty("prompt")]
        public string Prompt { get; set; }
        [JsonProperty("n")]
        public int N { get; set; }
        [JsonProperty("max_context_length")]
        public int MaxContextLength { get; set; }
        [JsonProperty("max_length")]
        public int MaxLength { get; set; }
        [JsonProperty("rep_pen")]
        public float RepPen { get; set; }
        [JsonProperty("temperature")]
        public float Temperature { get; set; }
        [JsonProperty("top_p")]
        public float TopP { get; set; }
        [JsonProperty("top_k")]
        public int TopK { get; set; }
        [JsonProperty("top_a")]
        public int TopA { get; set; }
        [JsonProperty("typical")]
        public int Typical { get; set; }
        [JsonProperty("tfs")]
        public float Tfs { get; set; }
        [JsonProperty("rep_pen_range")]
        public int RepPenRange { get; set; }
        [JsonProperty("rep_pen_slope")]
        public int RepPenSlope { get; set; }
        [JsonProperty("sample_order")]
        public List<int> SamplerOrder { get; set; }
        [JsonProperty("quiet")]
        public bool Quiet { get; set; }
        [JsonProperty("memory")]
        public string Memory { get; set; }

        public GenParams(string prompt = "", int n = 1, int maxContextLength = 2048, int maxLength = 80, float repPen = 1.1f, float temperature = 0.59f, float topP = 1f, int topK = 100, int topA = 0, int typical = 1, float tfs = 0.87f, int repPenRange = 256, int repPenSlope = 1, List<int> samplerOrder = null, bool quiet = true, string memory = "")
        {
            Memory = memory;
            Prompt = prompt;
            N = n;
            MaxContextLength = maxContextLength;
            MaxLength = maxLength;
            RepPen = repPen;
            Temperature = temperature;
            TopP = topP;
            TopK = topK;
            TopA = topA;
            Typical = typical;
            Tfs = tfs;
            RepPenRange = repPenRange;
            RepPenSlope = repPenSlope;
            SamplerOrder = samplerOrder ?? new List<int> { 5, 0, 2, 3, 1, 4, 6 };
            Quiet = quiet;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}