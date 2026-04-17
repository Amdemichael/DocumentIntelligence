using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class DocumentName : ValueObject
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".png", ".docx" };
    private const int MaxLength = 255;

    public string Value { get; }
    public string Extension { get; }
    public string NameWithoutExtension { get; }

    private DocumentName(string value, string extension, string nameWithoutExtension)
    {
        Value = value;
        Extension = extension;
        NameWithoutExtension = nameWithoutExtension;
    }

    public static DocumentName Create(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("File name cannot be empty");

        if (fileName.Length > MaxLength)
            throw new DomainException($"File name cannot exceed {MaxLength} characters");

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            throw new DomainException($"File extension '{extension}' is not allowed. Allowed: {string.Join(", ", AllowedExtensions)}");

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        // Remove invalid characters
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitizedName = new string(nameWithoutExtension
            .Where(c => !invalidChars.Contains(c))
            .ToArray());

        if (string.IsNullOrWhiteSpace(sanitizedName))
            throw new DomainException("File name contains only invalid characters");

        return new DocumentName($"{sanitizedName}{extension}", extension, sanitizedName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}