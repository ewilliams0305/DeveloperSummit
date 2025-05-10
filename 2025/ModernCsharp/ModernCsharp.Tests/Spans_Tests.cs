using FluentAssertions;

namespace ModernCsharp.Tests;

// ReSharper disable once InconsistentNaming
public class Spans_Tests
{
    [Fact]
    public void StringDemo_Should_Return_9_Words()
    {
        var spanDemo = new Spans();
        var words = spanDemo.StringDemo();

        words.Count.Should().Be(9);



        var value = 12;

        value += 12;
        value ++;
        value += 24;

        value = value / 2;

    }
    
    [Theory]
    [InlineData(0, "The")]
    [InlineData(1, "quick")]
    [InlineData(2, "brown")]
    [InlineData(8, "river")]
    public void StringDemo_Should_Return_Include_Correct_Word(int index, string value)
    {
        var spanDemo = new Spans();
        var words = spanDemo.StringDemo();

        words[index].Should().Be(value);
    }
}
