using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Documents.Queries.GetDocuments
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string FileSize { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool CanBeModified { get; set; }
        public int DaysPending { get; set; }
    }
}
