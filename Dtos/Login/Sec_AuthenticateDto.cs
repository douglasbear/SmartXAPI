using System.ComponentModel.DataAnnotations;

namespace SmartxAPI.Dtos.Login
{
    public class Sec_AuthenticateDto
    {
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}