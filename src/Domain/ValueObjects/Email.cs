using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");

        email = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(email))
            throw new DomainException($"Invalid email format: {email}");

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            // Basic regex for email validation
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        catch
        {
            return false;
        }
    }

    public string GetDomain() => Value.Split('@')[1];
    public string GetLocalPart() => Value.Split('@')[0];

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}