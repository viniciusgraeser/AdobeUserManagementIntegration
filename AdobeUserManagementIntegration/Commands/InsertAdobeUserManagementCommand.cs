using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdobeUserManagementIntegration.Commands
{
    public class InsertAdobeUserManagementCommand
    {
        [JsonProperty("user")] public string User { get; set; }

        [JsonProperty("requestID")] public string RequestId => Guid.NewGuid().ToString();

        [JsonProperty("do")] public IList<DoAddAdobeId> Do { get; set; }
    }

    public class DoAddAdobeId
    {
        [JsonProperty("addAdobeID")] public AddAdobeId AddAdobeId { get; set; }

        [JsonProperty("add")] public Add Add => new Add();
    }

    public class AddAdobeId
    {
        [JsonProperty("email")] public string Email { get; set; }

        [JsonProperty("country")] public string Country => "BR";

        [JsonProperty("firstname")] public string Firstname { get; set; }

        [JsonProperty("lastname")] public string Lastname { get; set; }

        [JsonProperty("option")] public string Option => "ignoreIfAlreadyExists";
    }

    public class Add
    {
        [JsonProperty("group")]
        public static List<string> Group => new List<string> {"Nome do grupo a ser inserido o usuário."};
    }
}