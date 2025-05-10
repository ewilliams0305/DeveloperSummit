namespace ModernCsharp;

public sealed class AccessModifiers
{
    public required string Required { get; init; }

    public string InitOnly { get; init; }

    public AccessModifiers(string initOnly)
    {
        InitOnly = initOnly;
    }

    public static void Example()
    {
        // Note we MUST provide an init only member a value before exiting the constructor.
        var modifier = new AccessModifiers("Must have value")
        {
            Required = "Must set the required property before exiting scope"
        };
    }
}