namespace ModernCsharp;

public class PatternMatching
{
    
    public void IsPattern()
    {
        Message message = new TextMessage("hello patterns");

        if (message is TextMessage)
        {
            // Because the type is in fact a text message this is true
        }
        
        if (message is TextMessage text)
        {
            // We can even cast to the Text Message type 100% safely
            Console.WriteLine(text.Text);
        }
    }

    public void SwitchPattern()
    {
        Message message = new NumericMessage(12);

        switch (message)
        {
            case TextMessage text:
                Console.WriteLine(text.Text);
                break;

            case NumericMessage num:
                Console.WriteLine(num);
                break;
        }
    }

    public void NumberOperators()
    {
        Message message = new NumericMessage(12);

        if (message is NumericMessage numeric)
        {
            if (numeric.Value is > 12 and < 200)
            {
                // This makes the numeric operation easier to read
            }
        }
        
        if (message is NumericMessage {Value: > 12 and < 200 })
        {
            // Because patterns with can merge this statement
        }
    }

    public void ExpressionPatterns()
    {
        Message message = new NotificationMessage("Oops", 2000, TimeSpan.FromHours(1));

        var number = message switch
        {
            null => 0,
            TextMessage => 0,
            NumericMessage num => num.Value,

            NotificationMessage notice when notice.Duration < TimeSpan.FromHours(2) => notice.AlertLevel,

            NotificationMessage { AlertLevel: > 12 and < 1000 } notice => notice.Duration.TotalMilliseconds,

            NotificationMessage { AlertLevel: < 12, Notice: "Oops" } notice => notice.Duration.TotalMilliseconds,

            NotificationMessage notice => notice.AlertLevel,

            _ => 0
        };
    }
}

public abstract record Message;

public record NumericMessage(int Value): Message;

public record TextMessage(string Text) : Message;

public record NotificationMessage(string Notice, int AlertLevel, TimeSpan  Duration) : Message, INotificationMessage;

public interface INotificationMessage
{
    string Notice { get; init; }
    int AlertLevel { get; init; }
    TimeSpan Duration { get; init; }
}