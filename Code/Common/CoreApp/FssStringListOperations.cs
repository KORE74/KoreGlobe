using System;
using System.Collections.Generic;
using System.Linq;

public static class FssStringListOperations
{
    // Filters the list to include only those strings that contain the specified substring
    public static List<string> FilterIn(List<string> list, string substring) => list.Where(item => item.Contains(substring)).ToList();

    // Filters the list to exclude those strings that contain the specified substring
    public static List<string> FilterOut(List<string> list, string substring) => list.Where(item => !item.Contains(substring)).ToList();
}
