using System;
using Domain.Common;
using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Aggregates;

public class Document : BaseEntity, IAggregateRoot
{
    // Properties - only unique ones, base properties come from BaseEntity
    public DocumentName Name { get; private set; } = null!;
    public BlobUrl BlobUrl { get; private set; } = null!;
    public DocumentStatus Status { get; private set; } = null!;
    public Email UploadedBy { get; private set; } = null!;
    public FileSize Size { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ProcessedBy { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovalComments { get; private set; }
    public string? ExtractedData { get; private set; }
    public double? ExtractionConfidence { get; private set; }

    // Private constructor for EF Core
    private Document() { }

    // Factory method
    public static Document Create(
        DocumentName name,
        BlobUrl blobUrl,
        Email uploadedBy,
        FileSize size,
        string? description = null)
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Name = name,
            BlobUrl = blobUrl,
            Status = DocumentStatus.Pending,
            UploadedBy = uploadedBy,
            Size = size,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = uploadedBy.ToString()
        };

        // Raise domain event
        document.AddDomainEvent(new DocumentCreatedEvent(
            document.Id,
            name.ToString(),
            uploadedBy.ToString()));

        return document;
    }

    // Business method: Approve the document
    public void Approve(string approvedBy, string? comments = null)
    {
        if (Status != DocumentStatus.AwaitingApproval && Status != DocumentStatus.Pending)
            throw new InvalidDocumentStateException($"Cannot approve document in {Status.Name} state");

        Status = DocumentStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        ApprovalComments = comments;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = approvedBy;

        AddDomainEvent(new DocumentApprovedEvent(Id, approvedBy));
    }

    // Business method: Reject the document
    public void Reject(string rejectedBy, string reason)
    {
        if (Status != DocumentStatus.AwaitingApproval && Status != DocumentStatus.Pending)
            throw new InvalidDocumentStateException($"Cannot reject document in {Status.Name} state");

        Status = DocumentStatus.Rejected;
        ApprovedBy = rejectedBy;
        ApprovedAt = DateTime.UtcNow;
        ApprovalComments = reason;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = rejectedBy;

        AddDomainEvent(new DocumentRejectedEvent(Id, rejectedBy, reason));
    }

    // Helper methods
    public bool CanBeModified()
    {
        return Status == DocumentStatus.Pending || Status == DocumentStatus.Rejected;
    }

    public int GetDaysPending()
    {
        return (int)(DateTime.UtcNow - CreatedAt).TotalDays;
    }
}

public interface IAggregateRoot { }