using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Gherkin;
using Gherkin.Ast;
using VerifyXunit;
using Xunit.Abstractions;

namespace Tests;

public class FullyQualifiedNameParsing
{
    private readonly ITestOutputHelper _output;
    private IEnumerable<Comment> _comments;
    private int _lastLine;
    private readonly StringBuilder _documentBuilder = new ();

    public FullyQualifiedNameParsing(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void A_string_without_dots_is_a_class()
    {
        var fqn = "SomeClass";
        Assert.Equal(new ClassIdentifier("", "SomeClass"), fqn.ToClassIdentifier());
    }
    
    [Fact]
    public void A_string_with_one_dot_is_a_class_in_a_single_namespace()
    {
        var fqn = "Namespace.SomeClass";
        Assert.Equal(new ClassIdentifier("Namespace", "SomeClass"), fqn.ToClassIdentifier());
    }
    
    [Fact]
    public void A_string_with_multiple_dots_is_a_class_in_a_multi_segment_namespace()
    {
        var fqn = "Some.Deep.Namespace.SomeClass";
        Assert.Equal(new ClassIdentifier("Some.Deep.Namespace", "SomeClass"), fqn.ToClassIdentifier());
    }
}