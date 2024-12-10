using GreenDefined.DTOs.UserDTos.Edit;
using GreenDefined.DTOs.UserDTos.Login;
using GreenDefined.DTOs.UserDTos.Password;
using GreenDefined.DTOs.UserDTos.Register;
using GreenDefined.Models;

namespace GreenDefined.Service.IServices
{
    public interface IAuthService
    {
        Task<RegisterErrorResponeDTO> RegisterAsync(RegisterDTO model);
        Task<LoginResponseDTO> Login(LoginDTO model);
        Task<string> LogOut(string userid);

        Task<GetUserData> GetUserData(string userId);
        Task<string> ConfirmingAccount(CodeConfirm codeConfirm);
        Task<string> EditProfile(EditProfileDTO DTO);
        Task<ApplicationUser> GetUserById(string id);
        Task<string> SendOTPForgetPassword(string Email);
        Task<string> CheckForgetPasswordOTP(string Email, int Code);
        Task<string> AddNewPassword(AddNewPasswordDTO passwordDTO);
        Task<string> ChangePassword(ChangePasswordDTO passwordDTO);
    }
}
