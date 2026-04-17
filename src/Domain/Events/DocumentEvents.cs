using System;
using Domain.Common;

namespace Domain.Events;

public record DocumentCreatedEvent : IDomainEvent
{
    public Guid DocumentId { get; }
    public string FileName { get; }
    public string UploadedBy { get; }
    public DateTime OccurredOn { get; }

    public DocumentCreatedEvent(Guid documentId, string fileName, string uploadedBy)
    {
        DocumentId = documentId;
        FileName = fileName;
        UploadedBy = uploadedBy;
        OccurredOn = DateTime.UtcNow;
    }
}

public record DocumentProcessingStartedEvent : IDomainEvent
{
    public Guid DocumentId { get; }
    public DateTime OccurredOn { get; }

    public DocumentProcessingStartedEvent(Guid documentId)
    {
        DocumentId = documentId;
        OccurredOn = DateTime.UtcNow;
    }
}

public record DocumentProcessedEvent : IDomainEvent
{
    public Guid DocumentId { get; }
    public string ExtractedData { get; }
    public double Confidence { get; }
    public DateTime OccurredOn { get; }

    public DocumentProcessedEvent(Guid documentId, string extractedData, double confidence)
    {
        DocumentId = documentId;
        ExtractedData = extractedData;
        Confidence = confidence;
        OccurredOn = DateTime.UtcNow;
    }
}

public record DocumentProcessingFailedEvent : IDomainEvent
{
    public Guid DocumentId { get; }
    public string Error { get; }
    public DateTime OccurredOn { get; }

    public DocumentProcessingFailedEvent(Guid documentId, string error)
    {
        DocumentId = documentId;
        Error = error;
        OccurredOn = DateTime.UtcNow;
    }
}

public record DocumentApprovedEvent : IDomainEvent
{
    public Guid DocumentId { get; }
    public string ApprovedBy { get; }
    public DateTime OccurredOn { get; }

    public DocumentApprovedEvent(Guid documentId, string approvedBy)
    {
        DocumentId = documentId;
        ApprovedBy = approvedBy;
        OccurredOn = DateTime.UtcNow;
    }
}

public record DocumentRejectedEvent : IDomainEvent
{
    public Guid DocumentId { get; }
    public string RejectedBy { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }

    public DocumentRejectedEvent(Guid documentId, string rejectedBy, string reason)
    {
        DocumentId = documentId;
        RejectedBy = rejectedBy;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
}