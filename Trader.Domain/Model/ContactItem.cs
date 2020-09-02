using Newtonsoft.Json;

namespace Trader.Domain.Model
{
    public class ContactItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("continent")]
        public string Avatar { get; set; }
        [JsonProperty("subregion")]
        public string Status { get; set; }
        [JsonProperty("region")]
        public string TimeStamp { get; set; }
    }
}
