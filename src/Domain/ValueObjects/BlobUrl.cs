using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class BlobUrl : ValueObject
{
    public string Value { get; }
    public Uri Uri { get; }
    public string ContainerName { get; }
    public string BlobName { get; }

    private BlobUrl(string value, Uri uri, string containerName, string blobName)
    {
        Value = value;
        Uri = uri;
        ContainerName = containerName;
        BlobName = blobName;
    }

    public static BlobUrl Create(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Blob URL cannot be empty");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new DomainException("Invalid blob URL format");

        // Parse Azure Blob URL: https://{account}.blob.core.windows.net/{container}/{blob}
        var pattern = @"https://([^.]+)\.blob\.core\.windows\.net/([^/]+)/(.+)";
        var match = Regex.Match(url, pattern);

        if (!match.Success)
            throw new DomainException("Invalid Azure Blob URL format");

        var containerName = match.Groups[2].Value;
        var blobName = Uri.UnescapeDataString(match.Groups[3].Value);

        return new BlobUrl(url, uri, containerName, blobName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}