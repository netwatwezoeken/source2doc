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
            s.AllInterfaces.Select(i => $"{i.FullNamespace()}.{i.Name}").Any(events.Contains));
        var eventHandlers = compiler.Symbols.Where(s =>
            s.AllInterfaces.Select(i => $"{i.FullNamespace()}.{i.Name}").Any(handlers.Contains)); ;
        
        foreach (var symbol in compiler.Symbols)
        {
            var matchedEvents = eventSymbols.Where(s => s.Name == symbol.Name);
            foreach (var matchedEvent in matchedEvents)
            {
                await FindEventThrowers(matchedEvent, eventThrowwers);

                foreach (var handler in eventHandlers.Select(e => (ITypeSymbol)e)
                             .Where(e => e.AllInterfaces.FirstOrDefault()?.TypeArguments.FirstOrDefault()?.Name ==
                                         matchedEvent.Name))
                {
                    Dependencies.Add(new Dependency(new CSharpType(symbol.Name, Type.Event), 
                        new CSharpType(handler.Name, Type.Handler), null));
                }
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
            // ignore references that call a base contructor
            if(loc.Location.SourceTree != null && 
               loc.Location.SourceTree.GetTextAsync().Result.GetSubText(loc.Location.SourceSpan).ToString().StartsWith("base"))
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
                eventThrowwers.Add(new Dependency(new CSharpType(throwerSymbol.Name, Type.Publisher),
                    new CSharpType(matchedEvent.Name, Type.Event),
                    loc.Location.GetMappedLineSpan().StartLinePosition.ToDomain()));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Multiple matches on {loc.Document.Name}");
                var throwerSymbol = compiler.Symbols.First(s =>
                    s.DeclaringSyntaxReferences.Any(_ =>
                        _.SyntaxTree.FilePath == loc.Document.Name
                        && _.Span.Start <= loc.Location.SourceSpan.Start
                        && _.Span.End >= loc.Location.SourceSpan.End));
                eventThrowwers.Add(new Dependency(new CSharpType(throwerSymbol.Name, Type.Publisher),
                    new CSharpType(matchedEvent.Name, Type.Event),
                    loc.Location.GetMappedLineSpan().StartLinePosition.ToDomain()));
            }
        }
    }

    private async Task AddDerivedClasses()
    {
        var derivateDependencies = new List<Dependency>();
        foreach (var dependency in Dependencies)
        {
            var symbol = compiler.Symbols.FirstOrDefault(s => s.Name == dependency.From.Name);
            if (symbol == null) continue;
            var derivatives = await SymbolFinder.FindDerivedClassesAsync(symbol, workspace.CurrentSolution);
            derivateDependencies.AddRange(derivatives.Select(derivative => 
                new Dependency(new CSharpType(derivative.Name, dependency.From.Type), dependency.From, null)));
        }
        Dependencies.AddRange(derivateDependencies);
    }

    public static List<Dependency> Dependencies { get; set; } = new ();
}