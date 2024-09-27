using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace KoboldSharp
{
    public class KoboldClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUri;

        public KoboldClient(string baseUri)
        {
            _client = new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            _baseUri = baseUri;
        }

        public KoboldClient(string baseUri, HttpClient client)
        {
            _client = client;
            _baseUri = baseUri;
        }

        public async Task<ModelOutput> Generate(GenParams parameters)
        {
            var payload = new StringContent(parameters.GetJson(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{_baseUri}/api/v1/generate", payload);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content = content.Trim();
            return JsonConvert.DeserializeObject<ModelOutput>(content);

        }
        public async Task<ImageOutput> GenerateImage(ImageParams parameters)
        {
            var payload = new StringContent(parameters.GetJson(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{_baseUri}/sdapi/v1/txt2img", payload);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content = content.Trim();
            return JsonConvert.DeserializeObject<ImageOutput>(content);
        }

        public async Task<ModelOutput> Check()
        {
            var payload = new StringContent(string.Empty);
            var response = await _client.PostAsync($"{_baseUri}/api/extra/generate/check", payload);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content = content.Trim();
            return JsonConvert.DeserializeObject<ModelOutput>(content);
        }

        public async void Abort()
        {
            var payload = new StringContent(string.Empty);
            var response = await _client.PostAsync($"{_baseUri}/api/v1/abort", payload);
            await response.Content.ReadAsStringAsync();
        }

        public async void SetSeed(SeedParams seed)
        {
            var new_seed = new StringContent(seed.GetJson(), Encoding.UTF8, "application/json");
            Debug.Log(seed.GetJson());
            await _client.PutAsync($"{_baseUri}/api/v1/config/sampler_seed", new_seed);
        }

        public async void SetMemory(MemoryParams memory)
        {
            var new_seed = new StringContent(memory.GetJson(), Encoding.UTF8, "application/json");
            await _client.PutAsync($"{_baseUri}/api/v1/config/memory", new_seed);
        }

        public async void ClearStory()
        {
            await _client.DeleteAsync($"{_baseUri}/api/v1/story");
        }
    }
}