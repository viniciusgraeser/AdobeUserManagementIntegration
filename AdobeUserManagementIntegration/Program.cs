using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AdobeUserManagementIntegration.Commands;
using AdobeUserManagementIntegration.Interfaces;
using AdobeUserManagementIntegration.ViewModels;
using Jose;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace AdobeUserManagementIntegration
{
    internal class Program
    {
        private const string Metascopes = "https://ims-na1.adobelogin.com/s/ent_user_sdk";
        private const string Aud = "https://ims-na1.adobelogin.com/c/";
        private static IAdobeLoginApi _adobeLoginApi;
        private static IAdobeUserManagementApi _adobeUserManagementApi;

        private static readonly string Certificate = Environment.GetEnvironmentVariable("Certificate");
        private static readonly string CertificatePassword = Environment.GetEnvironmentVariable("CertificatePassword");
        private static readonly string ClientId = Environment.GetEnvironmentVariable("ClientID");
        private static readonly string ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        private static readonly string OrgId = Environment.GetEnvironmentVariable("OrganizationID");
        private static readonly string TechAccId = Environment.GetEnvironmentVariable("TechnicalAccountID");

        private static readonly string IsTestonly =
            Environment.GetEnvironmentVariable("Ambiente").Equals("Development") ||
            Environment.GetEnvironmentVariable("Ambiente").Equals("Staging")
                ? "true"
                : "false";


        public Program(IAdobeLoginApi adobeLoginApi, IAdobeUserManagementApi adobeUserManagementApi)
        {
            _adobeLoginApi = adobeLoginApi;
            _adobeUserManagementApi = adobeUserManagementApi;
        }

        private static void Main()
        {
            var services = new ServiceCollection();

            services.AddRefitClient<IAdobeLoginApi>().ConfigureHttpClient(
                (options) =>
                {
                    options.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EndPointAdobeLoginApi"));
                });

            services.AddRefitClient<IAdobeUserManagementApi>().ConfigureHttpClient(
                (options) =>
                {
                    options.DefaultRequestHeaders.Add("X-Api-Key", ClientId);
                    options.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EndPointAdobeUserManagementApi"));
                });

            services.BuildServiceProvider();

            Console.WriteLine("Adicionando um usuário a Adobe.");
            AddOne();

            Console.WriteLine("Adicionando vários usuários a Adobe.");
            AddMany();

            Console.WriteLine("Apagando um usuário na Adobe.");
            RemoveOne();

            Console.WriteLine("Apagando vários usuários na Adobe.");
            RemoveMany();
        }

        #region Exemplos

        private static void AddOne()
        {
            var command = new InsertUserCommand
            {
                Email = "emailqualquer@dominio.com",
                Firstname = "PrimeiroNome",
                Lastname = "UltimoNome"
            };

            var result = Post(command);
            if (Equals(result.Completed, 1)) Console.WriteLine("Integração Realizada!");
        }

        private static void AddMany()
        {
            var command = new List<InsertUserCommand>
            {
                new InsertUserCommand
                {
                    Email = "emailqualquer1@dominio.com",
                    Firstname = "PrimeiroNome1",
                    Lastname = "UltimoNome1"
                },
                new InsertUserCommand
                {
                    Email = "emailqualquer2@dominio.com",
                    Firstname = "PrimeiroNome2",
                    Lastname = "UltimoNome2"
                },
                new InsertUserCommand
                {
                    Email = "emailqualquer_N@dominio.com",
                    Firstname = "PrimeiroNome_N",
                    Lastname = "UltimoNome_N"
                }
            };

            var result = Post(command);
            if (Equals(result.Completed, 1)) Console.WriteLine("Integração Realizada!");
        }

        private static void RemoveOne()
        {
            var command = new DeleteUserCommand
            {
                Email = "emailqualquer@dominio.com"
            };

            var result = Delete(command);
            if (Equals(result.Completed, 1)) Console.WriteLine("Integração Realizada!");
        }

        private static void RemoveMany()
        {
            var command = new List<DeleteUserCommand>
            {
                new DeleteUserCommand
                {
                    Email = "emailqualquer1@dominio.com"
                },
                new DeleteUserCommand
                {
                    Email = "emailqualquer2@dominio.com"
                },
                new DeleteUserCommand
                {
                    Email = "emailqualquer_N@dominio.com"
                }
            };

            var result = Delete(command);
            if (Equals(result.Completed, 1)) Console.WriteLine("Integração Realizada!");
        }

        #endregion

        #region Chamando EndPoints Adobe

        private static AdobeUserManagementResponseViewModel Post(InsertUserCommand command)
        {
            return AddAdobeId(new List<InsertAdobeUserManagementCommand> { CreateInsertAdobeUser(command) });
        }

        private static AdobeUserManagementResponseViewModel Post(IEnumerable<InsertUserCommand> command)
        {
            var adobeUserManagementCommand = new List<InsertAdobeUserManagementCommand>();

            Parallel.ForEach(command, item => { adobeUserManagementCommand.Add(CreateInsertAdobeUser(item)); });

            return AddAdobeId(adobeUserManagementCommand);
        }

        private static AdobeUserManagementResponseViewModel Delete(DeleteUserCommand command)
        {
            return RemoveFromOrg(new List<DeleteAdobeUserManagementCommand> { CreateDeleteAdobeUser(command) });
        }

        private static AdobeUserManagementResponseViewModel Delete(IEnumerable<DeleteUserCommand> command)
        {
            var adobeUserManagementCommand = new List<DeleteAdobeUserManagementCommand>();

            Parallel.ForEach(command, item => { adobeUserManagementCommand.Add(CreateDeleteAdobeUser(item)); });

            return RemoveFromOrg(adobeUserManagementCommand);
        }

        #endregion

        #region Integração

        private static AdobeUserManagementResponseViewModel AddAdobeId(List<InsertAdobeUserManagementCommand> command)
        {
            return _adobeUserManagementApi.AddAdobeID(OrgId, IsTestonly,
                $"Bearer {CreateToken()}", command).Result;
        }

        private static AdobeUserManagementResponseViewModel RemoveFromOrg(
            List<DeleteAdobeUserManagementCommand> adobeUserManagementCommand)
        {
            return _adobeUserManagementApi.RemoveFromOrg(OrgId, IsTestonly,
                $"Bearer {CreateToken()}", adobeUserManagementCommand).Result;
        }

        private static string CreateToken()
        {
            var payload = new Dictionary<string, object>
            {
                {"exp", DateTimeOffset.Now.ToUnixTimeSeconds() + 60},
                {"iss", OrgId},
                {"sub", TechAccId},
                {"aud", $"{Aud}{ClientId}"},
                {Metascopes, true}
            };

            using var cert = new X509Certificate2(Certificate, CertificatePassword);
            var adobeLogin = new AdobeLoginCommand
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                JwtToken = JWT.Encode(payload, cert.GetRSAPrivateKey(), JwsAlgorithm.RS256)
            };
            return _adobeLoginApi.Login(adobeLogin).Result.AccessToken;
        }

        #endregion

        #region Criar Adobe Command

        private static InsertAdobeUserManagementCommand CreateInsertAdobeUser(InsertUserCommand command)
        {
            return new InsertAdobeUserManagementCommand
            {
                User = command.Email,
                Do = new List<DoAddAdobeId>
                {
                    new DoAddAdobeId
                    {
                        AddAdobeId = new AddAdobeId
                        {
                            Email = command.Email,
                            Firstname = command.Firstname,
                            Lastname = command.Lastname
                        }
                    }
                }
            };
        }

        private static DeleteAdobeUserManagementCommand CreateDeleteAdobeUser(DeleteUserCommand command)
        {
            return new DeleteAdobeUserManagementCommand { User = command.Email };
        }

        #endregion
    }
}