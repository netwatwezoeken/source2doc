using System.Reflection;
using App.Renderers;
using CommandLine;
using CommandLine.Text;

namespace App;

internal static class Program
{
    private static string _versionString = null!;

    private static async Task Main(string[] args)
    {
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        _versionString = $"source2doc {version!.InformationalVersion}";

        var parser = new Parser(with =>
        {
            with.HelpWriter = null;
            with.CaseInsensitiveEnumValues = true;
        });
        var t =  parser.ParseArguments<Options>(args);
        await t.WithParsedAsync(RunOptions);
        t.WithNotParsed(errs => DisplayHelp(t));
    }

    private static async Task RunOptions(Options opts)
    {
        Console.WriteLine(_versionString);
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
        var groups = Grouping.GroupDependencies(analyzer.Data);

        IRenderer? renderer = null;
        switch (opts.Format)
        {
            case Format.Json:
            {
                renderer = new Json();
                break;
            }
            case Format.MermaidMd:
            default:
            {
                renderer = new MermaidMarkdown();
                break;
            }
        }

        var stream = await renderer.Render(groups);
        var output = await new StreamReader(stream).ReadToEndAsync();
        Console.Write(output);
        renderer.Dispose();
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
    
    static void DisplayHelp<T>(ParserResult<T> result)
    {  
        var helpText = HelpText.AutoBuild(result, h =>
        {
            h.AdditionalNewLineAfterOption = false;
            h.Heading = _versionString;
            h.Copyright = "Copyright (c) 2024 .NET wat we zoeken";
            return HelpText.DefaultParsingErrorsHandler(result, h);
        }, e => e);
        Console.WriteLine(helpText);
    }
}