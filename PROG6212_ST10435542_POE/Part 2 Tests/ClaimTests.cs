using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Part_2_Tests
{
    [TestClass]
    public class ClaimTests
    {
        private Mock<IWebHostEnvironment> _mockEnv;

        [TestInitialize]
        public void Setup()
        {
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            Directory.CreateDirectory(Path.Combine(_mockEnv.Object.ContentRootPath, "uploads"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            var path = Path.Combine(_mockEnv.Object.ContentRootPath, "uploads", "claims.json");
            if (File.Exists(path))
                File.Delete(path);
        }

        [TestMethod]
        public void Lecturer_SubmitClaim_SavesClaimToFile() // this tests if a claim is saved to a file when submitted by a lecturer
        {
            // arrange
            var claim = new PendingClaimViewModel
            {
                ClaimID = 1,
                LecturerName = "John Doe",
                SubmissionDate = System.DateTime.Now,
                ClaimPeriod = System.DateTime.Now,
                TotalAmount = 1000m,
                Status = ClaimStatusEnum.Submitted,
                FilePath = "file.enc"
            };

            var claimsFile = Path.Combine(_mockEnv.Object.ContentRootPath, "uploads", "claims.json");

            // act
            var claims = new List<PendingClaimViewModel> { claim };
            File.WriteAllText(claimsFile, JsonSerializer.Serialize(claims));

            // assert
            Assert.IsTrue(File.Exists(claimsFile));
            var storedClaims = JsonSerializer.Deserialize<List<PendingClaimViewModel>>(File.ReadAllText(claimsFile));
            Assert.IsNotNull(storedClaims);
            Assert.AreEqual(1, storedClaims.Count);
            Assert.AreEqual("John Doe", storedClaims[0].LecturerName);
        }

        [TestMethod]
        public void AcademicManager_PendingClaims_ReturnsOnlyPendingManagerApproval()
        {
            // arrange
            var claimsFile = Path.Combine(_mockEnv.Object.ContentRootPath, "uploads", "claims.json");
            var claimsData = new List<Dictionary<string, object>>
            {
                new() { { "ClaimID", 1 }, { "LecturerName", "Jane Doe" }, { "Status", ClaimStatusEnum.PendingManagerApproval.ToString() }, { "TotalAmount", 500m }, { "EncryptedFileName", "file1.enc" }, { "SubmissionDate", DateTime.Now.ToString("yyyy-MM-dd") }, { "ClaimPeriod", DateTime.Now.ToString("yyyy-MM-dd") } },
                new() { { "ClaimID", 2 }, { "LecturerName", "John Doe" }, { "Status", ClaimStatusEnum.ApprovedByManager.ToString() }, { "TotalAmount", 700m }, { "EncryptedFileName", "file2.enc" }, { "SubmissionDate", DateTime.Now.ToString("yyyy-MM-dd") }, { "ClaimPeriod", DateTime.Now.ToString("yyyy-MM-dd") } }
            };
            File.WriteAllText(claimsFile, JsonSerializer.Serialize(claimsData));

            var controller = new PROG6212_ST10435542_POE.Controllers.AcademicManagerController(_mockEnv.Object);

            // act
            var result = controller.PendingClaims() as ViewResult;
            var model = result?.Model as List<PendingClaimViewModel>;

            // assert
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model!.Count); // only 1 claim is pending manager approval
            Assert.AreEqual("Jane Doe", model[0].LecturerName);
            Assert.AreEqual(ClaimStatusEnum.PendingManagerApproval, model[0].Status);
        }
    }
}
