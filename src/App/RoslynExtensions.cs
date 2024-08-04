using Microsoft.CodeAnalysis;

namespace App;

public static class RoslynExtensions
{
    public static string FullNamespace(this ISymbol type)
    {
        var name = "";
        var ns = type.ContainingNamespace;
        while (ns != null && !ns.IsGlobalNamespace)
        {
            if (string.IsNullOrEmpty(name))
                name += ns.Name;
            else
                name = ns.Name + "." + name;
            ns = ns.ContainingNamespace;
        }

        return name;
    }

    public static LinePosition ToDomain(this Microsoft.CodeAnalysis.Text.LinePosition position) =>
        new LinePosition(position.Line, position.Character);
}