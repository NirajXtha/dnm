using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models.CustomModels
{
    public class FileUpload
    {
        public string contentType { get; set; }
        public string base64 { get; set; }
        public string fileName { get; set; }
    }

    public class SubmitResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class FileMailAttachment
    {
        public string Email { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string FileType { get; set; }

        public string FileName { get; set; } = string.Empty;
    }
}
