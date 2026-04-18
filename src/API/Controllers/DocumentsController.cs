using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Documents.Commands.CreateDocument;
using Application.Features.Documents.Queries.GetDocuments;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(IMediator mediator, ILogger<DocumentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new document
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get all documents with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetDocumentsQuery { Page = page, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}