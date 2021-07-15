# Adobe User Management API

Desde já aviso que o código neste repositório não irá funcionar. Ele será apenas para mostrar como realizar uma integração com a User Managemement API da Adobe. Para que funcione é necessário ter alguns pré-requisitos configurados na Adobe I/O Console.



1. Criar um "Service Account Integration" conforme o link: https://www.adobe.io/authentication/auth-methods.html#!AdobeDocs/adobeio-auth/master/AuthenticationOverview/ServiceAccountIntegration.md.
2. Seguindo passo 1 você terá o certificado publico e a chave privada.
3. Com o passo 1 feito, o "Service Account Integration" lhe informará  OrgId, TechAccId, Aud, ClientId que serão necessários para realizar a autenticação com a adobe. Como com o payload exemplo abaixo:
   1. var payload = new Dictionary<string, object>
                  {
                      {"exp", DateTimeOffset.Now.ToUnixTimeSeconds() + 60},
                      {"iss", OrgId},
                      {"sub", TechAccId},
                      {"aud", $"{Aud}{ClientId}"},
                      {Metascopes, true}
                  };
4. 