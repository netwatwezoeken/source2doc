using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using App.Renderers;
using Reqnroll.UnitTestProvider;
using VerifyXunit;

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
    
    [Given("source code {string} and mediatr libs")]
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
    
    [Given("source code {string}")]
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
        Assert.Equal(number, _analyzer.Data.Dependencies.Count());
    }
    
    [Then("{int} types are created")]
    public void NumberOfTypes(int number)
    {
        Assert.Equal(number, _analyzer.Data.Types.Count());
    }
    
    [Then("dependency {string} to {string} is listed")]
    public void NumberOfDependencies(string from, string to)
    {
        Assert.Contains(_analyzer.Data.Dependencies, d => d.From.Id.Name == from && d.To.Id.Name == to);
    }
    
    [Then("class {string} is of type {string}")]
    public void ClasIfType(string className, string type)
    {
        Assert.Equal(type, _analyzer.Data.Types.FirstOrDefault(d => d.Id.Name == className)?.Type.ToString());
    }
    
    [Then("class verify mermaid")]
    public async Task verifyMermaid()
    {
        var groups = Grouping.GroupDependencies(_analyzer.Data);

        using var renderer = new MermaidMarkdown();
        var stream = await renderer.Render(groups);
        var output = await new StreamReader(stream).ReadToEndAsync();
       // await Verify(output);
        await Verifier.Verify(output);
        //Assert.Equal("", output);
    }
    
    [Then("{string} is an event")]
    public void IsAnEvent(string className)
    {
        IsA(className, Type.Event);
    }
    
    [Then("{string} is a publisher")]
    public void IsAPublisher(string className)
    {
        IsA(className, Type.Publisher);
    }
    
    [Then("{string} is a handler")]
    public void IsAHandler(string className)
    {
        IsA(className, Type.Handler);
    }
    
    private void IsA(string className, Type type)
    {
        var dep = _analyzer.Data.Types.FirstOrDefault(
            d => d.Id.Name == className);
        Assert.Equal(type, dep?.Type);
    }
}
