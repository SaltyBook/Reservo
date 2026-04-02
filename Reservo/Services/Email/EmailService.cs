#region Usings
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using Reservo.Infrastructure;
using Reservo.Models;
using Reservo.Services.Credentials;
using System.Diagnostics;
using System.IO;
#endregion

namespace Reservo.Services.Email
{
    public class EmailService : IEmailService
    {
        //Creates a prefilled email draft in Mozilla Thunderbird for a given entry.
        //The method connects to an IMAP mailbox, searches the inbox and send folder for the most recent email exchanged with the recipient,
        //and quotes the latest message in the new email body.
        //It builds an HTML email with a predefined signature, optional invoice or reservation attachment, and launches Thunderbird with the composed email ready to send.
        public void CreateEmail(Entry entry, string year, bool invoice)
        {
            string quoted = "";
            string subject = "";

            using (var client = new ImapClient())
            {
                client.Connect("secureimap.t-online.de", 993);
                client.Authenticate(CredentialsService.creds.Username, CryptoHelper.Decrypt(CredentialsService.creds.Password));
                var inboxMessage = GetLatestMessage(client, client.Inbox, entry.EMail);
                var sendMessage = GetLatestMessage(client, client.GetFolder(SpecialFolder.Sent), entry.EMail);

                if (inboxMessage != null)
                {
                    if (sendMessage != null)
                    {
                        if (inboxMessage.Date < sendMessage.Date)
                        {
                            quoted = GetOldMessages(sendMessage);
                            subject = sendMessage.Subject;
                        }
                        else
                        {
                            quoted = GetOldMessages(inboxMessage);
                            subject = inboxMessage.Subject;
                        }
                    }
                    else
                    {
                        quoted = GetOldMessages(inboxMessage);
                        subject = inboxMessage.Subject;
                    }
                }
                else
                {
                    if (sendMessage != null)
                    {
                        quoted = GetOldMessages(sendMessage);
                        subject = sendMessage.Subject;
                    }
                }
                client.Disconnect(true);
            }

            string body = BuildEmailBody(entry, invoice, quoted);
            string attachment = invoice ? entry.GetInvoicePath(year).Replace(".docx", ".pdf") : entry.GetReservationPath(year).Replace(".docx", ".pdf");

            OpenThunderbird(entry.EMail, subject, body, attachment);
        }

        //Returns the latest email from the given folder send from the specified address.
        private MimeMessage GetLatestMessage(ImapClient client, IMailFolder folder, string fromAddress)
        {
            folder.Open(FolderAccess.ReadOnly);
            var uids = folder.Search(SearchQuery.FromContains(fromAddress));

            MimeMessage latestMessage = null;

            foreach (var uid in uids)
            {
                var message = folder.GetMessage(uid);
                if (latestMessage == null || message.Date > latestMessage.Date)
                    latestMessage = message;
            }

            return latestMessage;
        }

        //Extracts and formats the content of a previous email message to be used as a quoted reply.
        //The method supports both plain text and HTML emails, prefixes lines in classic email quote style, filters unwanted HTML content,
        //and returns the formatted quoted message including sender and date information.
        private string GetOldMessages(MimeMessage message)
        {
            string output = "";
            using (var quoted = new StringWriter())
            {
                var sender = message.Sender ?? message.From.Mailboxes.FirstOrDefault();
                quoted.WriteLine("Am {0} schrieb {1}:", message.Date.ToString("f"), !string.IsNullOrEmpty(sender.Name) ? sender.Name : sender.Address);
                if (message.TextBody != null)
                {
                    using (var reader = new StringReader(message.TextBody))
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            quoted.Write("> ");
                            quoted.WriteLine(line);
                        }
                    }
                }
                else
                {
                    using (var reader = new StringReader(message.HtmlBody))
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains("gruppenhaus.de")) break;
                            if (line == "" || line.StartsWith("<div>&nbsp;")) continue;
                            if (line.Contains("<div>"))
                            {
                                line = line.Substring(line.LastIndexOf("<div>"));
                                line = line.Insert(line.LastIndexOf("<div>") + 5, ">");
                            }
                            quoted.WriteLine(line);
                        }
                    }
                }
                output = quoted.ToString();
            }
            return "--\r\n" + output;
        }

        //Builds the HTML body of the email for a given entry.
        private string BuildEmailBody(Entry entry, bool invoice, string quoted)
        {
            string imageHouse = $@"file://{Paths.ResourcesPath}/imageHouse.jpg";
            string imageLogo = $@"file://{Paths.ResourcesPath}/imageLogo.png";

            string textBody =
            $"<p>Guten Tag {entry.Salutation} {entry.LastName},</p>";
            if (invoice)
            {
                textBody += "<p>wir hoffen, dass alle wieder gut Zuhause angekommen sind.<br>Im Anhang finden Sie die Rechnung für Ihren Aufenthalt bei uns.</p>";
            }
            textBody +=
            "<p>&nbsp;</p>" +
            "<p>Mit freundlichen Grüßen</p>" +
            "<p>Klaus Herget</p>" +
            $"<img src=\"{imageHouse}\" width=\"216\" height=\"96\" />" +
            $"<p>CVJM-Freizeitheim Niederdieten<br>Neuer Weg 11<br>35236 Breidenbach-Niederdieten</p>" +
            $"<p>Festnetz: 06465-20089<br>Mobil: 0178-1968111</p>" +
            $"<img src=\"{imageLogo}\" width=\"175\" height=\"85\" /> " +
            $"<p><a href='https://www.cvjm-kv-biedenkopf.de/website/de/kv/cvjm-kreisverband-biedenkopf/gaestehaus'>Webseite</a></p>" +
            $"<div name =\"quote\" style=\"margin:10px 5px 5px 10px; padding: 10px 0 10px 10px; border-left:2px solid #C3D9E5; word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;\">" +
            $"<div style='white-space: normal; font-size:small; color:gray;'>{quoted}</div>";

            return textBody.Replace("\"", "\\\"");
        }

        //Opens Thunderbird to compose an email with the given parameters.
        private void OpenThunderbird(string to, string subject, string body, string attachmentPath)
        {
            string thunderbirdPath = @"C:\Program Files\Mozilla Thunderbird\thunderbird.exe";
            string arguments = $"-compose \"to='{to}',subject='{subject}',body='{body}',attachment='{attachmentPath}'\"";
            Process.Start(thunderbirdPath, arguments);
        }
    }
}
