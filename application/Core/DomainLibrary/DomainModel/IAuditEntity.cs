namespace ResumeEnhancer.Core.DomainLibrary.DomainModel;

public interface IAuditEntity
{
    int Id { get; set; }

    int? App_CreateUserId { get; set; }

    int? App_UpdateUserId { get; set; }

    DateTime App_CreateDate { get; set; }

    DateTime? App_UpdateDate { get; set; }

    byte[] App_Version { get; set; }
}

