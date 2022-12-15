using System.ComponentModel.DataAnnotations;

namespace UserOnboardingApi.Model
{
    public class userModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Valid Email is Required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        //[Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; } = string.Empty;
        public double ExpiryTime { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;



    }
}
