using ResumeEnhancer.ProfilingModule.AM.Requests;
using ResumeEnhancer.ProfilingModule.AM.Responses;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.DM.Enums;

namespace ResumeEnhancer.ProfilingModule.SL.Mapping;

internal static class ProfilingModelMapper
{
    private static readonly string BillingAddressTypeCode = nameof(UserAddressType.Billing);
    private static readonly string CommunicationAddressTypeCode = nameof(UserAddressType.Communication);

    public static User CreateUser(
        CreateUserRequest request,
        UserAddressTypeSetup billingAddressType,
        UserAddressTypeSetup communicationAddressType) => new()
    {
        FirstName = request.FirstName.Trim(),
        LastName = request.LastName.Trim(),
        Email = request.Email.Trim(),
        UserAddresses = CreateUserAddresses(
            billingAddressType,
            communicationAddressType,
            request.BillingAddressLine1,
            request.BillingCity,
            request.BillingCountry,
            request.CommunicationAddressLine1,
            request.CommunicationCity,
            request.CommunicationCountry),
        IsDeactivated = request.IsDeactivated
    };

    public static void Apply(
        UpdateUserRequest request,
        User user,
        UserAddressTypeSetup billingAddressType,
        UserAddressTypeSetup communicationAddressType)
    {
        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = request.Email.Trim();
        SyncUserAddress(user, billingAddressType, request.BillingAddressLine1, request.BillingCity, request.BillingCountry);
        SyncUserAddress(user, communicationAddressType, request.CommunicationAddressLine1, request.CommunicationCity, request.CommunicationCountry);
        user.IsDeactivated = request.IsDeactivated;
    }

    public static Role CreateRole(CreateRoleRequest request) => new()
    {
        Code = request.Code.Trim(),
        Description = request.Description.Trim(),
        DisplayName = request.DisplayName.Trim()
    };

    public static void Apply(UpdateRoleRequest request, Role role)
    {
        role.Code = request.Code.Trim();
        role.Description = request.Description.Trim();
        role.DisplayName = request.DisplayName.Trim();
        role.ObsoleteFlag = request.ObsoleteFlag;
    }

    public static AccessProfile CreateAccessProfile(CreateAccessProfileRequest request) => new()
    {
        Code = request.Code.Trim(),
        Description = request.Description.Trim(),
        DisplayName = request.DisplayName.Trim()
    };

    public static void Apply(UpdateAccessProfileRequest request, AccessProfile accessProfile)
    {
        accessProfile.Code = request.Code.Trim();
        accessProfile.Description = request.Description.Trim();
        accessProfile.DisplayName = request.DisplayName.Trim();
        accessProfile.ObsoleteFlag = request.ObsoleteFlag;
    }

    public static UserDetailResponse MapUserDetail(User user)
    {
        var billingAddress = GetAddress(user, BillingAddressTypeCode);
        var communicationAddress = GetAddress(user, CommunicationAddressTypeCode);

        return new UserDetailResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            BillingAddressLine1 = billingAddress?.AddressLine1,
            BillingCity = billingAddress?.City,
            BillingCountry = billingAddress?.Country,
            CommunicationAddressLine1 = communicationAddress?.AddressLine1,
            CommunicationCity = communicationAddress?.City,
            CommunicationCountry = communicationAddress?.Country,
            IsDeactivated = user.IsDeactivated,
            AccessProfileIds = user.UserAccessProfiles.Select(item => item.AccessProfileId).Distinct().Order().ToArray(),
            AccessProfileCodes = user.UserAccessProfiles
                .Where(item => item.AccessProfile is not null)
                .Select(item => item.AccessProfile!.Code)
                .Distinct()
                .Order()
                .ToArray(),
            App_CreateDate = user.App_CreateDate,
            App_UpdateDate = user.App_UpdateDate,
            App_Version = user.App_Version
        };
    }

    public static UserListItemResponse MapUserListItem(User user) => new()
    {
        Id = user.Id,
        FullName = $"{user.FirstName} {user.LastName}".Trim(),
        Email = user.Email,
        IsDeactivated = user.IsDeactivated
    };

    public static RoleDetailResponse MapRoleDetail(Role role) => new()
    {
        Id = role.Id,
        Code = role.Code,
        Description = role.Description,
        DisplayName = role.DisplayName,
        ObsoleteFlag = role.ObsoleteFlag,
        App_CreateDate = role.App_CreateDate,
        App_UpdateDate = role.App_UpdateDate,
        App_Version = role.App_Version
    };

    public static RoleListItemResponse MapRoleListItem(Role role) => new()
    {
        Id = role.Id,
        Code = role.Code,
        DisplayName = role.DisplayName,
        ObsoleteFlag = role.ObsoleteFlag
    };

    public static AccessProfileDetailResponse MapAccessProfileDetail(AccessProfile accessProfile) => new()
    {
        Id = accessProfile.Id,
        Code = accessProfile.Code,
        Description = accessProfile.Description,
        DisplayName = accessProfile.DisplayName,
        ObsoleteFlag = accessProfile.ObsoleteFlag,
        RoleIds = accessProfile.AccessProfileRoles.Select(item => item.RoleId).Distinct().Order().ToArray(),
        RoleCodes = accessProfile.AccessProfileRoles
            .Where(item => item.Role is not null)
            .Select(item => item.Role!.Code)
            .Distinct()
            .Order()
            .ToArray(),
        App_CreateDate = accessProfile.App_CreateDate,
        App_UpdateDate = accessProfile.App_UpdateDate,
        App_Version = accessProfile.App_Version
    };

    public static AccessProfileListItemResponse MapAccessProfileListItem(AccessProfile accessProfile) => new()
    {
        Id = accessProfile.Id,
        Code = accessProfile.Code,
        DisplayName = accessProfile.DisplayName,
        ObsoleteFlag = accessProfile.ObsoleteFlag
    };

    private static List<UserAddress> CreateUserAddresses(
        UserAddressTypeSetup billingAddressType,
        UserAddressTypeSetup communicationAddressType,
        string? billingAddressLine1,
        string? billingCity,
        string? billingCountry,
        string? communicationAddressLine1,
        string? communicationCity,
        string? communicationCountry)
    {
        var addresses = new List<UserAddress>();

        AddAddressIfPresent(addresses, billingAddressType, billingAddressLine1, billingCity, billingCountry);
        AddAddressIfPresent(addresses, communicationAddressType, communicationAddressLine1, communicationCity, communicationCountry);

        return addresses;
    }

    private static void SyncUserAddress(
        User user,
        UserAddressTypeSetup addressType,
        string? addressLine1,
        string? city,
        string? country)
    {
        var normalizedAddressLine1 = TrimOrNull(addressLine1);
        var normalizedCity = TrimOrNull(city);
        var normalizedCountry = TrimOrNull(country);
        var existingAddress = GetAddress(user, addressType.Code);

        if (normalizedAddressLine1 is null && normalizedCity is null && normalizedCountry is null)
        {
            if (existingAddress is not null)
            {
                user.UserAddresses.Remove(existingAddress);
            }

            return;
        }

        if (existingAddress is null)
        {
            user.UserAddresses.Add(new UserAddress
            {
                AddressTypeId = addressType.Id,
                AddressType = addressType,
                AddressLine1 = normalizedAddressLine1,
                City = normalizedCity,
                Country = normalizedCountry
            });

            return;
        }

        existingAddress.AddressLine1 = normalizedAddressLine1;
        existingAddress.City = normalizedCity;
        existingAddress.Country = normalizedCountry;
    }

    private static void AddAddressIfPresent(
        ICollection<UserAddress> addresses,
        UserAddressTypeSetup addressType,
        string? addressLine1,
        string? city,
        string? country)
    {
        var normalizedAddressLine1 = TrimOrNull(addressLine1);
        var normalizedCity = TrimOrNull(city);
        var normalizedCountry = TrimOrNull(country);

        if (normalizedAddressLine1 is null && normalizedCity is null && normalizedCountry is null)
        {
            return;
        }

        addresses.Add(new UserAddress
        {
            AddressTypeId = addressType.Id,
            AddressType = addressType,
            AddressLine1 = normalizedAddressLine1,
            City = normalizedCity,
            Country = normalizedCountry
        });
    }

    private static UserAddress? GetAddress(User user, string addressTypeCode) =>
        user.UserAddresses.FirstOrDefault(address =>
            string.Equals(address.AddressType?.Code, addressTypeCode, StringComparison.OrdinalIgnoreCase));

    private static string? TrimOrNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
