using System.ComponentModel.DataAnnotations;

namespace AWS_Cognito_Users.Models.Accounts
{
    public class SignupModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}