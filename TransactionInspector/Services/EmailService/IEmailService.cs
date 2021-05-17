using TransactionInspector.Models;

namespace TransactionInspector.Services.EmailService
{
    public interface IEmailService
    {
        void SendMail(SharedThings.Models.DailyReport dr);
        string CreateEmailBody(Report r);
    }
}