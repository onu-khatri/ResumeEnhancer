using System.Reflection;
using System.Xml.Linq;
using NetArchTest.Rules;
using Shouldly;
using ResumeEnhancer.WebSolution.ModulesComposition;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.ResumeModule.PL;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.ResumeModule.Web;

namespace ResumeEnhancer.Tests.Unit.Architecture;

public sealed class DependencyRuleTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly string ApplicationRoot = Path.Combine(RepositoryRoot, "application");
    private static readonly string ModulesRoot = Path.Combine(ApplicationRoot, "Modules");

    public static IEnumerable<object[]> ModuleProjectReferenceRules() =>
        LoadModuleProjects()
            .OrderBy(project => project.ModuleName)
            .ThenBy(project => project.Layer)
            .Select(project => new object[] { project });

    public static IEnumerable<object[]> ModulePackageRules() =>
        LoadModuleProjects()
            .OrderBy(project => project.ModuleName)
            .ThenBy(project => project.Layer)
            .Select(project => new object[] { project });

    public static IEnumerable<object[]> AssemblyDependencyRules()
    {
        yield return
        [
            new AssemblyDependencyRule(
                "ResumeModuleAM contracts stay transport/domain/infrastructure-free",
                typeof(CreateResumeRequest).Assembly,
                "ResumeEnhancer.ResumeModule.DM",
                "ResumeEnhancer.ResumeModule.SL",
                "ResumeEnhancer.ResumeModule.PL",
                "ResumeEnhancer.ResumeModule.Web",
                "ResumeEnhancer.Infrastructure.Persistence",
                "Microsoft.AspNetCore",
                "Microsoft.EntityFrameworkCore")
        ];
        yield return
        [
            new AssemblyDependencyRule(
                "ResumeModuleDM domain stays infrastructure-free",
                typeof(Resume).Assembly,
                "ResumeEnhancer.ResumeModule.AM",
                "ResumeEnhancer.ResumeModule.SL",
                "ResumeEnhancer.ResumeModule.PL",
                "ResumeEnhancer.ResumeModule.Web",
                "ResumeEnhancer.Infrastructure.Persistence",
                "Microsoft.AspNetCore",
                "Microsoft.EntityFrameworkCore")
        ];
        yield return
        [
            new AssemblyDependencyRule(
                "ResumeModuleSL use cases do not depend on Web PL or infrastructure",
                typeof(CreateResumeCommand).Assembly,
                "ResumeEnhancer.ResumeModule.PL",
                "ResumeEnhancer.ResumeModule.Web",
                "ResumeEnhancer.Infrastructure.Persistence",
                "Microsoft.AspNetCore",
                "Microsoft.EntityFrameworkCore")
        ];
        yield return
        [
            new AssemblyDependencyRule(
                "ResumeModuleWeb HTTP boundary does not depend on persistence adapter",
                typeof(ResumeEnhancer.ResumeModule.Web.DependencyInjection).Assembly,
                "ResumeEnhancer.ResumeModule.PL",
                "Microsoft.EntityFrameworkCore")
        ];
        yield return
        [
            new AssemblyDependencyRule(
                "ResumeModulePL persistence adapter does not depend on Web",
                typeof(ResumeEnhancer.ResumeModule.PL.DependencyInjection).Assembly,
                "ResumeEnhancer.ResumeModule.Web",
                "Microsoft.AspNetCore")
        ];
    }

    [Theory]
    [MemberData(nameof(ModuleProjectReferenceRules))]
    public void ModuleProjectReferences_ShouldRespectLayerDependencyRules(ProjectModel project)
    {
        var violations = project.ProjectReferences
            .Select(reference => new ProjectReferenceViolation(
                Reference: reference,
                Message: GetProjectReferenceViolation(project, reference)))
            .Where(violation => violation.Message is not null)
            .Select(violation => $"{project.DisplayName} -> {violation.Reference.DisplayName}: {violation.Message}")
            .ToArray();

        violations.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(ModulePackageRules))]
    public void ModulePackages_ShouldNotBypassLayerDependencyRules(ProjectModel project)
    {
        var violations = project.PackageReferences
            .Where(package => IsForbiddenPackage(project, package))
            .Select(package => $"{project.DisplayName} references forbidden package '{package}'.")
            .ToArray();

        violations.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(AssemblyDependencyRules))]
    public void CurrentModuleAssemblies_ShouldRespectCleanArchitectureDependencies(
        AssemblyDependencyRule rule)
    {
        var result = Types.InAssembly(rule.Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(rule.ForbiddenDependencies)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(rule.GetFailureMessage(result));
    }

    [Fact]
    public void WebSolutionServer_ShouldEnterModulesOnlyThroughModulesComposition()
    {
        var project = LoadProject(Path.Combine(
            ApplicationRoot,
            "WebSolution",
            "WebSolution.Server",
            "ResumeEnhancer.WebSolution.Server.csproj"));
        var moduleReferences = project.ProjectReferences
            .Where(reference => reference.Area == ProjectArea.Module)
            .Select(reference => reference.DisplayName)
            .ToArray();

        moduleReferences.ShouldBeEmpty(
            "WebSolution.Server must not reference module projects directly; use WebSolution/ModulesComposition.");
    }

    [Fact]
    public void ModulesComposition_ShouldReferenceOnlyModuleWebAndPersistenceAdapters()
    {
        var project = LoadProject(typeof(ResumeEnhancer.WebSolution.ModulesComposition.DependencyInjection).Assembly);
        var violations = project.ProjectReferences
            .Where(reference => reference.Area == ProjectArea.Module
                && reference.Layer is not ModuleLayer.Web and not ModuleLayer.PL)
            .Select(reference => reference.DisplayName)
            .ToArray();

        violations.ShouldBeEmpty("ModulesComposition may compose module Web and PL projects only.");
    }

    [Fact]
    public void MigrationProject_ShouldReferenceOnlySharedPersistenceAndModulePersistenceAdapters()
    {
        var project = LoadProject(Path.Combine(
            ApplicationRoot,
            "Infrastructure",
            "Migration",
            "ResumeEnhancer.Infrastructure.Migration.csproj"));
        var violations = project.ProjectReferences
            .Where(reference =>
                (reference.Area == ProjectArea.Module && reference.Layer != ModuleLayer.PL)
                || (reference.Area == ProjectArea.Infrastructure && !reference.ProjectName.Equals("ResumeEnhancer.Infrastructure.Persistence", StringComparison.Ordinal)))
            .Select(reference => reference.DisplayName)
            .ToArray();

        violations.ShouldBeEmpty("Migration may reference shared Persistence and module PL projects only.");
    }

    private static string? GetProjectReferenceViolation(ProjectModel project, ProjectReferenceModel reference)
    {
        if (reference.Area == ProjectArea.Module)
        {
            if (!string.Equals(project.ModuleName, reference.ModuleName, StringComparison.Ordinal))
            {
                return "cross-module project references are not allowed";
            }

            var allowedModuleLayers = GetAllowedModuleReferenceLayers(project.Layer);

            return allowedModuleLayers.Contains(reference.Layer)
                ? null
                : $"{project.Layer} projects may reference only module layers: {FormatLayers(allowedModuleLayers)}";
        }

        if (reference.Area == ProjectArea.Core)
        {
            var allowedCoreProjects = GetAllowedCoreReferences(project.Layer);

            return allowedCoreProjects.Contains(reference.ProjectName)
                ? null
                : $"{project.Layer} projects may reference only core projects: {string.Join(", ", allowedCoreProjects)}";
        }

        if (reference.Area == ProjectArea.Infrastructure)
        {
            var allowedInfrastructureProjects = GetAllowedInfrastructureReferences(project.Layer);

            return allowedInfrastructureProjects.Contains(reference.ProjectName)
                ? null
                : $"{project.Layer} projects may reference only infrastructure projects: {FormatProjects(allowedInfrastructureProjects)}";
        }

        return $"module projects must not reference {reference.Area} projects";
    }

    private static IReadOnlySet<ModuleLayer> GetAllowedModuleReferenceLayers(ModuleLayer sourceLayer) =>
        sourceLayer switch
        {
            ModuleLayer.AM => new HashSet<ModuleLayer>(),
            ModuleLayer.DM => new HashSet<ModuleLayer>(),
            ModuleLayer.SL => new HashSet<ModuleLayer> { ModuleLayer.AM, ModuleLayer.DM },
            ModuleLayer.Web => new HashSet<ModuleLayer> { ModuleLayer.AM, ModuleLayer.SL },
            ModuleLayer.PL => new HashSet<ModuleLayer> { ModuleLayer.SL, ModuleLayer.DM },
            _ => throw new InvalidOperationException($"Unsupported module layer '{sourceLayer}'.")
        };

    private static IReadOnlySet<string> GetAllowedCoreReferences(ModuleLayer sourceLayer) =>
        sourceLayer switch
        {
            ModuleLayer.AM => new HashSet<string> { "ResumeEnhancer.Core.CommonLibrary" },
            ModuleLayer.DM => new HashSet<string> { "ResumeEnhancer.Core.CommonLibrary", "ResumeEnhancer.Core.DomainLibrary" },
            ModuleLayer.SL => new HashSet<string> { "ResumeEnhancer.Core.CommonLibrary" },
            ModuleLayer.Web => new HashSet<string> { "ResumeEnhancer.Core.CommonLibrary", "ResumeEnhancer.Core.WebLibrary" },
            ModuleLayer.PL => new HashSet<string> { "ResumeEnhancer.Core.CommonLibrary" },
            _ => throw new InvalidOperationException($"Unsupported module layer '{sourceLayer}'.")
        };

    private static IReadOnlySet<string> GetAllowedInfrastructureReferences(ModuleLayer sourceLayer) =>
        sourceLayer == ModuleLayer.PL
            ? new HashSet<string> { "ResumeEnhancer.Infrastructure.Persistence" }
            : new HashSet<string>();

    private static bool IsForbiddenPackage(ProjectModel project, string packageName)
    {
        if (IsAspNetCorePackage(packageName))
        {
            return project.Layer != ModuleLayer.Web;
        }

        if (IsEntityFrameworkPackage(packageName))
        {
            return project.Layer != ModuleLayer.PL;
        }

        if (packageName.StartsWith("FluentValidation", StringComparison.OrdinalIgnoreCase))
        {
            return project.Layer != ModuleLayer.Web;
        }

        return false;
    }

    private static bool IsAspNetCorePackage(string packageName) =>
        packageName.StartsWith("Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase);

    private static bool IsEntityFrameworkPackage(string packageName) =>
        packageName.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.OrdinalIgnoreCase);

    private static IReadOnlyList<ProjectModel> LoadModuleProjects()
    {
        return Directory.EnumerateFiles(ModulesRoot, "*.csproj", SearchOption.AllDirectories)
            .Select(LoadProject)
            .Where(project => project.Area == ProjectArea.Module)
            .ToArray();
    }

    private static ProjectModel LoadProject(Assembly assembly)
    {
        var projectFile = Directory
            .EnumerateFiles(ApplicationRoot, "*.csproj", SearchOption.AllDirectories)
            .First(path => string.Equals(Path.GetFileNameWithoutExtension(path), assembly.GetName().Name, StringComparison.Ordinal));

        return LoadProject(projectFile);
    }

    private static ProjectModel LoadProject(string projectFile)
    {
        var document = XDocument.Load(projectFile);
        var references = document.Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => LoadProjectReference(projectFile, include!))
            .ToArray();
        var packages = document.Descendants("PackageReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => include!)
            .ToArray();

        return ProjectModel.Create(projectFile, references, packages);
    }

    private static ProjectReferenceModel LoadProjectReference(string sourceProjectFile, string include)
    {
        var sourceDirectory = Path.GetDirectoryName(sourceProjectFile)!;
        var referencePath = Path.GetFullPath(Path.Combine(sourceDirectory, include));

        return ProjectReferenceModel.Create(referencePath);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git"))
                && Directory.Exists(Path.Combine(directory.FullName, "application"))
                && Directory.Exists(Path.Combine(directory.FullName, "test")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root.");
    }

    private static string FormatLayers(IReadOnlySet<ModuleLayer> layers) =>
        layers.Count == 0
            ? "(none)"
            : string.Join(", ", layers.OrderBy(layer => layer.ToString()));

    private static string FormatProjects(IReadOnlySet<string> projects) =>
        projects.Count == 0
            ? "(none)"
            : string.Join(", ", projects.OrderBy(project => project));

    public sealed record AssemblyDependencyRule(
        string Description,
        Assembly Assembly,
        params string[] ForbiddenDependencies)
    {
        public string GetFailureMessage(NetArchTest.Rules.TestResult result)
        {
            var failingTypes = result.FailingTypes is null
                ? string.Empty
                : string.Join(Environment.NewLine, result.FailingTypes.Select(type => $" - {type.FullName}"));

            return $"{Description} failed. Forbidden dependencies: {string.Join(", ", ForbiddenDependencies)}.{Environment.NewLine}{failingTypes}";
        }

        public override string ToString() => Description;
    }

    public sealed record ProjectModel(
        string ProjectFile,
        string ProjectName,
        ProjectArea Area,
        string? ModuleName,
        ModuleLayer Layer,
        IReadOnlyList<ProjectReferenceModel> ProjectReferences,
        IReadOnlyList<string> PackageReferences)
    {
        public string DisplayName => ModuleName is null
            ? ProjectName
            : $"{ModuleName}/{ProjectName}";

        public static ProjectModel Create(
            string projectFile,
            IReadOnlyList<ProjectReferenceModel> projectReferences,
            IReadOnlyList<string> packageReferences)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectFile);
            var area = GetProjectArea(projectFile);
            var moduleName = area == ProjectArea.Module
                ? GetModuleName(projectFile)
                : null;

            return new ProjectModel(
                projectFile,
                projectName,
                area,
                moduleName,
                area == ProjectArea.Module ? GetModuleLayer(projectName) : ModuleLayer.Other,
                projectReferences,
                packageReferences);
        }
    }

    public sealed record ProjectReferenceModel(
        string ProjectFile,
        string ProjectName,
        ProjectArea Area,
        string? ModuleName,
        ModuleLayer Layer)
    {
        public string DisplayName => ModuleName is null
            ? ProjectName
            : $"{ModuleName}/{ProjectName}";

        public static ProjectReferenceModel Create(string projectFile)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectFile);
            var area = GetProjectArea(projectFile);
            var moduleName = area == ProjectArea.Module
                ? GetModuleName(projectFile)
                : null;

            return new ProjectReferenceModel(
                projectFile,
                projectName,
                area,
                moduleName,
                area == ProjectArea.Module ? GetModuleLayer(projectName) : ModuleLayer.Other);
        }
    }

    private sealed record ProjectReferenceViolation(ProjectReferenceModel Reference, string? Message);

    public enum ModuleLayer
    {
        Other,
        AM,
        DM,
        SL,
        PL,
        Web
    }

    public enum ProjectArea
    {
        Other,
        Core,
        Infrastructure,
        Module,
        WebSolution
    }

    private static ProjectArea GetProjectArea(string projectFile)
    {
        var relativePath = Path.GetRelativePath(ApplicationRoot, projectFile);
        var firstSegment = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0];

        return firstSegment switch
        {
            "Core" => ProjectArea.Core,
            "Infrastructure" => ProjectArea.Infrastructure,
            "Modules" => ProjectArea.Module,
            "WebSolution" => ProjectArea.WebSolution,
            _ => ProjectArea.Other
        };
    }

    private static string GetModuleName(string projectFile)
    {
        var relativePath = Path.GetRelativePath(ModulesRoot, projectFile);

        return relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0];
    }

    private static ModuleLayer GetModuleLayer(string projectName)
    {
        if (projectName.EndsWith("AM", StringComparison.Ordinal))
        {
            return ModuleLayer.AM;
        }

        if (projectName.EndsWith("DM", StringComparison.Ordinal))
        {
            return ModuleLayer.DM;
        }

        if (projectName.EndsWith("SL", StringComparison.Ordinal))
        {
            return ModuleLayer.SL;
        }

        if (projectName.EndsWith("PL", StringComparison.Ordinal))
        {
            return ModuleLayer.PL;
        }

        if (projectName.EndsWith("Web", StringComparison.Ordinal))
        {
            return ModuleLayer.Web;
        }

        throw new InvalidOperationException(
            $"Module project '{projectName}' does not follow a supported layer suffix: AM, DM, SL, PL, or Web.");
    }
}




