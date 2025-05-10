using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModernCsharp.Examples;

internal class PureFunctions
{

    public int Multiply(int x, int y) => x * y;

    public Func<int, int, int> MultiplyPointer { get; set; }
}


internal class NotPureFunctions
{
    public string Prefix { get; set; }

    public string FormatData(int value) => $"{Prefix}_{value}";
}

public record InputForRequest(int Value);
public record CalculationResult(int Calculation);

public class BadExample
{
    public async ValueTask<CalculationResult> CalculatorInput(InputForRequest input)
    {
        if (input.Value < 10 && input.Value > 0)
        {
            return new CalculationResult(-1);
        }

        using var client = new HttpClient();
        using var response = await client.PostAsync("http://calculor", JsonContent.Create(input));

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return new CalculationResult(1);
        }

        var data = await response.Content.ReadAsStringAsync();

        if (!int.TryParse(data, out var number) && number > 0)
        {
            return new CalculationResult(-1);
        }

        var result = input.Value * number * 12;

        return new CalculationResult(result);
    }
}

public class CompositionExample
{
    public async ValueTask<CalculationResult> CalculatorInput(InputForRequest input)
    {
        if (!InputIsWithinValidRange(input, 0, 10))
        {
            return new CalculationResult(-1);
        }

        using var client = new HttpClient();
        var data = await QueryCalculationApi("http://calculor", client, input);

        return IsResponseDataValidNumber(data, 0, out var number) 
            ? PerformCalculation(input, number, 12)
            : new CalculationResult(-1);
    }

    public bool InputIsWithinValidRange(InputForRequest input, int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentOutOfRangeException(nameof(min), "The minimum must be less than the max");
        }
        return input.Value < max && input.Value > min;
    }

    public async Task<string?> QueryCalculationApi(string url, HttpClient client, InputForRequest input)
    {
        using var response = await client.PostAsync(url, JsonContent.Create(input));

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        return await response.Content.ReadAsStringAsync();
    }

    public bool IsResponseDataValidNumber(string? data, int min, out int number)
    {
        return int.TryParse(data, out number) && number > min;
    }

    public CalculationResult PerformCalculation(InputForRequest input, int number, int multiplier)
    {
        return new CalculationResult(input.Value * number * multiplier);
    }
}
