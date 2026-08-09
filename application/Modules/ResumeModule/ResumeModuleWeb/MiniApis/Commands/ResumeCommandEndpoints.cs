using Microsoft.AspNetCore.Builder;

namespace ResumeModuleWeb.MiniApis.Commands;

internal static partial class ResumeCommandEndpoints
{
    public static RouteGroupBuilder MapResumeCommandEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateResumeAsync)
            .WithName("CreateResume");

        group.MapPut("/{resumeId:int}", UpdateResumeAsync)
            .WithName("UpdateResume");

        group.MapDelete("/{resumeId:int}", DeleteResumeAsync)
            .WithName("DeleteResume");

        group.MapPost("/delete", DeleteResumesAsync)
            .WithName("DeleteResumes");

        return group;
    }
}
