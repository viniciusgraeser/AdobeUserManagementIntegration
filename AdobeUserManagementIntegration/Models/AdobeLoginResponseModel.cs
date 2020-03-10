using Newtonsoft.Json;

namespace AdobeUserManagementIntegration.Models
{
    public class AdobeLoginResponseModel
    {
        [JsonProperty("token_type")] public string TokenType { get; set; }

        [JsonProperty("access_token")] public string AccessToken { get; set; }

        [JsonProperty("expires_in")] public uint ExpiresIn { get; set; }
    }
}