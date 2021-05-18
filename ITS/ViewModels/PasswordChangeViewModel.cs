using System;

namespace KobiAsITS.ViewModels
{
    public class PasswordChangeViewModel
    {
        public Guid ResetPasswordCode { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
    }
}
