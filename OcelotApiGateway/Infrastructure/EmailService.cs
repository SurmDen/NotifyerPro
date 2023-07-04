using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace OcelotApiGateway.Infrastructure
{
    public class EmailService
    {
        public static async Task SendEmailAsync(string email, int password)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("NotifyerPro Administration", "surmanidzedenis609@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("Dear NotifyerPro User", email));
            emailMessage.Subject = "for you darling";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = password.ToString()
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("surmanidzedenis609@gmail.com", "cfyofdfwilntaapq");
                await client.SendAsync(emailMessage);

                //cfyofdfwilntaapq
                await client.DisconnectAsync(true);
            }
        }
    }
}
