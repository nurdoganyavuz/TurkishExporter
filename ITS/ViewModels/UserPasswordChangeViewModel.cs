using System;


namespace KobiAsITS.ViewModels
{
    public class UserPasswordChangeViewModel
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
    }
}
