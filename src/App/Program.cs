using System.Reflection;
using CommandLine;

namespace App;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithNotParsed(HandleParseError);
        await Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync(RunOptions);
    }

    private static async Task RunOptions(Options opts)
    {
        var versionString = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        Console.WriteLine($"source2doc v{versionString!.InformationalVersion}");
        var sourcePath = opts.Source.ToAbsolutePath();

        if (!Path.Exists(sourcePath))
        {
            Console.WriteLine($"Directory \"{sourcePath}\" does not exist.");
            return;
        }

        foreach (var library in opts.Libraries)
        {
            var file = library.ToAbsolutePath();
            if (File.Exists(file)) continue;
            Console.WriteLine($"File \"{file}\" does not exist.");
            return;
        }
        
        var compiler = new Compiler(sourcePath.ToAbsolutePath(), 
            opts.Libraries.Select(l => l.ToAbsolutePath()).ToArray());
        
        await compiler.Initialize();
        var analyzer = new Analyzer(compiler.Workspace, compiler, 
            opts.Events.ToArray(), opts.Handlers.ToArray());
        await analyzer.Analyze();
        var groups = Grouping.GroupDependencies(analyzer.Dependencies);

        Render(groups);
    }

    private static void Render(IEnumerable<IEnumerable<Dependency>> groups)
    {
        foreach (var group in groups)
        {
            Console.WriteLine("```mermaid");
            Console.WriteLine("flowchart LR;");
            foreach (var dependency in group)
            {
                Console.WriteLine($"    {dependency.From.Name}-->{dependency.To.Name};");
            }
            Console.WriteLine("```");
        }
    }

    private static string ToAbsolutePath(this string input)
    {
        var path = Directory.GetCurrentDirectory();
        path = Path.IsPathRooted(input) ?
            input : 
            Path.Join(path, input);
        return path;
    }

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        //handle errors
    }
}