using Newtonsoft.Json;
using System.Collections.Generic;

namespace KoboldSharp
{
    public class ImageParams
    {
        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("negative_prompt")]
        public string NegativePrompt { get; set; }

        [JsonProperty("cfg_scale")]
        public int cfgScale { get; set; }

        [JsonProperty("steps")]
        public int Steps { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("seed")]
        public long Seed { get; set; }

        [JsonProperty("sampler_name")]
        public string SamplerName { get; set; }

        public ImageParams(string prompt = "", string negative_prompt = "", int cfg_scale = 1, int steps = 1, int width = 64, int height = 64, long seed = -1, string sampler_name = "")
        {
            Prompt = prompt;
            NegativePrompt = negative_prompt;
            cfgScale = cfg_scale;
            Steps = steps;
            Width = width;
            Height = height;
            Seed = seed;
            SamplerName = sampler_name;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}