namespace GreenDefined.Service.IServices
{
    public interface IMailService
    {
        void SendConfirmEmail(string EmailSentTo, string code);
        void SendOTPForgetPassword(string EmailSentTo, string code);

    }

}
