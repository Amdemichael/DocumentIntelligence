using Application.Common.Interfaces;

namespace Application.Features.Documents.Queries.GetDocuments;

public class GetDocumentsQuery : IQuery<GetDocumentsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
}



