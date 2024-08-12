namespace App;

public record CSharpTypeIdentifier(string Namespace, string Name);

public record CSharpType(CSharpTypeIdentifier Id, Type Type = Type.NotSpecified);

public enum Type
{
    NotSpecified,
    Event,
    Publisher,
    Handler
}