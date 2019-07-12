using System.Text.Json.Serialization;

namespace EasyAuthDemo
{
    public class AuthInfo // structure based on sample here: https://cgillum.tech/2016/03/07/app-service-token-store/
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("provider_name")]
        public string ProviderName { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("user_claims")]
        public AuthUserClaim[] UserClaims { get; set; }
        [JsonPropertyName("access_token_secret")]
        public string AccessTokenSecret { get; set; }
        [JsonPropertyName("authentication_token")]
        public string AuthenticationToken { get; set; }
        [JsonPropertyName("expires_on")]
        public string ExpiresOn { get; set; }
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
    public class AuthUserClaim
    {
        [JsonPropertyName("typ")]
        public string Type { get; set; }
        [JsonPropertyName("val")]
        public string Value { get; set; }
    }
}