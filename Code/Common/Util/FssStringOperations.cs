using System;
using System.Collections.Generic;
using System.Text;

// A utility calss of miscellaneous string operations

public static class FssStringOperations
{

    // --------------------------------------------------------------------------------------------
    // MARK: Letters to Numbers
    // --------------------------------------------------------------------------------------------

    // Usage: int index = FssStringOperations.NumberForLetter('a');

    public static int NumberForLetter(char letter)
    {
        if (letter >= 'a' && letter <= 'z')
            return letter - 'a';
        if (letter >= 'A' && letter <= 'Z')
            return letter - 'A';
        return -1;
    }

    public static char LetterForNumber(int number)
    {
        if (number >= 0 && number < 26)
            return (char)('a' + number);
        return ' ';
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String to Bool
    // --------------------------------------------------------------------------------------------

    // Any kind of string to a bool

    // Usage: bool isTrue = FssStringOperations.BoolForString("true");

    public static bool BoolForString(string str)
    {
        if (str.Contains("true",  StringComparison.OrdinalIgnoreCase)) return true;
        if (str.Contains("false", StringComparison.OrdinalIgnoreCase)) return false;
        if (str.Contains("yes",   StringComparison.OrdinalIgnoreCase)) return true;
        if (str.Contains("no",    StringComparison.OrdinalIgnoreCase)) return false;

        if (str.Contains("1")) return true;
        if (str.Contains("0")) return false;

        return false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String to Time
    // --------------------------------------------------------------------------------------------

    // Convert a string in the format "hh:mm:ss" to seconds
    // Usage: int seconds = FssStringOperations.TimeHmsToSeconds("01:23:45");

    public static int TimeHmsToSeconds(string timeHms)
    {
        if (string.IsNullOrEmpty(timeHms)) 
            return 0;

        string[] parts = timeHms.Split(':');
        if (parts.Length != 3) return 0;

        int hours = int.Parse(parts[0]);
        int mins  = int.Parse(parts[1]);
        int secs  = int.Parse(parts[2]);

        return (hours * 3600) + (mins * 60) + secs;
    }

}

