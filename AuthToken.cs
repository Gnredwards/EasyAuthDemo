using System.Text.Json.Serialization;

namespace EasyAuthDemo
{
    public class AuthToken
    {
        [JsonPropertyName("authenticationToken")]
        public string AuthenticationToken { get; set; }
        [JsonPropertyName("user")]
        public AuthenticationUser User { get; set; }
    }
    public class AuthenticationUser
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}