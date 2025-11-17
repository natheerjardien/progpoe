namespace PROG6212_ST10435542_POE.Models
{
    public class SupportingDocument
    {
        public int DocumentID { get; set; }
        public int ClaimID { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }

        public string EncryptedFileName { get; set; } // stored the encrypted file name
        public DateTime UploadDate { get; set; } // date when the file was uploaded

        public long Size { get; set; } // size of the file in bytes
        public DateTimeOffset? LastModified { get; set; }

        public string DisplaySize // displays the size of the file
        {
            get
            {
                if (Size >= 1024 * 1024)
                {
                    return $"{Size / 1024 / 1024} MB";
                }
                if (Size >= 1024)
                {
                    return $"{Size / 1024} KB";
                }

                return $"{Size} Bytes";
            }
        }

        public MonthlyClaim MonthlyClaim { get; set; }
    }
}
