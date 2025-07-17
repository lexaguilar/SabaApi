namespace Saba.Domain.ViewModels
{
    public class LoginModel
    {
        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

    public class LoginModelReponse
    {
        public string Token { get; set; } = null!;
        public UserModel User { get; set; } = null!;
        public string AccessResources { get; set; } = null!;
     
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string Role { get; set; } = null!;
        public int RoleId { get; set; }
        public string UserName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int CountryId { get; set; }
        public string CountryName { get; set; } = null!;
        public bool IsAdmin { get; set; } = false;
    
    }

    public class ChangePasswordModel
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordManualModel
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public bool SendEmail { get; set; } = true;
    }

    public class ResetPasswordModel
    {
        public string Username { get; set; }
    }

    public class ForgotPasswordConfirmationModel
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}