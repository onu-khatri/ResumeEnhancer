using Mediator;
using ResumeModuleAM.Requests;
using ResumeModuleAM.Responses;

namespace ResumeModuleSL.Contracts;

public sealed record SearchResumesQuery(
    ResumeSearchRequest Request) : IQuery<ResumeSearchResponse>;
