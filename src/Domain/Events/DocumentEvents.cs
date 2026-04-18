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