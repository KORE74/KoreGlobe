using System;
using System.Collections.Generic;
using System.Linq;

public static class FssStringListOperations
{
    // Filters the list to include only those strings that contain the specified substring
    public static List<string> FilterIn(List<string> list, string substring) => list.Where(item => item.Contains(substring)).ToList();

    // Filters the list to exclude those strings that contain the specified substring
    public static List<string> FilterOut(List<string> list, string substring) => list.Where(item => !item.Contains(substring)).ToList();

    // Take two lists of strings, and return a list that are not in the first list
    // Usage: List<string> omittedInSecond = FssStringListOperations.ListOmittedInSecond(list1, list2);
    public static List<string> ListOmittedInSecond(List<string> list1, List<string> list2) => list1.Except(list2).ToList();

    public static List<string> ListInBoth(List<string> list1, List<string> list2) => list1.Intersect(list2).ToList();
}
