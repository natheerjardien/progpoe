using System.Security.Cryptography;

namespace PROG6212_ST10435542_POE.Services
{
    public class FileStorageService : IFileStorageService // implements secure local storage of files using AES encyrption
    {
        private readonly string _storageFolder; // local folder where the files are stored

        private static readonly byte[] Key = Convert.FromHexString("00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF"); // key used for encryption and decryption
        private static readonly byte[] IV = Convert.FromHexString("AABBCCDDEEFF00112233445566778899");

        public FileStorageService(IWebHostEnvironment env)
        {
            _storageFolder = Path.Combine(env.ContentRootPath, "uploads"); // ensures uploads folder exists in the projects root folder
            Directory.CreateDirectory(_storageFolder);
        }

        public async Task<FileSaveResult> SaveEncryptedAsync(IFormFile file, string claimId, CancellationToken ct = default) // encrypts and saves the uploaded file to local storage
        {
            if (file == null || file.Length == 0) // checks if the file exists
            {
                throw new ArgumentException("File cannot tbe empty.", nameof(file));
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant(); // generates a unique file name including the claimID and GUID
            var encryptedFileName = $"{claimId}_{Guid.NewGuid():N}{fileExtension}";

            var path = Path.Combine(_storageFolder, encryptedFileName);

            using (var memoryStream = new MemoryStream()) // reads the file into memory and encrypts it
            {
                await file.CopyToAsync(memoryStream, ct);
                var fileBytes = memoryStream.ToArray();

                var encryptedBytes = Encrypt(fileBytes); // encrypts the file bytes using AES

                await File.WriteAllBytesAsync(path, encryptedBytes, ct); // saves the encrypted file to uploads folder
            }

            return new FileSaveResult // returns the result including original file name, encrypted file name and size of the file
            {
                OriginalFileName = file.FileName,
                EncryptedFileName = encryptedFileName,
                Size = file.Length
            };
        }

        public async Task<Stream> OpenDecryptedStreamAsync(string encryptedFileName, CancellationToken ct = default) // opens and decrypts the file for viewing or downloading
        {
            var path = Path.Combine(_storageFolder, encryptedFileName);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Encrypted file not found", path);
            }

            var encryptedBytes = await File.ReadAllBytesAsync(path, ct); // reads the encrypted bytes

            var decryptedBytes = Decrypt(encryptedBytes); // decrypts the bytes using AES

            return new MemoryStream(decryptedBytes); // returns the memory stream for downloading and viewing the file
        }

        // I put the encryption helpers within this service class to keep it contained, seeing as im not using a database as yet
        private static byte[] Encrypt(byte[] data) // encryption helper
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, encryptor);
                }
            }
        }

        private static byte[] Decrypt(byte[] data) // decryption helper
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, decryptor);
                }
            }
        }

        private static byte[] PerformCryptography(byte[] data, ICryptoTransform transform) // shares logic for encryption and decryption
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length); // writes the data to the crypto stream
                    cryptoStream.FlushFinalBlock(); // finalizes the encryption/decryption process

                    return memoryStream.ToArray();
                }
            }
        }
    }
}
