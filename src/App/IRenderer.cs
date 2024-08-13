namespace App;

public interface IRenderer : IDisposable
{
    Task<Stream> Render(DependencyModel model);
}