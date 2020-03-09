using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using AdobeUserManagementIntegration.Commands;
using AdobeUserManagementIntegration.Interfaces;
using AdobeUserManagementIntegration.ViewModels;
using Jose;

namespace AdobeUserManagementIntegration
{
    internal class Program
    {
        private const string Metascopes = "https://ims-na1.adobelogin.com/s/ent_user_sdk";

        private readonly IAdobeLoginApi _adobeLoginApi;
        private readonly IAdobeUserManagementApi _adobeUserManagementApi;
        private const string Aud = "https://ims-na1.adobelogin.com/c/";
        private readonly string _certificate = Environment.GetEnvironmentVariable("Certificate");
        private readonly string _certificatePassword = Environment.GetEnvironmentVariable("CertificatePassword");
        private readonly string _clientId = Environment.GetEnvironmentVariable("ClientID");
        private readonly string _clientSecret = Environment.GetEnvironmentVariable("ClientSecret");

        private readonly string _isTestonly =
            Environment.GetEnvironmentVariable("Ambiente").Equals("Development") ||
            Environment.GetEnvironmentVariable("Ambiente").Equals("Staging")
                ? "true"
                : "false";

        private readonly string _orgId = Environment.GetEnvironmentVariable("OrganizationID");
        private readonly string _techAccId = Environment.GetEnvironmentVariable("TechnicalAccountID");

        public Program(IAdobeLoginApi adobeLoginApi, IAdobeUserManagementApi adobeUserManagementApi)
        {
            _adobeLoginApi = adobeLoginApi;
            _adobeUserManagementApi = adobeUserManagementApi;
        }

        private static void Main()
        {
            Console.WriteLine("Adicionando um usuário a Adobe.");

            Console.WriteLine("Adicionando vários usuários a Adobe.");

            Console.WriteLine("Apagando um usuário na Adobe.");

            Console.WriteLine("Apagando vários usuários na Adobe.");
        }


        #region Integração

        private AdobeUserManagementResponseViewModel AddAdobeId(List<InsertAdobeUserManagementCommand> command)
        {
            return _adobeUserManagementApi.AddAdobeID(_orgId, _isTestonly,
                $"Bearer {CreateToken()}", command).Result;
        }

        private AdobeUserManagementResponseViewModel RemoveFromOrg(
            List<DeleteAdobeUserManagementCommand> adobeUserManagementCommand)
        {
            return _adobeUserManagementApi.RemoveFromOrg(_orgId, _isTestonly,
                $"Bearer {CreateToken()}", adobeUserManagementCommand).Result;
        }

        private string CreateToken()
        {
            var payload = new Dictionary<string, object>
            {
                {"exp", DateTimeOffset.Now.ToUnixTimeSeconds() + 60},
                {"iss", _orgId},
                {"sub", _techAccId},
                {"aud", $"{Aud}{_clientId}"},
                {Metascopes, true}
            };

            var cert = new X509Certificate2(_certificate, _certificatePassword);

            var adobeLogin = new AdobeLoginCommand
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                JwtToken = JWT.Encode(payload, cert.GetRSAPrivateKey(), JwsAlgorithm.RS256)
            };

            return _adobeLoginApi.Login(adobeLogin).Result.AccessToken;
        }

        #endregion
    }
}