namespace Reservo.Services.Credentials
{
    public class SmtpCredentials
    {
        public string SmtpHost { get; set; } = "secureimap.t-online.de";
        public int Port { get; set; } = 993;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
