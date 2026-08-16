namespace ResumeEnhancer.BillingModule.AM.Responses;

public sealed class BillingAccountDetailResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ExternalReference { get; set; }
    public DateTime App_CreateDate { get; set; }
    public DateTime? App_UpdateDate { get; set; }
    public byte[] App_Version { get; set; } = [];
}

public sealed class BillingAccountListItemResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class BillingPlanDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string BillingInterval { get; set; } = string.Empty;
    public bool IsDeactivated { get; set; }
    public bool ObsoleteFlag { get; set; }
    public DateTime App_CreateDate { get; set; }
    public DateTime? App_UpdateDate { get; set; }
    public byte[] App_Version { get; set; } = [];
}

public sealed class BillingPlanListItemResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsDeactivated { get; set; }
}

public sealed class BillingSubscriptionDetailResponse
{
    public int Id { get; set; }
    public int BillingAccountId { get; set; }
    public int BillingPlanId { get; set; }
    public int? ResumeId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public DateTime App_CreateDate { get; set; }
    public DateTime? App_UpdateDate { get; set; }
    public byte[] App_Version { get; set; } = [];
}

public sealed class BillingSubscriptionListItemResponse
{
    public int Id { get; set; }
    public int BillingAccountId { get; set; }
    public int BillingPlanId { get; set; }
    public int? ResumeId { get; set; }
    public string Status { get; set; } = string.Empty;
}
