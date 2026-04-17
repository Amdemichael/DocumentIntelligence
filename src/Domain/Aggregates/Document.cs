using System;
using Domain.Common;
using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Aggregates;

public class Document : BaseEntity, IAggregateRoot
{
    // Properties
    public DocumentName Name { get; private set; }
    public BlobUrl BlobUrl { get; private set; }
    public DocumentStatus Status { get; private set; }
    public Email UploadedBy { get; private set; }
    public FileSize Size { get; private set; }
    public string? Description { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ProcessedBy { get; private set; }
    public string? ProcessingError { get; private set; }

    // Approval related
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovalComments { get; private set; }

    // AI extracted data (JSON)
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

        document.AddDomainEvent(new DocumentCreatedEvent(document.Id, name.ToString(), uploadedBy.ToString()));

        return document;
    }

    // Domain behaviors
    public void StartProcessing(string processor)
    {
        if (Status != DocumentStatus.Pending)
            throw new InvalidDocumentStateException($"Cannot process document in {Status.Name} state");

        Status = DocumentStatus.Processing;
        ProcessedBy = processor;

        AddDomainEvent(new DocumentProcessingStartedEvent(Id));
    }

    public void CompleteProcessing(string extractedData, double confidence)
    {
        if (Status != DocumentStatus.Processing)
            throw new InvalidDocumentStateException($"Cannot complete processing in {Status.Name} state");

        ExtractedData = extractedData;
        ExtractionConfidence = confidence;
        Status = confidence >= 0.8 ? DocumentStatus.AwaitingApproval : DocumentStatus.Pending;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new DocumentProcessedEvent(Id, extractedData, confidence));
    }

    public void FailProcessing(string error)
    {
        if (Status != DocumentStatus.Processing)
            throw new InvalidDocumentStateException($"Cannot fail document in {Status.Name} state");

        Status = DocumentStatus.Failed;
        ProcessingError = error;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new DocumentProcessingFailedEvent(Id, error));
    }

    public void Approve(string approvedBy, string? comments = null)
    {
        if (Status != DocumentStatus.AwaitingApproval)
            throw new InvalidDocumentStateException($"Cannot approve document in {Status.Name} state");

        Status = DocumentStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        ApprovalComments = comments;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = approvedBy;

        AddDomainEvent(new DocumentApprovedEvent(Id, approvedBy));
    }

    public void Reject(string rejectedBy, string reason)
    {
        if (Status != DocumentStatus.AwaitingApproval)
            throw new InvalidDocumentStateException($"Cannot reject document in {Status.Name} state");

        Status = DocumentStatus.Rejected;
        ApprovedBy = rejectedBy;
        ApprovedAt = DateTime.UtcNow;
        ApprovalComments = reason;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = rejectedBy;

        AddDomainEvent(new DocumentRejectedEvent(Id, rejectedBy, reason));
    }

    public void UpdateDescription(string description, string updatedBy)
    {
        if (!CanBeModified())
            throw new InvalidDocumentStateException($"Cannot modify document in {Status.Name} state");

        Description = description;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;
    }

    public bool CanBeModified()
    {
        return Status == DocumentStatus.Pending || Status == DocumentStatus.Rejected;
    }

    public bool NeedsHumanReview()
    {
        return ExtractionConfidence.HasValue && ExtractionConfidence < 0.8;
    }

    public int GetDaysPending()
    {
        return (int)(DateTime.UtcNow - CreatedAt).TotalDays;
    }
}

// Marker interface for aggregate roots
public interface IAggregateRoot { }