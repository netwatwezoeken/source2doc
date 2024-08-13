using System.Collections.Generic;
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
    
    private Compiler _compiler;
    private Analyzer _analyzer;
    private IEnumerable<Dependency> _dependencyList;
    private DependencyModel _dependenciyGroups;

    [Given(@"these dependencies")]
    public void GivenTheseDependencies(DataTable table)
    {
        _dependencyList = table.CreateSet<Dependency>();
    }
    
    [When(@"grouped")]
    public void GivenTheseDependencies()
    {
        _dependenciyGroups = Grouping.GroupDependencies(_dependencyList);
    }
    
    [Then(@"{int} groups exist")]
    public void GivenTheseProductsExist(int number)
    {
        Assert.Equal(number, _dependenciyGroups.Groups.Count);
    }
        
    [Then("group {int} number {int} is {string}")]
    public void NumberOfDependencies(int group, int index, string dependency)
    {
        Assert.Equal(dependency, _dependenciyGroups.Groups[group].Dependencies[index].From.Id.ToString());
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
        return new CSharpType(new CSharpTypeIdentifier(keyValuePair.Value));
    }
}