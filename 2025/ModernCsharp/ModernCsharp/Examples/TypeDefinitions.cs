using ModernCsharp.ValueObjects;
using System.Net;

namespace ModernCsharp.Examples;

internal class TypeDefinitions
{
    Func<string, string> WriteResultFunc;

    public string Calculate(string data)
    {
        return data;
    }

    public SubnetMask CalculateSubnetMask(IPAddress ipAddress)
    {
        // Do the calculation

        return new SubnetMask(24);
    }


}