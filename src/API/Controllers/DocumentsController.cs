using Microsoft.AspNetCore.Mvc;
using Domain.Aggregates;
using Domain.Interfaces;
using Domain.ValueObjects;
using Domain.Exceptions;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentRepository documentRepository,
        ILogger<DocumentsController> logger)
    {
        _documentRepository = documentRepository;
        _logger = logger;
    }

    // GET: api/documents
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var documents = await _documentRepository.GetAllAsync(page, pageSize);
        var totalCount = await _documentRepository.GetTotalCountAsync();

        return Ok(new
        {
            data = documents.Select(d => new
            {
                d.Id,
                FileName = d.Name.ToString(),
                Status = d.Status.Name,
                UploadedBy = d.UploadedBy.ToString(),
                UploadedAt = d.CreatedAt,
                FileSize = d.Size.ToString()
            }),
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    // GET: api/documents/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var document = await _documentRepository.GetByIdAsync(id);

        if (document == null)
            return NotFound(new { message = $"Document {id} not found" });

        return Ok(new
        {
            document.Id,
            FileName = document.Name.ToString(),
            Status = document.Status.Name,
            UploadedBy = document.UploadedBy.ToString(),
            UploadedAt = document.CreatedAt,
            FileSize = document.Size.ToString(),
            Description = document.Description,
            ApprovedBy = document.ApprovedBy,
            ApprovedAt = document.ApprovedAt,
            CanBeModified = document.CanBeModified(),
            DaysPending = document.GetDaysPending()
        });
    }

    // POST: api/documents
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentRequest request)
    {
        try
        {
            var document = Document.Create(
                DocumentName.Create(request.FileName),
                BlobUrl.Create(request.BlobUrl),
                Email.Create(request.UploadedBy),
                FileSize.FromBytes(request.FileSize),
                request.Description
            );

            await _documentRepository.AddAsync(document);
            await _documentRepository.SaveChangesAsync();

            _logger.LogInformation("Document created: {DocumentId}", document.Id);

            return CreatedAtAction(nameof(GetById), new { id = document.Id }, new
            {
                document.Id,
                document.Status.Name
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT: api/documents/{id}/approve
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveRequest request)
    {
        var document = await _documentRepository.GetByIdAsync(id);

        if (document == null)
            return NotFound(new { message = $"Document {id} not found" });

        try
        {
            document.Approve(request.ApprovedBy, request.Comments);
            _documentRepository.Update(document);
            await _documentRepository.SaveChangesAsync();

            return Ok(new
            {
                document.Id,
                document.Status.Name,
                document.ApprovedAt
            });
        }
        catch (InvalidDocumentStateException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT: api/documents/{id}/reject
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRequest request)
    {
        var document = await _documentRepository.GetByIdAsync(id);

        if (document == null)
            return NotFound(new { message = $"Document {id} not found" });

        try
        {
            document.Reject(request.RejectedBy, request.Reason);
            _documentRepository.Update(document);
            await _documentRepository.SaveChangesAsync();

            return Ok(new
            {
                document.Id,
                document.Status.Name
            });
        }
        catch (InvalidDocumentStateException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class CreateDocumentRequest
{
    public string FileName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public string UploadedBy { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Description { get; set; }
}

public class ApproveRequest
{
    public string ApprovedBy { get; set; } = string.Empty;
    public string? Comments { get; set; }
}

public class RejectRequest
{
    public string RejectedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}