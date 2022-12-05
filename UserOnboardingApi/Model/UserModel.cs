using System.ComponentModel.DataAnnotations;

namespace UserOnboardingApi.Model
{
    public class userModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}
