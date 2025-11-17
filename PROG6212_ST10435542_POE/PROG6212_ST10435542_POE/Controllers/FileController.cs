using Microsoft.AspNetCore.Mvc;
using PROG6212_ST10435542_POE.Services;
using System.Text.Json;

namespace PROG6212_ST10435542_POE.Controllers
{
    public class FileController : Controller // controller handles file related actions like downloading claim documents
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IWebHostEnvironment _env;

// According to FullstackPrep (2025), IWebHostEnvironment provides information about the web hosting environment an application is running in.
        public FileController(IFileStorageService fileStorageService, IWebHostEnvironment env)
        {
            _fileStorageService = fileStorageService;
            _env = env;
        }

        private string ClaimsJsonFile => Path.Combine(_env.ContentRootPath, "uploads", "claims.json"); // defines path to claims json file

        [HttpGet]
        public async Task<IActionResult> Download(int id, CancellationToken ct)
        {
// According to W3Schools (2025), System.IO.File.Exists checks if a file exists at the specified path.
            if (!System.IO.File.Exists(ClaimsJsonFile)) // checks if claims file exists to avoid exceptions
            {
                return NotFound("Claims file not found.");
            }

// According to C-SharpCorner (2017), JsonSerializer.Deserialize converts JSON text into .NET objects.
            var claims = JsonSerializer.Deserialize<List<dynamic>>(System.IO.File.ReadAllText(ClaimsJsonFile)) ?? new List<dynamic>(); // deserializes claims from json
            var claim = claims.FirstOrDefault(c => c.GetProperty("ClaimID").GetInt32() == id); // finds claim matching the id

            if (claim.ValueKind == JsonValueKind.Undefined) // checks if claim was found
            {
                return NotFound("Claim not found.");
            }

            string? encryptedFileName = claim.GetProperty("EncryptedFileName").GetString(); // retrieves encrypted file name from claim
            if (string.IsNullOrEmpty(encryptedFileName)) // ensures file is actually uploaded
            {
                return NotFound("Document not uploaded.");
            }

            var stream = await _fileStorageService.OpenDecryptedStreamAsync(encryptedFileName, ct); // opens file stream with decryption
            string downloadName = claim.GetProperty("OriginalFileName").GetString() ?? encryptedFileName; // determines download name, fallback to encrypted name

            return File(stream, "application/octet-stream", downloadName); // returns file as downloadable response with correct name
        }
    }
}

/* References:

W3Schools, 2025. C# Files. [online] 
Available at: <https://www.w3schools.com/cs/cs_files.php>
[Accessed 10 October 2025].

C-SharpCorner, 2017. How to Read and Write JSON Files in C#. [online] 
Available at: <https://www.c-sharpcorner.com/article/how-to-read-and-write-json-files-in-c-sharp/>
[Accessed 14 October 2025].

FullstackPrep, 2025. Working with IWebHostEnvironment in ASP.NET Core. [online] 
Available at: <https://www.fullstackprep.dev/articles/webd/aspnet/IWebHostEnvironment-in-aspnet-core>
[Accessed 11 October 2025].

*/
