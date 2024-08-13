namespace App.Renderers;

public class MermaidMarkdown : IRenderer
{
    public void Dispose()
    {
        // TODO release managed resources here
    }

    public async Task<Stream> Render(DependencyModel model)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        
        foreach (var group in model.Groups)
        {
            await writer.WriteLineAsync("```mermaid");
            await writer.WriteLineAsync("flowchart LR;");
            foreach (var dependency in group.Dependencies)
            {
                await writer.WriteLineAsync($"    {dependency.From.Id}-->{dependency.To.Id};");
            }
            await writer.WriteLineAsync("```");
        }
        await writer.FlushAsync();
        stream.Seek(0, SeekOrigin.Begin);
        
        return stream;
    }
}