using FluentAssertions;

namespace ModernCsharp.Tests;

// ReSharper disable once InconsistentNaming
public class CollectionExpressions_Tests
{
    [Fact]
    public void SelectedType()
    {
        var exp = new CollectionExpressions();
        var type = exp.AcceptsEnumerable(
            [
                "First",
                "Second"
            ]);

        type.Should().Be<string[]>();
    }
}