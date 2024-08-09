namespace App;

public static class Grouping
{
    public static IEnumerable<IEnumerable<Dependency>> GroupDependencies(IEnumerable<Dependency> dependencies)
    {
        var depGroups = new List<List<Dependency>>();
        var dict = dependencies.ToDictionary(h => h, h => false);
        // var dict = dependencies.ToDictionary(h => h , h => false);
        List<Dependency> groupList = [];
        foreach (var dep in dict)
        {
            var closeGroup = false;
            if (!dict[dep.Key])
            {
                closeGroup = true;
                groupList = [];
            }

            AddRecursive(dict, dep.Key, groupList);

            void AddRecursive(Dictionary<Dependency, bool> dictionary,
                Dependency depKey, List<Dependency> groupList)
            {
                if (!dictionary[depKey])
                {
                    dictionary[depKey] = true;
                    groupList.Add(depKey);
                    foreach (var thing in dictionary.Where(k =>
                                 k.Key.From.Name == depKey.From.Name ||
                                 k.Key.From.Name == depKey.To.Name ||
                                 k.Key.To.Name == depKey.From.Name ||
                                 k.Key.To.Name == depKey.To.Name
                             ))
                    {
                        AddRecursive(dictionary, thing.Key, groupList);
                    }
                }
            }

            if (closeGroup)
            {
                depGroups.Add(groupList);
            }
        }

        return depGroups;
    }
}