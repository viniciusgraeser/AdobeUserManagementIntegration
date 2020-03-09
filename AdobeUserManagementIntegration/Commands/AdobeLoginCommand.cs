using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AdobeUserManagementIntegration.Commands
{
    public class AdobeLoginCommand
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("jwt_token")]
        public string JwtToken { get; set; }
    }
}
