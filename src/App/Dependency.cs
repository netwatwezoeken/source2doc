namespace App;

public record Dependency(CSharpType From, CSharpType To, LinePosition? Position);