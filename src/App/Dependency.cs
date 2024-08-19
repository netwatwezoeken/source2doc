namespace App;

public record Dependency(CSharpTypeIdentifier From, CSharpTypeIdentifier To, LinePosition? Position);