using GreenDefined.DTOs.UserDTos.Edit;
using GreenDefined.DTOs.UserDTos.Login;
using GreenDefined.DTOs.UserDTos.Password;
using GreenDefined.DTOs.UserDTos.Register;
using GreenDefined.Models;
using GreenDefined.Service.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GreenDefined.Service.Services
{
    public class AuthService : IAuthService
    {
        #region Fields
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IUploadingImage _uploadingImage;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;

        #endregion

        #region Ctor
        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            IMailService mailService, IUploadingImage uploadingImage, IHttpContextAccessor httpContextAccessor
             , SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _uploadingImage = uploadingImage;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;

        }


        #endregion

        #region Handle Functions

        #region Login/Register
        //Email Service !
        public async Task<RegisterErrorResponeDTO> RegisterAsync(RegisterDTO model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            { // User with same email

                return new RegisterErrorResponeDTO() { message = "Existing", error = "Email Already Registered" };
            }

            //maping from RegisterDTO to ApplicationUser
            var user = new ApplicationUser() // Role - Password 
            {
                Email = model.Email,
                FullName = model.FullName,
                UserName = model.Email

            };
            //Create User  
            var result = await _userManager.CreateAsync(user, model.Password);

            // Complete Comment and mapping
            if (!result.Succeeded)
            {
                var Es = string.Empty;
                foreach (var error in result.Errors)
                {
                    Es += $"{error.Description} ,";
                }
                return new RegisterErrorResponeDTO() { message = "Password", error = Es };

            }
            //Email 
            Random generator = new Random();
            string Code = generator.Next(1000, 10000).ToString("D4");
            //Save in DB
            user.ValidationCode = Code;
            var updating = await _userManager.UpdateAsync(user);
            if (!updating.Succeeded)
            {
                return new RegisterErrorResponeDTO
                {
                    message = "Falid Save Verify Code",
                    error = $"{updating.Errors.ToList()}"
                };
            }
            // Send Email ! 
            _mailService.SendConfirmEmail(user.Email, Code);

            await _userManager.UpdateAsync(user);//To Make Sure thier is id! "كانت ساعات بتعمل ايرور"
            return new RegisterErrorResponeDTO() { message = "Success" };
        }

        public async Task<string> ConfirmingAccount(CodeConfirm codeConfirm)
        {
            var user = await _userManager.FindByEmailAsync(codeConfirm.userEmail);
            if (user == null) return "No user with this email";
            if (user.ValidationCode == codeConfirm.Code)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return "Success";
            }
            return "Faild";
        }
        public async Task<LoginResponseDTO> Login(LoginDTO model)
        {
            LoginResponseDTO returnedUser = new();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                returnedUser.Message = "Name Or Password is invalid..!";
                returnedUser.Email = model.Email;
                return returnedUser;
            }
            if (user.EmailConfirmed == false)
            {
                returnedUser.Message = "Not Confirmed";
                returnedUser.Email = model.Email;
                returnedUser.IsAuthenticated = false;
                return returnedUser;
            }

            //JWT
            var JwtSecuirtyToken = await CreateJwtToken(user);

            //map from applicationUser => LoginResponseDTO
            var returned = new LoginResponseDTO()
            {
                Email = user.Email,
                IsAuthenticated = true,
                userId = user.Id,
                FullName = user.FullName,
                imageUrl = user.imageUrl,
                Country = user.Country,
                Bio = user.Bio,
            };

            returned.Expier = JwtSecuirtyToken.ValidTo;
            returned.Token = new JwtSecurityTokenHandler().WriteToken(JwtSecuirtyToken);
            return returned;
        }

        public async Task<GetUserData> GetUserData(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return new GetUserData();
            var response = new GetUserData()
            {
                Bio=user.Bio,
                Country=user.Country,
                Email=user.Email,
                FullName=user.FullName,
                imageUrl=user.imageUrl,
                userId = userId,
            };

            return response;
        }
        public async Task<string> LogOut(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            await _signInManager.SignOutAsync();
            return "";
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id)
            }
            .Union(userClaims);
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var SigingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddDays(7), // Expire date..
                signingCredentials: SigingCredentials
                );
            var myToken = new
            {
                _token = new JwtSecurityTokenHandler().WriteToken(token), // To make Token Json
                expiration = token.ValidTo
            };
            return token;
        }

        #endregion

        #region Password
        public async Task<string> SendOTPForgetPassword(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null) return "NoUser";
            //Generate OTP
            Random generator = new Random();
            string Code = generator.Next(0, 1000000).ToString("D6");
            //Send 
            _mailService.SendOTPForgetPassword(Email, Code);
            //Save in DB
            user.ForgetPasswordOTP = int.Parse(Code);
            await _userManager.UpdateAsync(user);
            //return
            return "";
        }

        public async Task<string> CheckForgetPasswordOTP(string Email, int Code)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null) return "No User with this Email!";
            if (user.ForgetPasswordOTP == Code) return "Success";
            return "Not Same Code!";
        }

        public async Task<string> AddNewPassword(AddNewPasswordDTO passwordDTO)
        {
            var user = await _userManager.FindByEmailAsync(passwordDTO.Email);
            if (user == null) return "No User with this Email!";

            await _userManager.RemovePasswordAsync(user);
            await _userManager.UpdateAsync(user);
            await _userManager.AddPasswordAsync(user, passwordDTO.Password);
            await _userManager.UpdateAsync(user);

            return "Success";
        }

        public async Task<string> ChangePassword(ChangePasswordDTO passwordDTO)
        {
            var user = await _userManager.FindByEmailAsync(passwordDTO.Email);
            if (user == null) return "No User with this Email!";

            var result = await _userManager.ChangePasswordAsync(user, passwordDTO.CurrentPassword, passwordDTO.NewPassword);

            if (result.Succeeded)
            {
                return "Success";
            }
            foreach (var error in result.Errors)
            {
                string response = "";
                response += $" {error.Description}. ";
                return response;
            }


            return "";
        }

        #endregion

        #region EditProfile , GetActiveUsers

        public async Task<string> EditProfile(EditProfileDTO DTO)
        {
            var user = await _userManager.FindByIdAsync(DTO.id);
            if (user == null) return "Not Found user by this id!";
            user.FullName = DTO.FullName;
            user.Bio = DTO.Bio;
            user.Country = DTO.Country;
            if (DTO.ProfileImage.Length > 0)
            {
                var schema = _httpContextAccessor.HttpContext.Request.Scheme;
                var host = _httpContextAccessor.HttpContext.Request.Host;
                var saving = await _uploadingImage.SavingImage(DTO.ProfileImage, schema, host);
                if (saving.IsSuccess)
                {
                    user.imageUrl = saving.urlOrError;
                }
            }
            await _userManager.UpdateAsync(user);

            return "Success";
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        #endregion

        #endregion
    }
}
