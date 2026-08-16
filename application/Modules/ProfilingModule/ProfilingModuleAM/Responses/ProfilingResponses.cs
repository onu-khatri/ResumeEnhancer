namespace ResumeEnhancer.ProfilingModule.AM.Responses;

public sealed class UserDetailResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? BillingAddressLine1 { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingCountry { get; set; }
    public string? CommunicationAddressLine1 { get; set; }
    public string? CommunicationCity { get; set; }
    public string? CommunicationCountry { get; set; }
    public bool IsDeactivated { get; set; }
    public IReadOnlyList<int> AccessProfileIds { get; set; } = [];
    public IReadOnlyList<string> AccessProfileCodes { get; set; } = [];
    public DateTime App_CreateDate { get; set; }
    public DateTime? App_UpdateDate { get; set; }
    public byte[] App_Version { get; set; } = [];
}

public sealed class UserListItemResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsDeactivated { get; set; }
}

public sealed class RoleDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool ObsoleteFlag { get; set; }
    public DateTime App_CreateDate { get; set; }
    public DateTime? App_UpdateDate { get; set; }
    public byte[] App_Version { get; set; } = [];
}

public sealed class RoleListItemResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool ObsoleteFlag { get; set; }
}

public sealed class AccessProfileDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool ObsoleteFlag { get; set; }
    public IReadOnlyList<int> RoleIds { get; set; } = [];
    public IReadOnlyList<string> RoleCodes { get; set; } = [];
    public DateTime App_CreateDate { get; set; }
    public DateTime? App_UpdateDate { get; set; }
    public byte[] App_Version { get; set; } = [];
}

public sealed class AccessProfileListItemResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool ObsoleteFlag { get; set; }
}
