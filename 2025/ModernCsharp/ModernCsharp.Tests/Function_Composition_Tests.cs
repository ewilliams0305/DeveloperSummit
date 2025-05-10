using FluentAssertions;
using ModernCsharp.Examples;

namespace ModernCsharp.Tests;

// ReSharper disable once InconsistentNaming
public class Function_Composition_Tests
{
    [Theory]
    [InlineData(-10, 0)]
    [InlineData(0, 1)]
    [InlineData(12, 50)]
    public void InputIsWithinValidRange_Should_ReturnFalse_When_InputIsLessThan_Min(int value, int min)
    {
        var calculator = new CompositionExample();

        calculator.InputIsWithinValidRange(new InputForRequest(value), min, 100).Should().BeFalse();
    }
    
    [Theory]
    [InlineData(13, 12)]
    [InlineData(1000, 32)]
    [InlineData(21000, 1000)]
    public void InputIsWithinValidRange_Should_ReturnFalse_When_InputIsGreaterThan_Max(int value, int max)
    {
        var calculator = new CompositionExample();

        calculator.InputIsWithinValidRange(new InputForRequest(value), 0, max).Should().BeFalse();
    }
    
    [Theory]
    [InlineData(13, 12)]
    [InlineData(1000, 32)]
    [InlineData(21000, 1000)]
    public void InputIsWithinValidRange_Should_ThrowArgumentException_When_MinIsGreaterThan_Max(int min, int max)
    {
        var calculator = new CompositionExample();

        var act = () => calculator.InputIsWithinValidRange(new InputForRequest(12), min, max);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(13, 12, 200)]
    [InlineData(1000, 92, 20111)]
    [InlineData(93, 92, 20111)]
    [InlineData(20110, 92, 20111)]
    public void InputIsWithinValidRange_Should_ReturnTrue_When_InputIsWithinRange(int value, int min, int max)
    {
        var calculator = new CompositionExample();

        calculator.InputIsWithinValidRange(new InputForRequest(value), min, max).Should().BeTrue();
    }
}
