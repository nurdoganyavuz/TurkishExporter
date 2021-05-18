using System.ComponentModel.DataAnnotations;

namespace KobiAsITS.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}
