using GreenDefined.Models;
using Microsoft.AspNetCore.Identity;
using MimeKit.Text;
using MimeKit;
using GreenDefined.Service.IServices;

namespace GreenDefined.Service.Services
{
    public class MailService : IMailService
    {

        #region InJect
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public MailService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        #endregion



        public void SendConfirmEmail(string EmailSentTo, string code)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_configuration["WebSite:FromName"], _configuration["WebSite:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(EmailSentTo));
            email.Subject = "Confirm Email";
            var htmlPage = $"<h1>Confirm Email</h1><p> your Confirm Email => {code}</p>";
            email.Body = new TextPart(TextFormat.Html) { Text = htmlPage };
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, true);

            smtp.Authenticate(userName: _configuration["WebSite:FromEmail"], _configuration["WebSite:Password"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public void SendOTPForgetPassword(string EmailSentTo, string code)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_configuration["WebSite:FromName"], _configuration["WebSite:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(EmailSentTo));
            email.Subject = "Forget Password";
            var htmlPage = $"<h1>Forget Password</h1><p> Your forget password code => {code} </p>";
            email.Body = new TextPart(TextFormat.Html) { Text = htmlPage };
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, true);

            smtp.Authenticate(userName: _configuration["WebSite:FromEmail"], _configuration["WebSite:Password"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }

}
