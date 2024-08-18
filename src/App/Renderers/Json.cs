using System.Text.Json;
using System.Text.Json.Serialization;

namespace App.Renderers;

public class Json : IRenderer
{
    public void Dispose()
    {
        // TODO release managed resources here
    }

    public async Task<Stream> Render(DependencyModel model)
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(JsonSerializer.Serialize(model.Groups, options));
        await writer.FlushAsync();
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}