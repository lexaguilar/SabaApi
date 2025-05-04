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
     
    }

    public class UserModel{
        public string Role { get; set; } = null!;
        public int RoleId { get; set; }
        public string UserName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
    
    }

    public class ChangePasswordModel
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; }
        public string Host { get; set; }    
    }

    public class ForgotPasswordConfirmationModel
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}