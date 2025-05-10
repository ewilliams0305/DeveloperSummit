using ModernCsharp.ValueObjects;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;

namespace ModernCsharp.Examples;

public class BadPractices
{
    public required string IpAddress { get; init; }
    public required string MacAddress { get; init; }
    public required double Volume { get; init; }
}

public class BestPractices
{
    public required IPAddress IpAddress { get; init; }
    public required PhysicalAddress MacAddress{ get; init; }
    public required VolumeLevel Volume { get; init; }
}

public static class VolumeExtensions
{
    public static double CalculateAverageVolume(params double[] volumeLevels)
    {
        return volumeLevels.Sum() / volumeLevels.Length;
    }

    public static VolumeLevel CalculateAverageVolume(params VolumeLevel[] volumeLevels)
    {
        return volumeLevels.Sum(v => v.Value) / volumeLevels.Length;
    }
}