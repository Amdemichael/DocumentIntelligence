using MediatR;
using Domain.Interfaces;

namespace Application.Features.Documents.Queries.GetDocuments;

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, GetDocumentsResponse>
{
    private readonly IDocumentRepository _documentRepository;

    public GetDocumentsQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<GetDocumentsResponse> Handle(
        GetDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        var documents = await _documentRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await _documentRepository.GetTotalCountAsync(cancellationToken);

        return new GetDocumentsResponse
        {
            Items = documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                FileName = d.Name.ToString(),
                Status = d.Status.Name,
                UploadedBy = d.UploadedBy.ToString(),
                UploadedAt = d.CreatedAt,
                FileSize = d.Size.ToString(),
                Description = d.Description,
                CanBeModified = d.CanBeModified(),
                DaysPending = d.GetDaysPending()
            }).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}