using System.Threading.Tasks;
using AdobeUserManagementIntegration.Commands;
using AdobeUserManagementIntegration.Models;
using Refit;

namespace AdobeUserManagementIntegration.Interfaces
{
    public interface IAdobeLoginApi
    {
        [Post("/jwt")]
        Task<AdobeLoginResponseModel> Login([Body(BodySerializationMethod.UrlEncoded)]
            AdobeLoginCommand command);
    }
}