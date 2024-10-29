
using System;
using System.Collections.Generic;

// FssAttributeDictionary: A simple dictionary class that stores key-value pairs of strings.
// Example usage: Allows an entity to have a list of extra attributes that can be set, retrieved, and removed
//                depending on the entity, without forcing subclassing of elements too early.

public class FssAttributeDictionary
{
    private readonly Dictionary<string, string> Attributes  = new Dictionary<string, string>();

    public void Set(string key, string value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        Attributes[key] = value;
    }

    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        return Attributes.TryGetValue(key, out var value) ? value : null;
    }

    public bool Has(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        return Attributes.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        return Attributes.Remove(key);
    }

    public void Clear()
    {
        Attributes.Clear();
    }

    public override string ToString()
    {
        return string.Join(", ", Attributes);
    }
}

// Example usage
// var attributes = new AttributeDictionary();
// attributes.Set("Color", "Blue");
// string color = attributes.Get("Color");
// bool hasColor = attributes.Has("Color");
// attributes.Remove("Color");
// attributes.Clear();
