using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace App;

public class Compiler(string path, string[] assemblyFiles)
{
    public Compilation Compilation { get; private set; } = null!;
    public List<INamedTypeSymbol> Symbols { get; set; } = [];
    public Workspace Workspace { get; set; } = null!;

    public async Task Initialize()
    {
        (Compilation, Workspace) = await Compile();
    }

    public async Task<(Compilation compilation, Workspace workspace)> Compile()
    {
        var workspace = new AdhocWorkspace();
        var solutionInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Default);

        workspace.AddSolution(solutionInfo);

        var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Default, "AnalysisProject",
            "AnalysisProject", "C#");
        workspace.AddProject(projectInfo);

        AddFolder(path, workspace, projectInfo.Id);

        var project = workspace.CurrentSolution.Projects.Single();

        project = assemblyFiles.Select(assemblyFile => 
            MetadataReference.CreateFromFile(assemblyFile))
                .Aggregate(project, (current, metaref) => current.AddMetadataReference(metaref));

        var comp = await project.GetCompilationAsync().ConfigureAwait(false);

        if (comp == null) throw new Exception("Could not get compilation");
        
        GetSymbols(comp);
        return (comp, workspace);
    }

    private void AddFolder(string pathName, AdhocWorkspace ws, ProjectId projectId)
    {
        var f = new FileInfo(pathName);
        DirectoryInfo? di = null;
        IEnumerable<FileInfo> files;
        if (f.Exists)
        {
            files = new [] { f };
        }
        else
        {
            di = new DirectoryInfo(pathName);
            files = di.GetFiles("*.cs");
        }

        foreach (var file in files)
        {
            var sourceText = SourceText.From(File.OpenRead(file.FullName));
            ws.AddDocument(projectId, file.Name, sourceText);
        }

        if (di == null) return;
    
        foreach (var dir in di.GetDirectories())
        {
            AddFolder(dir.FullName, ws, projectId);
        }
    }
    
    private void GetSymbols(Compilation compilation)
    {
        HashSet<INamespaceSymbol> namespaces = [];
        AddNamespaceRecursive(compilation.GlobalNamespace, namespaces);
        List<INamedTypeSymbol> symbols = [];
        foreach (var ns in namespaces)
        {
            symbols.AddRange(Types(ns));
        }

        Symbols = symbols
            .OrderBy(t => t.Name)
            .ThenBy(t => t.FullNamespace()).ToList();
    }

    private IEnumerable<INamedTypeSymbol> Types(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers().OfType<INamedTypeSymbol>())
        {
            if (type.Locations.Any(l => l.Kind != LocationKind.SourceFile 
                                        && l.Kind != LocationKind.MetadataFile))
                continue;

            yield return type;
        }
    }

    private void AddNamespaceRecursive(INamespaceSymbol namespaceSymbol, HashSet<INamespaceSymbol> namespaceList)
    {
        foreach (var childNamespace in namespaceSymbol
                     .GetMembers().OfType<INamespaceSymbol>()
                     .Where(n => n.Locations.Any(l => l.Kind == LocationKind.SourceFile)))
        {
            namespaceList.Add(childNamespace);
            AddNamespaceRecursive(childNamespace, namespaceList);
        }
        foreach (var childNamespace in namespaceSymbol
                     .GetMembers().OfType<INamespaceSymbol>()
                     .Where(n => n.Locations.Any(l => l.Kind == LocationKind.MetadataFile)))
        {
            namespaceList.Add(childNamespace);
            AddNamespaceRecursive(childNamespace, namespaceList);
        }
    }
}