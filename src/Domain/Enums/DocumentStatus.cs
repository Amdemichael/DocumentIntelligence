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
}