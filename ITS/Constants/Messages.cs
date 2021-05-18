namespace KobiAsITS.Constants
{
    public class Messages
    {
        public static string[] ValidFileTypes = { ".JPG", ".JPEG", ".PNG", ".PDF", ".DOCX", ".XLSX", ".TXT" };

        public static string InvalidFileExtension = "Geçersiz dosya uzantısı, dosya için kabul edilen uzantılar: " + string.Join(",", ValidFileTypes);
        public static string UsersExistsInDepartment = "Bu departmana kayıtlı kullanıcılar mevcut! Silme işlemi gerçekleştirilemiyor.";
    }
}
