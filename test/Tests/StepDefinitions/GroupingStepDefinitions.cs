using System;
using System.Collections.Generic;
using System.Linq;
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
        //Service.Instance.ValueRetrievers.Register(new CSharpTypeRetriever());
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
    
    [Then(@"{int} groups exist")]
    public void GivenTheseProductsExist(int number)
    {
        Assert.Equal(number, _dependenciyGroups.Groups.Count);
    }
        
    [Then("group {int} number {int} is {string}")]
    public void IndexOfDependencies(int group, int index, string dependency)
    {
        Assert.Equal(dependency, _dependenciyGroups.Groups[group].Dependencies[index].From.ToString());
    }
    
    [Then("group {int} type at index {int} is {string}")]
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

// public class CSharpTypeRetriever : IValueRetriever
// {
//     public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, System.Type targetType, System.Type propertyType)
//     {
//         if (!keyValuePair.Key.Equals("Identifier"))
//         {
//             return false;
//         }
//         return true;
//     }
//
//     public object Retrieve(KeyValuePair<string, string> keyValuePair, System.Type targetType, System.Type propertyType)
//     {
//         return new CSharpType(new CSharpTypeIdentifier(keyValuePair.Value), Type.Event);
//     }
// }