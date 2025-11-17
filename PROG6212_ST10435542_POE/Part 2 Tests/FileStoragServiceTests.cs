using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Moq;
using PROG6212_ST10435542_POE.Controllers;
using PROG6212_ST10435542_POE.Services;
using System.Text;
using System.Text.Json;

namespace Part_2_Tests
{
    [TestClass]
    public class FileStoragServiceTests
    {
        private Mock<IFileStorageService> _mockStorageService;
        private Mock<IWebHostEnvironment> _mockEnv;
        private FileController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockStorageService = new Mock<IFileStorageService>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());

            _controller = new FileController(_mockStorageService.Object, _mockEnv.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var path = Path.Combine(_mockEnv.Object.ContentRootPath, "uploads", "claims.json");
            if (File.Exists(path))
                File.Delete(path);
        }

        [TestMethod]
        public async Task Download_ReturnsFileStream_WhenDocumentExists() // tests if a document downloads when it exists
        {
            // arrange
            var claimsFile = Path.Combine(_mockEnv.Object.ContentRootPath, "uploads", "claims.json");
            var claim = new Dictionary<string, object>
            {
                { "ClaimID", 1 },
                { "LecturerName", "John Doe" },
                { "EncryptedFileName", "testfile.enc" },
                { "OriginalFileName", "document.pdf" },
                { "Status", "PendingManagerApproval" }
            };
            File.WriteAllText(claimsFile, JsonSerializer.Serialize(new List<Dictionary<string, object>> { claim }));

            var fileBytes = new byte[] { 1, 2, 3, 4, 5 };
            _mockStorageService.Setup(f => f.OpenDecryptedStreamAsync("testfile.enc", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(fileBytes));

            var controller = new FileController(_mockStorageService.Object, _mockEnv.Object);

            // act
            var result = await controller.Download(1, CancellationToken.None) as FileStreamResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual("application/octet-stream", result.ContentType);
            Assert.AreEqual("document.pdf", result.FileDownloadName);

            using var ms = new MemoryStream();
            await result.FileStream.CopyToAsync(ms);
            CollectionAssert.AreEqual(fileBytes, ms.ToArray());
        }

        [TestMethod]
        public async Task FileStorageService_SaveEncryptedAsync_SavesFileCorrectly() // tests if a file is saved and encrypted correctly
        {
            // arrange
            var service = new FileStorageService(_mockEnv.Object);
            var fileContent = Encoding.UTF8.GetBytes("test file content");
            var formFile = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "file", "test.txt");

            // act
            var result = await service.SaveEncryptedAsync(formFile, "123");

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test.txt", result.OriginalFileName);
            Assert.IsTrue(result.Size > 0);
            Assert.IsTrue(File.Exists(Path.Combine(_mockEnv.Object.ContentRootPath, "uploads", result.EncryptedFileName)));
        }

        [TestMethod]
        public async Task FileStorageService_OpenDecryptedStreamAsync_ReturnsOriginalContent() // tests if the uploaded document can be decrypted and opened correctly
        {
            // arrange
            var service = new FileStorageService(_mockEnv.Object);
            var fileContent = Encoding.UTF8.GetBytes("original content");
            var formFile = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "file", "original.txt");

            var saveResult = await service.SaveEncryptedAsync(formFile, "456");

            // act
            var stream = await service.OpenDecryptedStreamAsync(saveResult.EncryptedFileName);
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            // assert
            Assert.AreEqual("original content", content);
        }
    }
}
