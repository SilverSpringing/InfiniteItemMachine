using Newtonsoft.Json;

namespace InfiniteItems
{
    public class ItemJSONData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon-data")]
        public string IconData { get; set; }

        public ItemJSONData(string name = "", string description = "", string icon_data = "")
        {
            Name = name;
            Description = description;
            IconData = icon_data;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}