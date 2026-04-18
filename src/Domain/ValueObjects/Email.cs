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

        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, pattern))
            throw new DomainException($"Invalid email format: {email}");

        return new Email(email);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}