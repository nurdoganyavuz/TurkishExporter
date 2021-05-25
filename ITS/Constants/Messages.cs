namespace KobiAsITS.Constants
{
    public class Messages
    {
        public static string[] ValidFileTypes = { ".JPG", ".JPEG", ".PNG", ".PDF", ".DOCX", ".XLSX", ".TXT" };

        public static string InvalidFileExtension = "Geçersiz dosya uzantısı, dosya için kabul edilen uzantılar: " + string.Join(",", ValidFileTypes);

        public static string UsersExistsInDepartment = "Bu departmana kayıtlı kullanıcılar mevcut! Silme işlemi gerçekleştirilemiyor.";

        public static string DeletedDepartment = "Departman silindi.";

        public static string UsersExistsInRole = "Bu role kayıtlı kullanıcılar mevcut! Silme işlemi gerçekleştirilemiyor.";

        public static string DeletedRole = "Rol silindi.";

        public static string MismatchedPasswords = "Girdiğiniz şifreler uyuşmuyor!";

        public static string ChangedPassword = "Şifreniz değiştirildi!";

        public static string UsersExists = "Bu e-mail ile sisteme kayıtlı kullanıcı mevcut. Lütfen geçerli bir e-mail adresi giriniz.";
    }
}
