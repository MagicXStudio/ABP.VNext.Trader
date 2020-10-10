using Newtonsoft.Json;
using System;

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

        /// <summary>
        /// 终结器[析构函数]用于在垃圾回收器收集类实例时执行任何必要的最终清理
        /// </summary>
        ~ContactItem()
        {
            GC.SuppressFinalize(this);
        }
    }
}
