using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class BlobUrl : ValueObject
{
    public string Value { get; }
    public Uri Uri { get; }
    public string? ContainerName { get; }
    public string? BlobName { get; }
    public bool IsAzureBlob { get; }

    private BlobUrl(string value, Uri uri, bool isAzureBlob, string? containerName = null, string? blobName = null)
    {
        Value = value;
        Uri = uri;
        IsAzureBlob = isAzureBlob;
        ContainerName = containerName;
        BlobName = blobName;
    }

    public static BlobUrl Create(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Blob URL cannot be empty");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new DomainException("Invalid URL format");

        // Check if it's an Azure Blob URL
        var isAzureBlob = uri.Host?.Contains(".blob.core.windows.net") == true;

        string? containerName = null;
        string? blobName = null;

        if (isAzureBlob)
        {
            // Parse Azure Blob URL: https://{account}.blob.core.windows.net/{container}/{blob}
            var segments = uri.AbsolutePath.TrimStart('/').Split('/');
            if (segments.Length >= 2)
            {
                containerName = segments[0];
                blobName = string.Join("/", segments.Skip(1));
            }
        }

        return new BlobUrl(url, uri, isAzureBlob, containerName, blobName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}