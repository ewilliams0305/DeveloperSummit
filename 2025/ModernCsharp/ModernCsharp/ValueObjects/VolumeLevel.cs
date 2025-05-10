namespace ModernCsharp.ValueObjects;

public record struct VolumeLevel
{
    public double Value { get; }

    public VolumeLevel(double value)
    {
        Value = value switch
        {
            > 150 => throw new ArgumentOutOfRangeException(nameof(value), "Volume level can't be greater than 150db"),
            < 0 => throw new ArgumentOutOfRangeException(nameof(value), "Volume level can't be less than zero"),
            _ => value
        };
    }

    public static implicit operator VolumeLevel(double value) => new(value);

    public static implicit operator double(VolumeLevel volume) => volume.Value;
}