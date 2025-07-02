using System.Collections.Generic;
using Reqnroll.Assist;
using Type = App.Type;

namespace Tests.StepDefinitions;

[Binding]
public sealed class GroupingStepDefinitions
{
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Service.Instance.ValueRetrievers.Register(new CSharpTypeIdentifierRetriever());
        Service.Instance.ValueRetrievers.Register(new TypeRetriever());
    }

    private DependencyModel _dependenciyGroups;

    private DependencyGroup _unsortedGroup = new DependencyGroup(
        new List<CSharpType>(), 
        new List<Dependency>());

    [Given(@"these types")]
    public void GivenTheseTypes(DataTable table)
    {
         var typeList = table.CreateSet<CSharpType>();
         foreach (var type in typeList)
         {
             _unsortedGroup.Types.Add(type);
         }
    }
    
    [Given(@"these dependencies")]
    public void GivenTheseDependencies(DataTable table)
    {
        var dependencyList = table.CreateSet<Dependency>();
        foreach (var dep in dependencyList)
        {
            _unsortedGroup.Dependencies.Add(dep);
        }
    }
    
    [When(@"grouped")]
    public void GivenTheseDependencies()
    {
        _dependenciyGroups = Grouping.GroupDependencies(_unsortedGroup);
    }
    
    [Then("""
          "(.*)" groups exist
          """)]
    public void GivenTheseProductsExist(int number)
    {
        Assert.Equal(number, _dependenciyGroups.Groups.Count);
    }
        
    [Then(@"group (.*) number (.*) is (.*)")]
    public void IndexOfDependencies(int group, int index, string dependency)
    {
        Assert.Equal(dependency, _dependenciyGroups.Groups[group].Dependencies[index].From.ToString());
    }

    [Then(@"group (.*) type at index (.*) is (.*)")]
    public void IndexOfTypes(int group, int index, string dependency)
    {
        Assert.Equal(dependency, _dependenciyGroups.Groups[group].Types[index].Id.ToString());
    }
}

public class CSharpTypeIdentifierRetriever : IValueRetriever
{
    public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, System.Type targetType, System.Type propertyType)
    {
        if (!keyValuePair.Key.Equals("From")
            && !keyValuePair.Key.Equals("To")
            && !keyValuePair.Key.Equals("Id"))
        {
            return false;
        }
        return true;
    }

    public object Retrieve(KeyValuePair<string, string> keyValuePair, System.Type targetType, System.Type propertyType)
    {
        return new CSharpTypeIdentifier(keyValuePair.Value);
    }
}

public class TypeRetriever : IValueRetriever
{
    public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, System.Type targetType, System.Type propertyType)
    {
        if (!keyValuePair.Key.Equals("Type"))
        {
            return false;
        }
        return true;
    }

    public object Retrieve(KeyValuePair<string, string> keyValuePair, System.Type targetType, System.Type propertyType)
    {
        return Type.Publisher;
    }
}