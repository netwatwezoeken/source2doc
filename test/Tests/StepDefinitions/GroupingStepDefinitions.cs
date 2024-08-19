using System.Collections.Generic;
using System.Linq;
using Reqnroll.Assist;

namespace Tests.StepDefinitions;

[Binding]
public sealed class GroupingStepDefinitions
{
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Service.Instance.ValueRetrievers.Register(new CSharpTypeRetriever());
    }
    
    private DependencyModel _dependenciyGroups;
    private DependencyGroup _unsortedGroup;

    [Given(@"these dependencies")]
    public void GivenTheseDependencies(DataTable table)
    {
         var dependencyList = table.CreateSet<Dependency>();
        _unsortedGroup = new DependencyGroup(
            new List<CSharpType>(), 
            dependencyList.ToList());
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
    public void NumberOfDependencies(int group, int index, string dependency)
    {
        Assert.Equal(dependency, _dependenciyGroups.Groups[group].Dependencies[index].From.ToString());
    }
}

public class CSharpTypeRetriever : IValueRetriever
{
    public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, System.Type targetType, System.Type propertyType)
    {
        if (!keyValuePair.Key.Equals("From")
            && !keyValuePair.Key.Equals("To"))
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