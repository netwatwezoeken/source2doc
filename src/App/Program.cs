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
        var dependencies = analyzer.Dependencies;

        DependenciesToMermaids(dependencies);
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
    
    static void DependenciesToMermaids(IEnumerable<Dependency> dependencies)
    {
        var dict = dependencies.ToDictionary(h => h , h => false);
        // var dict = dependencies.ToDictionary(h => h , h => false);
        foreach (var dep in dict)
        {
            var generateHeaderFooter = false;
            if (!dict[dep.Key])
            {
                generateHeaderFooter = true;
                Console.WriteLine("```mermaid");
                Console.WriteLine("flowchart LR;");
            }

            var names = new List<string>();
            AddRecursive(dict, dep.Key, names);
            void AddRecursive(Dictionary<Dependency, bool> dictionary,
                Dependency depKey, List<string> names)
            {
                if (!dictionary[depKey])
                {
                    dictionary[depKey] = true;
                    Console.WriteLine($"    {depKey.From.Name}-->{depKey.To.Name};");
                    foreach (var thing in dictionary.Where(k =>
                                 k.Key.From.Name == depKey.From.Name ||
                                 k.Key.From.Name == depKey.To.Name ||
                                 k.Key.To.Name == depKey.From.Name ||
                                 k.Key.To.Name == depKey.To.Name
                             ))
                    {
                        //names.Add(thing.Key.From);
                        //names.Add(thing.Key.To);
                        AddRecursive(dictionary, thing.Key, names);
                    }
                }
            }

            if (generateHeaderFooter)
            {
                Console.WriteLine("```");
            }
        }
    }
}