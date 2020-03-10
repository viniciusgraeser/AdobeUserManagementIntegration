using Newtonsoft.Json;

namespace AdobeUserManagementIntegration.ViewModels
{
    public class AdobeUserManagementResponseViewModel
    {
        [JsonProperty("completed")] public int Completed { get; set; }

        [JsonProperty("notCompleted")] public int NotCompleted { get; set; }

        [JsonProperty("completedInTestMode")] public int CompletedInTestMode { get; set; }

        [JsonProperty("result")] public string Result { get; set; }
    }
}