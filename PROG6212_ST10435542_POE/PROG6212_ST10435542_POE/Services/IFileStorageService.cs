namespace PROG6212_ST10435542_POE.Services
{
    public class FileSaveResult // model thats returned after a file gets saved to local storage
    {
        public string OriginalFileName { get; set; } = "";
        public string EncryptedFileName { get; set; } = "";
        public long Size { get; set; }
    }

    public interface IFileStorageService
    {
        Task<FileSaveResult> SaveEncryptedAsync(IFormFile file, string claimId, CancellationToken ct = default); // saves and encrypts the uploaded file
        Task<Stream> OpenDecryptedStreamAsync(string encryptedFileName, CancellationToken ct = default); // opens and decrypts the file for viewing or downloading
    }
}
