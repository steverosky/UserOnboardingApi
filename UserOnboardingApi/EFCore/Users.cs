using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserOnboardingApi.EFCore
{
    [Table("users")]
    public class User
    {
        [Key, Required]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Status { get; set; } = "InActive";


    }
}
