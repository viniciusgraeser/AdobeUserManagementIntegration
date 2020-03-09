using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdobeUserManagementIntegration.Commands;
using AdobeUserManagementIntegration.ViewModels;
using Refit;

namespace AdobeUserManagementIntegration.Interfaces
{
    public interface IAdobeUserManagementApi
    {
        [Post("/usermanagement/action/{orgId}")]
        Task<AdobeUserManagementResponseViewModel> AddAdobeID(string orgId, string testOnly, [Header("Authorization")] string token, [Body(BodySerializationMethod.Serialized)]List<InsertAdobeUserManagementCommand> command);

        [Post("/usermanagement/action/{orgId}")]
        Task<AdobeUserManagementResponseViewModel> RemoveFromOrg(string orgId, string testOnly, [Header("Authorization")] string token, [Body(BodySerializationMethod.Serialized)]List<DeleteAdobeUserManagementCommand> command);
    }
}
