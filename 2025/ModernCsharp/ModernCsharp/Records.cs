using System.Text.Json.Serialization;

namespace ModernCsharp;

public record PositionalRecord(int Property, string Value);

public record AnnotatedRecord(
    [property: JsonPropertyName("property")] int Property, 
    [property: JsonPropertyName("value")] string Value);

public record OtherwiseRecordsAreJustFancyClasses
{
    public int Property { get; set; }

    public void SomeFunction()
    {

    }
}

public record DeconstructRecord(int Property, string Value)
{

    public void Deconstruct(out string value)
    {
        value = Value;
    }
    public void Deconstruct(out int number)
    {
        throw new NotImplementedException();
    }
}

public static class Records
{
    public static void DestructureRecord()
    {
        var myRecord = new PositionalRecord(1, "Hello");

        // Extracts each property
        var (num, val) = myRecord;

        var myCustomRecord = new DeconstructRecord(1, "Hello");
    }
}

public static class RecordEquality
{
    public static void EqualDemo()
    {

        var rec1 = new PositionalRecord(1, "thing");
        var rec2 = new PositionalRecord(1, "thing");
        var rec3 = new PositionalRecord(2, "things");

        if (rec1 == rec2)
        {
            // This will be true because they have the same values, records do not use reference equality.
        }

        if (rec1 == rec3)
        {
            // This will NOT be true as the values in the record are different.
        }

        // Note there are some exceptions to this rule!
    }

}


public static class RecordMutation
{
    public static void WithDemo()
    {
        // Using the with keyword we can copy all the values into a new record
        // Since records can't be change this is our only option without using low level APIs such as reflection.
        var rec1 = new PositionalRecord(1, "thing");

        var rec2 = rec1 with
        {
            Property = 2
        };

        var rec3 = rec2 with
        {
            Value = "things"
        };

        // What do you think the value of each record is
    } 
}