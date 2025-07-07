

// Glo1DMappedRange: A class of a range of input values, mapped to a list of output values.
// Allows a caller to define a range of input and outputs that are accessible through linear interpolation.

using System;
using System.Collections.Generic;

public class Glo1DMappedRange
{
    // List to store input-output pairs
    private List<(double input, double output)> entries = new List<(double, double)>();

    // Method to add a new input-output pair
    public void AddEntry(double input, double output)
    {
        entries.Add((input, output));
        // Sort the list by input value
        entries.Sort((a, b) => a.input.CompareTo(b.input));
    }

    // Method to get the output value corresponding to an input value using linear interpolation
    public double GetValue(double input)
    {
        if (entries.Count == 0)
        {
            throw new InvalidOperationException("No entries have been added.");
        }

        // If the input is outside the bounds of the known inputs, return the nearest known output
        if (input <= entries[0].input)
        {
            return entries[0].output;
        }
        if (input >= entries[entries.Count - 1].input)
        {
            return entries[entries.Count - 1].output;
        }

        // Find the two entries that the input falls between
        for (int i = 0; i < entries.Count - 1; i++)
        {
            if (input >= entries[i].input && input <= entries[i + 1].input)
            {
                // Perform linear interpolation
                double t = (input - entries[i].input) / (entries[i + 1].input - entries[i].input);
                return entries[i].output + t * (entries[i + 1].output - entries[i].output);
            }
        }

        // Should never reach this point if the list is sorted correctly
        throw new Exception("Unexpected error in GetValue.");
    }
}
