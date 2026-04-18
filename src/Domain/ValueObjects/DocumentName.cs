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

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            throw new DomainException($"File extension '{extension}' not allowed");

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        return new DocumentName(fileName, extension, nameWithoutExtension);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}