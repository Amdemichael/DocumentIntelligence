using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;

namespace Domain.Enums;

public class DocumentStatus : Enumeration
{
    public static readonly DocumentStatus Pending = new(0, "Pending");
    public static readonly DocumentStatus Processing = new(1, "Processing");
    public static readonly DocumentStatus AwaitingApproval = new(2, "Awaiting Approval");
    public static readonly DocumentStatus Approved = new(3, "Approved");
    public static readonly DocumentStatus Rejected = new(4, "Rejected");
    public static readonly DocumentStatus Failed = new(5, "Failed");

    private DocumentStatus(int id, string name) : base(id, name) { }

    public bool IsTerminal => this == Approved || this == Rejected || this == Failed;

    // Add this explicit FromValue method
    public static DocumentStatus FromValue(int value)
    {
        return value switch
        {
            0 => Pending,
            1 => Processing,
            2 => AwaitingApproval,
            3 => Approved,
            4 => Rejected,
            5 => Failed,
            _ => throw new ArgumentException($"Invalid status value: {value}")
        };
    }

    // Add this explicit FromName method
    public static DocumentStatus FromName(string name)
    {
        return name switch
        {
            "Pending" => Pending,
            "Processing" => Processing,
            "Awaiting Approval" => AwaitingApproval,
            "Approved" => Approved,
            "Rejected" => Rejected,
            "Failed" => Failed,
            _ => throw new ArgumentException($"Invalid status name: {name}")
        };
    }
}