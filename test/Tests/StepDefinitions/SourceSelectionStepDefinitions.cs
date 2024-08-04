using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Reqnroll.UnitTestProvider;

namespace Tests.StepDefinitions;

[Binding]
public sealed class SourceSelectionStepDefinitions
{
    private Compiler _compiler;
    private readonly IUnitTestRuntimeProvider _unitTestRuntimeProvider;
    private Analyzer _analyzer;

    public SourceSelectionStepDefinitions(IUnitTestRuntimeProvider unitTestRuntimeProvider)
    {
        _unitTestRuntimeProvider = unitTestRuntimeProvider;
    }
    
    [Given("a compiler with path {string} and mediatr libs")]
    public async Task GivenASinglePathAndMediatrLibs(string path)
    {
        var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var testCodeDirectory = new DirectoryInfo(
                Path.Join(baseDirectory, path))
            .ToString();
        _compiler = new Compiler(testCodeDirectory, 
        [
            "../../../../MediatrCode/bin/Debug/net8.0/MediatR.Contracts.dll", 
            "../../../../MediatrCode/bin/Debug/net8.0/MediatR.dll"
        ]);
        await _compiler.Initialize();
    }
    
    [Given("a compiler with path {string}")]
    public async Task GivenASinglePath(string path)
    {
        var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var testCodeDirectory = new DirectoryInfo(
            Path.Join(baseDirectory, path))
            .ToString();
        _compiler = new Compiler(testCodeDirectory, []);
        await _compiler.Initialize();
    }
    
    [Given("analyzer is configured")]
    public void GivenAnalyzerIsConfigured()
    {
        _analyzer = new Analyzer(_compiler.Workspace, _compiler, [], []);
    }
    
    [Given("analyzer is configured for Mediatr")]
    public void GivenAnalyzerIsConfiguredForMediatr()
    {
        _analyzer = new Analyzer(_compiler.Workspace, _compiler, 
        [
            "MediatR.INotification",
            "MediatR.IRequest"
        ], 
        [
            "MediatR.INotificationHandler",
            "MediatR.IRequestHandler"
        ]);
    }
    
    [When("code is analyzed")]
    public async Task WhenCodeIsAnalyzed()
    {
        await _analyzer.Analyze();
    }

    [Then("the result should be {int} symbols")]
    public void ThenTheResultShouldBe(int result)
    {
        Assert.Equal(result, _compiler.Symbols.Count);
    }
    
    [Then(@"{string} is a listed symbol")]
    public void IsASymbol(string symbol)
    {
        Assert.Contains(symbol, 
            _compiler.Symbols.Select(s => $"{s.FullNamespace()}.{s.Name}"));
    }
    
    [Then("{int} dependencies are created")]
    public void NumberOfDependencies(int number)
    {
        Assert.Equal(number, Analyzer.Dependencies.Count());
    }
    
    [Then("dependency {string} to {string} is listed")]
    public void NumberOfDependencies(string from, string to)
    {
        Assert.Contains(Analyzer.Dependencies, d => d.From.Name == from && d.To.Name == to);
    }
}
