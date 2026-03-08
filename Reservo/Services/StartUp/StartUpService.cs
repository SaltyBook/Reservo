using Reservo.Documents;
using Reservo.Services.Credentials;
using System.Text;

namespace Reservo
{
    public static class StartupService
    {
        public async static void Run()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StartUp.CreateFolderStructure();
            Invoice.CreateItems();
            CredentialsService.Load();
            await CredentialsService.ReadCredentials();
        }
    }
}
