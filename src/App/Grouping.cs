namespace App;

public static class Grouping
{
    public static DependencyModel GroupDependencies(DependencyGroup dependencies)
    {
        var depGroups = new DependencyModel(new List<DependencyGroup>{});
        var dict = dependencies.Dependencies.ToDictionary(h => h, h => false);
        var group = new DependencyGroup(new List<CSharpType>(), new List<Dependency>());
        foreach (var dep in dict)
        {
            var closeGroup = false;
            if (!dict[dep.Key])
            {
                closeGroup = true;
                group = new DependencyGroup(new List<CSharpType>(), new List<Dependency>());
            }

            AddRecursive(dict, dep.Key);

            void AddRecursive(Dictionary<Dependency, bool> dictionary,
                Dependency depKey)
            {
                if (!dictionary[depKey])
                {
                    dictionary[depKey] = true;
                    group.Dependencies.Add(depKey);
                    foreach (var thing in dictionary.Where(k =>
                                 k.Key.From == depKey.From ||
                                 k.Key.From == depKey.To ||
                                 k.Key.To == depKey.From ||
                                 k.Key.To == depKey.To
                             ))
                    {
                        AddRecursive(dictionary, thing.Key);
                    }
                }
            }

            if (closeGroup)
            {
                var deplist = group.Dependencies.Select(d => d.From).ToList();
                deplist.AddRange(group.Dependencies.Select(d => d.To));

                var types = dependencies.Types.Where(t =>
                    deplist.Contains(t.Id)).OrderBy(t => t.Id.ToString()).ToList();
                
                var sortedGroup = new DependencyGroup(
                    Dependencies: group.Dependencies.OrderBy(d => d.From.ToString()).ToList(), 
                    Types: types.ToList());
                
                depGroups.Groups.Add(sortedGroup);
            }
        }
        
        return depGroups;
    }
}