using Microsoft.AspNetCore.Builder;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis.Queries;

internal static partial class ResumeQueryEndpoints
{
    public static RouteGroupBuilder MapResumeQueryEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{resumeId:int}", GetResumeAsync)
            .WithName("GetResume");

        group.MapPost("/search", SearchResumesAsync)
            .WithName("SearchResumes");

        group.MapGet("/{resumeId:int}/exists", ResumeExistsAsync)
            .WithName("ResumeExists");

        return group;
    }
}

