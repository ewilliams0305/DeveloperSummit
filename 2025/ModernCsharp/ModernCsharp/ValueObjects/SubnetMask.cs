namespace ModernCsharp.ValueObjects;

public record struct SubnetMask
{
    public int Value { get; set; }


    public SubnetMask(int value)
    {
        Value = value switch
        {
            > 32 => throw new ArgumentOutOfRangeException(nameof(value), "The subnet mask must be less than 32"),
            < 8 => throw new ArgumentOutOfRangeException(nameof(value), "The subnet mask must be greater than 8"),
            _ => value
        };
    }
}