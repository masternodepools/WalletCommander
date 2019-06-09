using Newtonsoft.Json;

namespace WalletCommander.Models
{
    public class UserWallet
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("coin")]
        public string Coin { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("createdAt")]
        public long CreatedAt { get; set; }
    }
}
