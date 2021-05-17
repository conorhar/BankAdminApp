using System;
using System.Linq;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using SharedThings.Data.Migrations;
using TransactionInspector.Config;
using TransactionInspector.Models;
using DailyReport = SharedThings.Models.DailyReport;

namespace TransactionInspector.Services.EmailService
{
    public class MailtrapEmailService : IEmailService
    {
        private EmailConfiguration emailConfig = new EmailConfiguration
        {
            From = "no-reply@bankadmin.com",
            SmtpServer = "smtp.mailtrap.io",
            Port = 2525,
            UserName = "0d334e27bfc9c1",
            Password = "161db806bc1b5d"
        };

        public void SendMail(DailyReport dr)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailConfig.From));
            message.To.Add(new MailboxAddress(dr.Country.ToLower() + "@testbanken.se")); 
            message.Subject = dr.EmailHeader;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = dr.EmailBody };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(emailConfig.SmtpServer, emailConfig.Port);
                    client.Authenticate(emailConfig.UserName, emailConfig.Password);

                    client.Send(message);
                    client.Disconnect(true);
                }
                catch (Exception)
                {


                    throw;
                }
            }
        }

        public string CreateEmailBody(Report report)
        {
            var sb = new StringBuilder();

            if (report.SuspiciousTransactions.Any())
            {
                sb.AppendLine("TRANSACTIONS OVER 15000 SEK WITHIN LAST 24 HOURS");

                foreach (var t in report.SuspiciousTransactions)
                {
                    sb.AppendLine($"Customer name: {t.CustomerName}");
                    sb.AppendLine($"Account number: {t.AccountId}");
                    sb.AppendLine($"Transaction number: {t.TransactionId}");
                    sb.AppendLine($"Amount: {t.Amount:F}");
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine("NO TRANSACTIONS OVER 15000 SEK WITHIN LAST 24 HOURS");
                sb.AppendLine();
            }

            if (report.SuspiciousTransactionGroups.Any())
            {
                sb.AppendLine("CUSTOMER WHO HAVE MADE TRANSACTIONS EXCEEDING 23000 SEK WITHIN LAST 72 HOURS");

                foreach (var list in report.SuspiciousTransactionGroups)
                {
                    sb.AppendLine("Customer name: " + list.First().CustomerName);
                    sb.AppendLine("Account number: " + list.First().AccountId);
                    foreach (var transaction in list)
                    {
                        sb.Append("Transaction number: " + transaction.TransactionId + " - ");
                        sb.AppendLine("Amount: " + transaction.Amount.ToString("F"));
                    }
                    sb.AppendLine("Total transaction amount: " + list.Sum(r => r.Amount));
                    sb.AppendLine();
                }
            }
            else
                sb.AppendLine("NO CUSTOMERS HAVE MADE TRANSACTIONS EXCEEDING 23000 SEK WITHIN LAST 72 HOURS");

            if (!report.SuspiciousTransactions.Any() && !report.SuspiciousTransactionGroups.Any())
                sb.AppendLine("NO SUSPICIOUS ACTIVITY");

            return sb.ToString();
        }
    }
}