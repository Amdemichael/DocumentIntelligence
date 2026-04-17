using System;

namespace Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string rule)
        : base($"Business rule violated: {rule}") { }
}

public class InvalidDocumentStateException : DomainException
{
    public InvalidDocumentStateException(string message) : base(message) { }
}