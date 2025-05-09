using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubscriptionDashboard.Models;

public class Invoice
{
    public int Epoch { get; init;  }
    
    [JsonConverter(typeof(StringToDoubleConverter))]
    public double AmountVSol { get; init; }

    public string DisplayAmountVSol => string.Format("${0:0.####}", AmountVSol / 1e7f);

    [JsonConverter(typeof(StringToDoubleConverter))]
    public double BalanceOutstanding { get; init; }
    public string VoteAccount { get; init; }
    
}

public class StringToDoubleConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && double.TryParse(reader.GetString(), out var result))
        {
            return result;
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDouble();
        }

        throw new JsonException("Invalid value for double.");
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
