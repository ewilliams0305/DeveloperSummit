namespace ModernCsharp;

public class CollectionExpressions
{
    public void InferredCollectionTypes()
    {
        AcceptsList(new List<string>()
        {
            "Boo"
        });

        // We can invoke the method with an expression of elements
        AcceptsList(
            [
                "First", "Second"
            ]);

        // The compiler is event smart enough to pick a type for us
        AcceptsEnumerable(
            [
                "First", "Second"
            ]);
        
        // The compiler will select a different type based on the number of items
        AcceptsEnumerable(
            [
                "First"
            ]);
        
        // TAnd type, note no use of generic params
        AcceptsEnumerable(

            [
                1, 2, 3, 5
            ]);
    }

    public void SpreadDemo()
    {
        string[] vowels = ["a", "e", "i", "o", "u"];
        string[] consonants = ["b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z"];
        string[] alphabet = [.. vowels, .. consonants, "y"];
    }


    public void AcceptsList(List<string> items)
    {
    }

    public Type AcceptsEnumerable<T>(IEnumerable<T> items)
    {
        return items.GetType();
    }
}