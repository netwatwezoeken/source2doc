namespace App;

public record DependencyModel(IList<DependencyGroup> Groups);

public record DependencyGroup(IList<CSharpType> Types, IList<Dependency> Dependencies);