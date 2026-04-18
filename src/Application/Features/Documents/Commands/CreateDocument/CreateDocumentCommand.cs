using Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Documents.Commands.CreateDocument
{
    public class CreateDocumentCommand : ICommand<CreateDocumentResponse>
    {
        public string FileName { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
        public string UploadedBy { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? Description { get; set; }
    }
}
