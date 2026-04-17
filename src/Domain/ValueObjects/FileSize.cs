using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class FileSize : ValueObject
{
    public long Bytes { get; }
    public double Kilobytes => Bytes / 1024.0;
    public double Megabytes => Kilobytes / 1024.0;
    public double Gigabytes => Megabytes / 1024.0;

    private FileSize(long bytes)
    {
        Bytes = bytes;
    }

    public static FileSize FromBytes(long bytes)
    {
        if (bytes < 0)
            throw new DomainException("File size cannot be negative");

        if (bytes > 500 * 1024 * 1024) // 500MB max
            throw new DomainException("File size cannot exceed 500MB");

        return new FileSize(bytes);
    }

    public static FileSize FromKilobytes(double kilobytes) => FromBytes((long)(kilobytes * 1024));
    public static FileSize FromMegabytes(double megabytes) => FromKilobytes(megabytes * 1024);

    public bool IsGreaterThan(FileSize other) => Bytes > other.Bytes;
    public bool IsLessThan(FileSize other) => Bytes < other.Bytes;
    public bool IsWithinLimit(FileSize limit) => Bytes <= limit.Bytes;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Bytes;
    }

    public override string ToString()
    {
        return Megabytes switch
        {
            >= 1 => $"{Megabytes:F2} MB",
            >= 0.001 => $"{Kilobytes:F2} KB",
            _ => $"{Bytes} B"
        };
    }
}