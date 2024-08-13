namespace App;

public record CSharpTypeIdentifier(string Namespace, string Name)
{
    public CSharpTypeIdentifier(string fullyQualifiedName) : this(
        fullyQualifiedName.ToClassIdentifier().FullNamespace,
        fullyQualifiedName.ToClassIdentifier().Name
        )
    { }
    public override string ToString()
    {
        return Namespace + "." + Name;
    }
};

public record CSharpType(CSharpTypeIdentifier Id, Type Type = Type.NotSpecified);

public enum Type
{
    NotSpecified,
    Event,
    Publisher,
    Handler
}