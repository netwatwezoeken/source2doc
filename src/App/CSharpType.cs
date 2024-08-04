namespace App;

public record CSharpType(string Name, Type Type = Type.NotSpecified);

public enum Type
{
    NotSpecified,
    Event,
    Publisher,
    Handler
}