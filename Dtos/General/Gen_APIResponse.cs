using System.ComponentModel.DataAnnotations;

namespace SmartxAPI.Dtos.General
{
    public class Gen_APIResponse
    {
        [Required]
        public string ErrorCode { get; set; }

        [Required]
        public string Message { get; set; }

    }
}