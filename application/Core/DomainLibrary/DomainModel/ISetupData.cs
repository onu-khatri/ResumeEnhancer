namespace DomainLibrary.DomainModel;

public interface ISetupData : IAuditEntity
{
    string Code { get; set; }

    string Description { get; set; }

    System.Guid? Guid { get; set; }

    bool ObsoleteFlag { get; set; }
}
