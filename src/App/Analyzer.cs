using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace App;

public class Analyzer(Workspace workspace, Compiler compiler, string[] events, string[] handlers)
{
    public async Task Analyze()
    {
        Dependencies = new ();
        var eventThrowwers = new HashSet<Dependency>();

        var eventSymbols = compiler.Symbols.Where(s =>
            s.AllInterfaces.Select(i => $"{i.FullNamespace()}.{i.Name}").Any(events.Contains))
            .ToDictionary(e => e.Name, e => e);
        var eventHandlers = compiler.Symbols.Where(s =>
            s.AllInterfaces.Select(i => $"{i.FullNamespace()}.{i.Name}").Any(handlers.Contains));
        
        foreach (var symbol in compiler.Symbols)
        {
            if (!eventSymbols.TryGetValue(symbol.Name, out var matchedEvent))
                continue;

            await FindEventThrowers(matchedEvent, eventThrowwers);

            foreach (var handler in eventHandlers.Select(e => (ITypeSymbol)e)
                         .Where(e => e.AllInterfaces.Any(i => i.TypeArguments.FirstOrDefault()?.Name ==
                                                              matchedEvent.Name)))
            {
                var from = new CSharpType(new CSharpTypeIdentifier(symbol.FullNamespace(), symbol.Name), Type.Event);
                Types.Add(from);
                var to = new CSharpType(new CSharpTypeIdentifier(handler.FullNamespace(), handler.Name), Type.Handler);
                Types.Add(to);
                Dependencies.Add(new Dependency(from.Id, to.Id, null));
            }
        }
        
        Dependencies.AddRange(eventThrowwers.ToList());
        
        await AddDerivedClasses();
    }

    private async Task FindEventThrowers(INamedTypeSymbol matchedEvent, HashSet<Dependency> eventThrowwers)
    {
        var usages = await SymbolFinder.FindReferencesAsync(matchedEvent, workspace.CurrentSolution);
        var locations = usages.Where(u => u.Definition.Kind == SymbolKind.Method &&
                                          (u.Definition as IMethodSymbol)?.MethodKind == MethodKind.Constructor)
            .SelectMany(u => u.Locations);
        foreach (var loc in locations)
        {
            // ignore references that call a base constructor
            if (loc.Location?.SourceTree == null || loc.Location?.SourceSpan == null)
            {
                continue;
            }
            if(
               (await loc.Location.SourceTree.GetTextAsync()).GetSubText(loc.Location.SourceSpan).ToString().StartsWith("base"))
            {
                continue;
            }
            try
            {
                var throwerSymbol = compiler.Symbols.Single(s =>
                    s.DeclaringSyntaxReferences.Any(_ =>
                        _.SyntaxTree.FilePath == loc.Document.Name
                        && _.Span.Start <= loc.Location.SourceSpan.Start
                        && _.Span.End >= loc.Location.SourceSpan.End));
                var from = new CSharpType(new CSharpTypeIdentifier(throwerSymbol.FullNamespace(), throwerSymbol.Name), Type.Publisher);
                Types.Add(from);
                var to = new CSharpType(new CSharpTypeIdentifier(matchedEvent.FullNamespace(), matchedEvent.Name), Type.Event);
                Types.Add(to);
                eventThrowwers.Add(new Dependency(from.Id, to.Id, loc.Location?.GetMappedLineSpan().StartLinePosition.ToDomain()));
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"Multiple matches on {loc.Document.Name}");
                var throwerSymbol = compiler.Symbols.First(s =>
                    s.DeclaringSyntaxReferences.Any(_ =>
                        _.SyntaxTree.FilePath == loc.Document.Name
                        && loc.Location != null
                        && _.Span.Start <= loc.Location.SourceSpan.Start
                        && _.Span.End >= loc.Location.SourceSpan.End));
                
                var from = new CSharpType(new CSharpTypeIdentifier(throwerSymbol.FullNamespace(), throwerSymbol.Name), Type.Event);
                Types.Add(from);
                var to = new CSharpType(new CSharpTypeIdentifier(matchedEvent.FullNamespace(), matchedEvent.Name), Type.Handler);
                Types.Add(to);
                eventThrowwers.Add(new Dependency(from.Id, to.Id, loc.Location?.GetMappedLineSpan().StartLinePosition.ToDomain()));
            }
        }
    }

    private async Task AddDerivedClasses()
    {
        var derivateDependencies = new HashSet<Dependency>();
        foreach (var dependency in Dependencies)
        {
            var symbol = compiler.Symbols.FirstOrDefault(s => s.Name == dependency.From.Name);
            if (symbol == null) continue;
            var derivatives = await SymbolFinder.FindDerivedClassesAsync(symbol, workspace.CurrentSolution);
            foreach (var derivative in derivatives)//.Select(derivative => 
                        // new Dependency(new CSharpTypeIdentifier(derivative.FullNamespace(), derivative.Name), dependency.From, null)))
            {
                var from = new CSharpTypeIdentifier(derivative.FullNamespace(), derivative.Name);
                var dep = new Dependency(from, dependency.From,
                    null);
                var type = Types.Single(t => 
                    t.Id == new CSharpTypeIdentifier(dependency.From.Namespace, dependency.From.Name))
                    .Type;
                Types.Add(new CSharpType(from, type));
                derivateDependencies.Add(dep);
            }
        }
        Dependencies.AddRange(derivateDependencies);
    }

    public DependencyGroup Data => new(Types.ToList(), Dependencies);

    private List<Dependency> Dependencies { get; set; } = new ();
    
    private HashSet<CSharpType> Types { get; set; } = new ();
}