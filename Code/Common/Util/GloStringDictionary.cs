using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GloStringDictionary
{
    private readonly Dictionary<string, object> _data = new(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<string>                       Keys    => _data.Keys;
    public IEnumerable<KeyValuePair<string, object>> Entries => _data;

    // --------------------------------------------------------------------------------------------
    // MARK: Set/Get
    // --------------------------------------------------------------------------------------------

    public void Set(string key, object value)
    {
        if (value is GloStringDictionary dict)
        {
            _data[key] = dict;
        }
        else
        {
            _data[key] = value?.ToString() ?? string.Empty;
        }
    }

    public string Get(string key, string fallback = null)
    {
        return _data.TryGetValue(key, out var val)
            ? val?.ToString() ?? fallback
            : fallback;
    }

    public bool Has(string key)    => _data.ContainsKey(key);
    public bool Remove(string key) => _data.Remove(key);

    // --------------------------------------------------------------------------------------------

    // Slightly more complex set / get operations

    public string GetOrSetDefault(string key, string defaultValue)
    {
        if (!_data.ContainsKey(key))
            _data[key] = defaultValue;

        return Get(key);
    }

    public IEnumerable<string> FindFirst(string keySubstring)
    {
        foreach (var key in _data.Keys)
        {
            if (key.Contains(keySubstring, StringComparison.OrdinalIgnoreCase))
                yield return key;
        }
    }

    public string this[string key]
    {
        get => Get(key) ?? throw new KeyNotFoundException($"Key '{key}' not found.");
        set => Set(key, value);
    }

    // Read a value and remove it from the dictionary, useful for dirty flags or other signals
    public bool Consume(string key, out string value)
    {
        if (_data.TryGetValue(key, out var val))
        {
            _data.Remove(key);
            value = val?.ToString();
            return true;
        }

        value = null;
        return false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    // Loop through all the objects in the dictionary and return a string report of their contents
    public string Report()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var kv in _data)
        {
            sb.AppendLine($"{kv.Key}: {kv.Value}");
        }
        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: JSON Export/Import
    // --------------------------------------------------------------------------------------------

    public string ExportJson(bool indented = true)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = indented,
            Converters = { new GloStringDictionaryConverter() }
        };
        return JsonSerializer.Serialize(this, options);
    }

    public void ImportJson(string json)
    {
        _data.Clear();
        var options = new JsonSerializerOptions
        {
            Converters = { new GloStringDictionaryConverter() }
        };

        var result = JsonSerializer.Deserialize<GloStringDictionary>(json, options);
        if (result != null)
        {
            foreach (var kv in result._data)
                _data[kv.Key] = kv.Value;
        }
    }

    // --------------------------------------------------------------------------------------------
    // Custom converter for recursive support
    // --------------------------------------------------------------------------------------------

    private class GloStringDictionaryConverter : JsonConverter<GloStringDictionary>
    {
        public override GloStringDictionary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = new GloStringDictionary();
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject");

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dict;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName");

                string key = reader.GetString();
                reader.Read();

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var nested = JsonSerializer.Deserialize<GloStringDictionary>(ref reader, options);
                    dict.Set(key, nested);
                }
                else
                {
                    string value = reader.GetString();
                    dict.Set(key, value);
                }
            }

            return dict;
        }

        public override void Write(Utf8JsonWriter writer, GloStringDictionary value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kv in value._data)
            {
                writer.WritePropertyName(kv.Key);
                if (kv.Value is GloStringDictionary nested)
                {
                    JsonSerializer.Serialize(writer, nested, options);
                }
                else
                {
                    writer.WriteStringValue(kv.Value?.ToString());
                }
            }
            writer.WriteEndObject();
        }
    }
}
