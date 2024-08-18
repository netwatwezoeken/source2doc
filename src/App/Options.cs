using CommandLine;

namespace App;

public class Options
{
    [Option('e', "events", Required = true, HelpText = "Events to process.")]
    public required IEnumerable<string> Events { get; set; }
    
    [Option('h', "handlers", Required = true, HelpText = "Handlers to process.")]
    public required IEnumerable<string> Handlers { get; set; }
    
    [Option('s', "source", Required = false, HelpText = "path to source code. default is './'")]
    public required string Source { get; set; }
    
    [Option('l', "libraries", Required = false, HelpText = "additional libraries to process.")]
    public IEnumerable<string> Libraries  { get; set; } = [];

    [Option('f', "format", Required = false, HelpText = "'json' or 'mermaidmd'. (default is mermaidmd)")]
    public Format Format { get; set; } = Format.MermaidMd;
    
    [Option('o', "output", Required = false, HelpText = "write to specified file")]
    public string? File { get; set; }
}

public enum Format
{
    Json,
    MermaidMd
}