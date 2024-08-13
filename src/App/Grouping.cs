namespace App;

public static class Grouping
{
    public static DependencyModel GroupDependencies(DependencyGroup dependencies)
    {
        return GroupDependencies(dependencies.Dependencies);
    }

    private static DependencyModel GroupDependencies(IEnumerable<Dependency> dependencies)
    {
        var depGroups = new DependencyModel(new List<DependencyGroup>{});
        var dict = dependencies.ToDictionary(h => h, h => false);
        List<Dependency> groupList = [];
        var group = new DependencyGroup(new List<CSharpType>(), new List<Dependency>());
        foreach (var dep in dict)
        {
            var closeGroup = false;
            if (!dict[dep.Key])
            {
                closeGroup = true;
                group = new DependencyGroup(new List<CSharpType>(), new List<Dependency>());
            }

            AddRecursive(dict, dep.Key, groupList);

            void AddRecursive(Dictionary<Dependency, bool> dictionary,
                Dependency depKey, List<Dependency> groupList)
            {
                if (!dictionary[depKey])
                {
                    dictionary[depKey] = true;
                    groupList.Add(depKey);
                    group.Dependencies.Add(depKey);
                    foreach (var thing in dictionary.Where(k =>
                                 k.Key.From == depKey.From ||
                                 k.Key.From == depKey.To ||
                                 k.Key.To == depKey.From ||
                                 k.Key.To == depKey.To
                             ))
                    {
                        AddRecursive(dictionary, thing.Key, groupList);
                    }
                }
            }

            if (closeGroup)
            {
                var sortedGroup = group with {Dependencies = 
                    group.Dependencies.OrderBy(d => d.From.Id.ToString()).ToList()};
                depGroups.Groups.Add(sortedGroup);
            }
        }
        
        return depGroups;
    }
}