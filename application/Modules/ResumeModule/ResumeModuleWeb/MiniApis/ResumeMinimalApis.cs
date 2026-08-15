using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using ResumeEnhancer.ResumeModule.Web.MiniApis.Commands;
using ResumeEnhancer.ResumeModule.Web.MiniApis.Queries;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis;

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

