using Reservo.Services.Credentials;
using System.Text;

namespace Reservo
{
    public static class StartupService
    {
        public static async Task<ServiceResult?> RunAsync()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StartUp.CheckDatabasePath();
            StartUp.CreateStructure();
            //CredentialsService.Load();
            //return await CredentialsService.ReadCredentials();
            return null;
        }
    }
}
