using MediatR;
using Domain.Aggregates;
using Domain.Interfaces;
using Domain.ValueObjects;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Features.Documents.Commands.CreateDocument;

public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, CreateDocumentResponse>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ILogger<CreateDocumentCommandHandler> _logger;

    public CreateDocumentCommandHandler(
        IDocumentRepository documentRepository,
        ILogger<CreateDocumentCommandHandler> logger)
    {
        _documentRepository = documentRepository;
        _logger = logger;
    }

    public async Task<CreateDocumentResponse> Handle(
        CreateDocumentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Create value objects (validation happens here)
            var name = DocumentName.Create(request.FileName);
            var blobUrl = BlobUrl.Create(request.BlobUrl);
            var uploadedBy = Email.Create(request.UploadedBy);
            var fileSize = FileSize.FromBytes(request.FileSize);

            // Create domain aggregate using factory method
            var document = Document.Create(
                name,
                blobUrl,
                uploadedBy,
                fileSize,
                request.Description);

            // Save to database
            await _documentRepository.AddAsync(document, cancellationToken);
            await _documentRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document created successfully: {DocumentId}", document.Id);

            return new CreateDocumentResponse
            {
                Id = document.Id,
                Status = document.Status.Name,
                Message = "Document created successfully"
            };
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Domain validation failed: {Error}", ex.Message);
            throw;
        }
    }
}