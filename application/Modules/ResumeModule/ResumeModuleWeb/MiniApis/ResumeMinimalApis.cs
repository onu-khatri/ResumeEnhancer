using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using ResumeModuleWeb.MiniApis.Commands;
using ResumeModuleWeb.MiniApis.Queries;

namespace ResumeModuleWeb.MiniApis;

public static class ResumeMinimalApis
{
    public static IEndpointRouteBuilder MapResumeModuleApis(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/resumes")
            .WithTags("Resumes");

        group.MapResumeCommandEndpoints();
        group.MapResumeQueryEndpoints();

        return endpoints;
    }
}
