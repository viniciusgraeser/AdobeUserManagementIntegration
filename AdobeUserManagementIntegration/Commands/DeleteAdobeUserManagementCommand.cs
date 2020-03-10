using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdobeUserManagementIntegration.Commands
{
    public class DeleteAdobeUserManagementCommand
    {
        [JsonProperty("user")] public string User { get; set; }

        [JsonProperty("requestID")] public string RequestId => Guid.NewGuid().ToString();

        [JsonProperty("do")] public IList<DoRemoveFromOrg> Do => new List<DoRemoveFromOrg> {new DoRemoveFromOrg()};
    }

    public class DoRemoveFromOrg
    {
        [JsonProperty("removeFromOrg")] public RemoveFromOrg RemoveFromOrg => new RemoveFromOrg();
    }

    public class RemoveFromOrg
    {
        [JsonProperty("deleteAccount")] public bool DeleteAccount => false;
    }
}