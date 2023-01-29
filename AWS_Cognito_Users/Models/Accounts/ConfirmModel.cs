using System.ComponentModel.DataAnnotations;

namespace AWS_Cognito_Users.Models.Accounts
{
    public class ConfirmModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }
    }
}