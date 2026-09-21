using System.Reflection;
using System.Xml.Linq;
using NetArchTest.Rules;
using Shouldly;
using ResumeEnhancer.WebSolution.ModulesComposition;
using ResumeEnhancer.Infrastructure.Persistence;

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

    public static IEnumerable<object[]> CoreAndInfrastructureProjectRules() =>
        LoadApplicationProjects()
            .Where(project => project.Area is ProjectArea.Core or ProjectArea.Infrastructure)
            .OrderBy(project => project.ProjectName, StringComparer.Ordinal)
            .Select(project => new object[] { project });

    public static IEnumerable<object[]> CrossModuleReferencePolicyRules()
    {
        yield return [ModuleLayer.DM, ModuleLayer.DM, true];
        yield return [ModuleLayer.SL, ModuleLayer.SL, true];
        yield return [ModuleLayer.PL, ModuleLayer.PL, false];
        yield return [ModuleLayer.Web, ModuleLayer.Web, false];
        yield return [ModuleLayer.PL, ModuleLayer.Web, false];
        yield return [ModuleLayer.AM, ModuleLayer.DM, false];
    }

    public static IEnumerable<string> ModuleNames() =>
        LoadModuleProjects()
            .Select(project => project.ModuleName)
            .OfType<string>()
            .Distinct(StringComparer.Ordinal)
            .OrderBy(moduleName => moduleName, StringComparer.Ordinal);

    public static IEnumerable<object[]> AssemblyDependencyRules()
    {
        foreach (var moduleName in ModuleNames())
        {
            var projectsByLayer = LoadModuleProjects()
                .Where(project => string.Equals(project.ModuleName, moduleName, StringComparison.Ordinal))
                .ToDictionary(project => project.Layer);

            foreach (var rule in CreateAssemblyDependencyRules(moduleName, projectsByLayer))
            {
                yield return [rule];
            }
        }
    }

    private static IEnumerable<AssemblyDependencyRule> CreateAssemblyDependencyRules(
        string moduleName,
        IReadOnlyDictionary<ModuleLayer, ProjectModel> projectsByLayer)
    {
        yield return CreateAssemblyDependencyRule(
            moduleName,
            ModuleLayer.AM,
            "contracts stay transport/domain/infrastructure-free",
            projectsByLayer,
            [ModuleLayer.DM, ModuleLayer.SL, ModuleLayer.PL, ModuleLayer.Web],
            "ResumeEnhancer.Infrastructure.Persistence",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore");

        yield return CreateAssemblyDependencyRule(
            moduleName,
            ModuleLayer.DM,
            "domain stays infrastructure-free",
            projectsByLayer,
            [ModuleLayer.AM, ModuleLayer.SL, ModuleLayer.PL, ModuleLayer.Web],
            "ResumeEnhancer.Infrastructure.Persistence",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore");

        yield return CreateAssemblyDependencyRule(
            moduleName,
            ModuleLayer.SL,
            "use cases do not depend on Web, PL, or infrastructure",
            projectsByLayer,
            [ModuleLayer.PL, ModuleLayer.Web],
            "ResumeEnhancer.Infrastructure.Persistence",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore");

        yield return CreateAssemblyDependencyRule(
            moduleName,
            ModuleLayer.Web,
            "HTTP boundary does not depend on persistence adapter",
            projectsByLayer,
            [ModuleLayer.PL],
            "Microsoft.EntityFrameworkCore");

        yield return CreateAssemblyDependencyRule(
            moduleName,
            ModuleLayer.PL,
            "persistence adapter does not depend on Web",
            projectsByLayer,
            [ModuleLayer.Web],
            "Microsoft.AspNetCore");
    }

    private static AssemblyDependencyRule CreateAssemblyDependencyRule(
        string moduleName,
        ModuleLayer layer,
        string description,
        IReadOnlyDictionary<ModuleLayer, ProjectModel> projectsByLayer,
        IReadOnlyCollection<ModuleLayer> forbiddenLayers,
        params string[] forbiddenDependencies)
    {
        if (!projectsByLayer.TryGetValue(layer, out var project))
        {
            throw new InvalidOperationException(
                $"Module '{moduleName}' does not contain the required '{layer}' project.");
        }

        var assembly = Assembly.Load(new AssemblyName(project.ProjectName));
        var moduleDependencies = forbiddenLayers
            .Select(forbiddenLayer => $"ResumeEnhancer.{moduleName}.{forbiddenLayer}")
            .Concat(forbiddenDependencies)
            .ToArray();

        return new AssemblyDependencyRule(
            $"{moduleName}.{layer} {description}",
            assembly,
            moduleDependencies);
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
    [MemberData(nameof(CoreAndInfrastructureProjectRules))]
    public void CoreAndInfrastructureProjects_ShouldRespectDependencyDirection(ProjectModel project)
    {
        var violations = project.ProjectReferences
            .Select(reference => new
            {
                Reference = reference,
                Message = GetSharedProjectReferenceViolation(project, reference)
            })
            .Where(item => item.Message is not null)
            .Select(item => $"{project.ProjectName} -> {item.Reference.DisplayName}: {item.Message}")
            .ToArray();

        violations.ShouldBeEmpty();
    }

    [Fact]
    public void ModuleInventory_ShouldContainExactlyOneRequiredProjectPerLayer()
    {
        var duplicateLayers = LoadModuleProjects()
            .GroupBy(project => (ModuleName: project.ModuleName!, project.Layer))
            .Where(group => group.Count() > 1)
            .OrderBy(group => group.Key.ModuleName, StringComparer.Ordinal)
            .ThenBy(group => group.Key.Layer)
            .Select(group => $"{group.Key.ModuleName}/{group.Key.Layer}: {string.Join(", ", group.Select(project => project.ProjectName).OrderBy(name => name, StringComparer.Ordinal))}")
            .ToArray();
        var incompleteModules = LoadModuleProjects()
            .GroupBy(project => project.ModuleName!, StringComparer.Ordinal)
            .Where(group => !RequiredModuleLayers.SetEquals(group.Select(project => project.Layer)))
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group => $"{group.Key}: found [{FormatLayers(group.Select(project => project.Layer).ToHashSet())}], expected [{FormatLayers(RequiredModuleLayers)}]")
            .ToArray();

        duplicateLayers.ShouldBeEmpty("Duplicate module layers: " + string.Join("; ", duplicateLayers));
        incompleteModules.ShouldBeEmpty("Incomplete modules: " + string.Join("; ", incompleteModules));
    }

    [Fact]
    public void ModuleProjects_ShouldNotBypassHostCompositionOrDatabaseMigration()
    {
        var violations = LoadModuleProjects()
            .SelectMany(project => project.ProjectReferences
                .Where(reference => reference.Area == ProjectArea.WebSolution
                    || reference.ProjectName.Equals("ResumeEnhancer.Infrastructure.DatabaseMigration", StringComparison.Ordinal))
                .Select(reference => $"{project.DisplayName} -> {reference.DisplayName}"))
            .OrderBy(violation => violation, StringComparer.Ordinal)
            .ToArray();

        violations.ShouldBeEmpty("Module host or migration bypasses: " + string.Join("; ", violations));
    }

    [Fact]
    public void ModuleProjectGraph_ShouldBeAcyclic()
    {
        var cycles = FindModuleCycles(LoadModuleProjects())
            .Select(cycle => string.Join(" -> ", cycle))
            .OrderBy(cycle => cycle, StringComparer.Ordinal)
            .ToArray();

        cycles.ShouldBeEmpty("Module project dependency cycles: " + string.Join("; ", cycles));
    }

    [Fact]
    public void ModuleProjectGraphCycleDiagnostic_ShouldIncludeOrderedCyclePath()
    {
        var a = CreateSyntheticModuleProject("A", ModuleLayer.DM);
        var b = CreateSyntheticModuleProject("B", ModuleLayer.DM);
        var aWithReference = a with { ProjectReferences = [CreateSyntheticModuleReference(b)] };
        var bWithReference = b with { ProjectReferences = [CreateSyntheticModuleReference(a)] };

        var cycles = FindModuleCycles([aWithReference, bWithReference]);

        cycles.ShouldContain(cycle => string.Join(" -> ", cycle) == "A/ResumeEnhancer.A.DM -> B/ResumeEnhancer.B.DM -> A/ResumeEnhancer.A.DM");
    }

    [Fact]
    public void CompiledModuleAssemblies_ShouldMatchProjectDependencyPolicy()
    {
        var applicationProjects = LoadApplicationProjects();
        var sourceProjects = applicationProjects
            .Where(project => project.Area is ProjectArea.Core or ProjectArea.Infrastructure or ProjectArea.Module)
            .OrderBy(project => project.ProjectName, StringComparer.Ordinal)
            .ToArray();
        var projectsByAssembly = applicationProjects.ToDictionary(project => project.ProjectName, StringComparer.Ordinal);
        var projectsByPath = applicationProjects.ToDictionary(
            project => NormalizeProjectPath(project.ProjectFile),
            StringComparer.OrdinalIgnoreCase);
        var violations = new List<string>();

        foreach (var project in sourceProjects)
        {
            var assembly = Assembly.Load(new AssemblyName(project.ProjectName));
            var declaredProjectReferenceClosure = GetDeclaredProjectReferenceClosure(project, projectsByPath);

            foreach (var referencedAssembly in assembly.GetReferencedAssemblies()
                .Where(reference => reference.Name is not null && projectsByAssembly.ContainsKey(reference.Name))
                .OrderBy(reference => reference.Name, StringComparer.Ordinal))
            {
                var target = projectsByAssembly[referencedAssembly.Name!];

                if (target.Area == ProjectArea.WebSolution
                    || target.ProjectName.Equals("ResumeEnhancer.Infrastructure.DatabaseMigration", StringComparison.Ordinal))
                {
                    violations.Add($"{project.DisplayName} assembly -> {target.DisplayName} assembly bypasses the host composition or migration boundary");
                    continue;
                }

                if (target.Area is ProjectArea.Module or ProjectArea.Core or ProjectArea.Infrastructure
                    && !declaredProjectReferenceClosure.Contains(NormalizeProjectPath(target.ProjectFile)))
                {
                    violations.Add($"{project.DisplayName} assembly -> {target.DisplayName} assembly is outside the declared project-reference closure");
                }
            }
        }

        violations.OrderBy(violation => violation, StringComparer.Ordinal).ToArray()
            .ShouldBeEmpty("Compiled module dependency violations: " + string.Join("; ", violations));
    }

    [Theory]
    [MemberData(nameof(CrossModuleReferencePolicyRules))]
    public void CrossModuleReferencePolicy_ShouldAllowOnlyDomainAndServiceLayerEdges(
        ModuleLayer sourceLayer,
        ModuleLayer targetLayer,
        bool expected)
    {
        IsAllowedCrossModuleReference(sourceLayer, targetLayer).ShouldBe(expected);
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
            "ResumeEnhancer.Infrastructure.DatabaseMigration.csproj"));
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
                return IsAllowedCrossModuleReference(project.Layer, reference.Layer)
                    ? null
                    : "cross-module project references are not allowed";
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

    private static string? GetSharedProjectReferenceViolation(ProjectModel project, ProjectReferenceModel reference)
    {
        if (project.Area == ProjectArea.Core)
        {
            return reference.Area == ProjectArea.Core
                ? null
                : "Core projects may reference only Core projects";
        }

        if (project.ProjectName.Equals("ResumeEnhancer.Infrastructure.Caching", StringComparison.Ordinal))
        {
            return reference.Area == ProjectArea.Core
                || reference.ProjectName.Equals("ResumeEnhancer.Infrastructure.Persistence", StringComparison.Ordinal)
                ? null
                : "Caching may reference only Core or shared Persistence";
        }

        if (project.ProjectName.Equals("ResumeEnhancer.Infrastructure.Persistence", StringComparison.Ordinal))
        {
            return reference.Area == ProjectArea.Core
                ? null
                : "Persistence may reference only Core projects";
        }

        if (project.ProjectName.Equals("ResumeEnhancer.Infrastructure.DatabaseMigration", StringComparison.Ordinal))
        {
            return reference.ProjectName.Equals("ResumeEnhancer.Infrastructure.Persistence", StringComparison.Ordinal)
                || (reference.Area == ProjectArea.Module && reference.Layer == ModuleLayer.PL)
                ? null
                : "Migration may reference only shared Persistence and module PL projects";
        }

        return "unsupported shared project area";
    }

    private static IReadOnlyList<IReadOnlyList<string>> FindModuleCycles(IEnumerable<ProjectModel> projects)
    {
        var byDisplayName = projects.ToDictionary(project => project.DisplayName, StringComparer.Ordinal);
        var edges = projects.ToDictionary(
            project => project.DisplayName,
            project => project.ProjectReferences
                .Where(reference => reference.Area == ProjectArea.Module && byDisplayName.ContainsKey(reference.DisplayName))
                .Select(reference => reference.DisplayName)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray(),
            StringComparer.Ordinal);
        var cycles = new HashSet<string>(StringComparer.Ordinal);

        foreach (var start in edges.Keys.OrderBy(name => name, StringComparer.Ordinal))
        {
            FindCycles(start, start, edges, [], cycles);
        }

        return cycles
            .OrderBy(cycle => cycle, StringComparer.Ordinal)
            .Select(cycle => (IReadOnlyList<string>)cycle.Split(" -> ", StringSplitOptions.None))
            .ToArray();
    }

    private static IReadOnlySet<string> GetDeclaredProjectReferenceClosure(
        ProjectModel project,
        IReadOnlyDictionary<string, ProjectModel> projectsByPath)
    {
        var closure = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var pending = new Stack<string>(project.ProjectReferences
            .Select(reference => NormalizeProjectPath(reference.ProjectFile))
            .OrderByDescending(path => path, StringComparer.OrdinalIgnoreCase));

        while (pending.Count > 0)
        {
            var projectPath = pending.Pop();
            if (!closure.Add(projectPath)
                || !projectsByPath.TryGetValue(projectPath, out var referencedProject))
            {
                continue;
            }

            foreach (var reference in referencedProject.ProjectReferences
                         .Select(reference => NormalizeProjectPath(reference.ProjectFile))
                         .OrderByDescending(path => path, StringComparer.OrdinalIgnoreCase))
            {
                pending.Push(reference);
            }
        }

        return closure;
    }

    private static string NormalizeProjectPath(string projectFile) =>
        Path.GetFullPath(projectFile)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static void FindCycles(
        string start,
        string current,
        IReadOnlyDictionary<string, string[]> edges,
        IReadOnlyList<string> path,
        ISet<string> cycles)
    {
        var nextPath = path.Append(current).ToArray();
        foreach (var target in edges[current])
        {
            if (target.Equals(start, StringComparison.Ordinal))
            {
                var cycle = nextPath.Append(start).ToArray();
                var cycleWithoutClosingNode = cycle[..^1];
                var rotations = Enumerable.Range(0, cycleWithoutClosingNode.Length)
                    .Select(index => cycleWithoutClosingNode.Skip(index).Concat(cycleWithoutClosingNode.Take(index)).ToArray())
                    .Select(rotation => string.Join(" -> ", rotation.Append(rotation[0])))
                    .OrderBy(value => value, StringComparer.Ordinal);
                cycles.Add(rotations.First());
            }
            else if (!nextPath.Contains(target, StringComparer.Ordinal))
            {
                FindCycles(start, target, edges, nextPath, cycles);
            }
        }
    }

    private static ProjectModel CreateSyntheticModuleProject(string moduleName, ModuleLayer layer) =>
        new(
            $"{moduleName}.csproj",
            $"ResumeEnhancer.{moduleName}.{layer}",
            ProjectArea.Module,
            moduleName,
            layer,
            [],
            []);

    private static ProjectReferenceModel CreateSyntheticModuleReference(ProjectModel project) =>
        new(project.ProjectFile, project.ProjectName, project.Area, project.ModuleName, project.Layer);

    private static IReadOnlySet<ModuleLayer> GetAllowedModuleReferenceLayers(ModuleLayer sourceLayer) =>
        sourceLayer switch
        {
            ModuleLayer.AM => new HashSet<ModuleLayer> { ModuleLayer.DM },
            ModuleLayer.DM => new HashSet<ModuleLayer> { ModuleLayer.DM },
            ModuleLayer.SL => new HashSet<ModuleLayer> { ModuleLayer.AM, ModuleLayer.DM, ModuleLayer.SL },
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
            ? new HashSet<string> { "ResumeEnhancer.Infrastructure.Caching", "ResumeEnhancer.Infrastructure.Persistence" }
            : new HashSet<string>();

    private static bool IsAllowedCrossModuleReference(ModuleLayer sourceLayer, ModuleLayer targetLayer) =>
        (sourceLayer, targetLayer) switch
        {
            (ModuleLayer.DM, ModuleLayer.DM) => true,
            (ModuleLayer.SL, ModuleLayer.SL) => true,
            _ => false
        };

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

    private static IReadOnlyList<ProjectModel> LoadApplicationProjects() =>
        Directory.EnumerateFiles(ApplicationRoot, "*.csproj", SearchOption.AllDirectories)
            .Select(LoadProject)
            .ToArray();

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
            var gitPath = Path.Combine(directory.FullName, ".git");
            if ((Directory.Exists(gitPath) || File.Exists(gitPath))
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

    private static readonly IReadOnlySet<ModuleLayer> RequiredModuleLayers =
        new HashSet<ModuleLayer> { ModuleLayer.AM, ModuleLayer.DM, ModuleLayer.SL, ModuleLayer.PL, ModuleLayer.Web };

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




