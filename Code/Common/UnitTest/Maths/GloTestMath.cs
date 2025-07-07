using System;

public static class GloTestMath
{
    public static void RunTests(GloTestLog testLog)
    {
        TestValueUtilsBool(testLog);
        TestValueUtilsInt(testLog);
        TestValueUtilsFloat(testLog);
        TestFloat1DArray_Basics(testLog);
    }

    public static void TestValueUtilsBool(GloTestLog testLog)
    {
        testLog.Add("BoolToStr(true)",      (GloValueUtils.BoolToStr(true)    == "True"));
        testLog.Add("BoolToStr(false)",     (GloValueUtils.BoolToStr(false)   == "False"));

        testLog.Add("StrToBool(\"True\")",  (GloValueUtils.StrToBool("True")  == true));
        testLog.Add("StrToBool(\"False\")", (GloValueUtils.StrToBool("False") == false));
        testLog.Add("StrToBool(\"true\")",  (GloValueUtils.StrToBool("true")  == true));
        testLog.Add("StrToBool(\"false\")", (GloValueUtils.StrToBool("false") == false));
        testLog.Add("StrToBool(\"y\")",     (GloValueUtils.StrToBool("y")     == true));
        testLog.Add("StrToBool(\"n\")",     (GloValueUtils.StrToBool("n")     == false));

        testLog.Add("StrToBool(\" \")",     (GloValueUtils.StrToBool(" ")     == false));
        testLog.Add("StrToBool(\"1212\")",  (GloValueUtils.StrToBool("1212")  == false));
    }

    public static void TestValueUtilsInt(GloTestLog testLog)
    {
        testLog.Add("Clamp( 0, 1, 10)", (GloValueUtils.Clamp( 0, 1, 10) == 1));
        testLog.Add("Clamp( 5, 1, 10)", (GloValueUtils.Clamp( 5, 1, 10) == 5));
        testLog.Add("Clamp(11, 1, 10)", (GloValueUtils.Clamp(11, 1, 10) == 10));

        testLog.Add("Wrap( 0, 1, 10)", (GloValueUtils.Wrap( 0, 1, 10) == 10));
        testLog.Add("Wrap( 5, 1, 10)", (GloValueUtils.Wrap( 5, 1, 10) == 5));
        testLog.Add("Wrap(11, 1, 10)", (GloValueUtils.Wrap(11, 1, 10) == 1));
    }

    public static void TestValueUtilsFloat(GloTestLog testLog)
    {
        // Test for Modulo operation
        testLog.Add("Modulo 1.1 % 1.0",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.Modulo(1.1f, 1.0f), 0.1f));
        testLog.Add("Modulo 2.1 % 1.0",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.Modulo(2.1f, 1.0f), 0.1f));
        testLog.Add("Modulo -0.1 % 1.0", GloValueUtils.EqualsWithinTolerance(GloValueUtils.Modulo(-0.1f, 1.0f), 0.9f));

        // Test for LimitToRange operation
        testLog.Add("LimitToRange 1.1 in 0-1", GloValueUtils.EqualsWithinTolerance(GloValueUtils.LimitToRange(1.1f, 0f, 1f), 1f));
        testLog.Add("LimitToRange -5 in 0-1",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.LimitToRange(-5f, 0f, 1f), 0f));
        testLog.Add("LimitToRange 0.5 in 0-1", GloValueUtils.EqualsWithinTolerance(GloValueUtils.LimitToRange(0.5f, 0f, 1f), 0.5f));

        // Test for WrapToRange operation
        testLog.Add("WrapToRange 1.1 in 1-2",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.WrapToRange(1.1f, 1f, 2f), 1.1f));
        testLog.Add("WrapToRange 3.1 in 1-2",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.WrapToRange(3.1f, 1f, 2f), 1.1f));
        testLog.Add("WrapToRange -1.5 in 1-2", GloValueUtils.EqualsWithinTolerance(GloValueUtils.WrapToRange(-1.5f, 1f, 2f), 1.5f));

        // Test for DiffInWrapRange operation
        testLog.Add("DiffInWrapRange 1 to 350 in 0-360",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.DiffInWrapRange(0f, 360f, 1f, 350f), -11f));
        testLog.Add("DiffInWrapRange 1 to 5 in 0-360",    GloValueUtils.EqualsWithinTolerance(GloValueUtils.DiffInWrapRange(0f, 360f, 1f, 5f), 4f));
        testLog.Add("DiffInWrapRange 340 to 20 in 0-360", GloValueUtils.EqualsWithinTolerance(GloValueUtils.DiffInWrapRange(0f, 360f, 340f, 20f), 40f));

        // Test for IndexFromFraction operation
        testLog.Add($"IndexFromFraction 0.1 in 0-10",   GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.1f, 0, 10), 1));
        testLog.Add($"IndexFromFraction 0.2 in 0-100",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.2f, 0, 100), 20));
        testLog.Add($"IndexFromFraction 0.49 in 0-5",   GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.49f, 0, 5), 2));
        testLog.Add($"IndexFromFraction 0.50 in 0-5",   GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.50f, 0, 5), 2));
        testLog.Add($"IndexFromFraction 0.6  in 0-5",   GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromFraction(0.6f, 0, 5), 3));

        // Test for IndexFromIncrement operation
        testLog.Add("IndexFromIncrement 0.1 from 0 increment 1",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromIncrement(0f, 1f, 0.1f), 0));
        testLog.Add("IndexFromIncrement 1.1 from 0 increment 1",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromIncrement(0f, 1f, 1.1f), 1));
        testLog.Add("IndexFromIncrement 13.1 from 0 increment 1", GloValueUtils.EqualsWithinTolerance(GloValueUtils.IndexFromIncrement(0f, 1f, 13.1f), 13));

        // Test for IsInRange operation
        testLog.Add("IsInRange 0 in 0-1",   GloValueUtils.IsInRange(0f, 0f, 1f));
        testLog.Add("IsInRange 1 in 0-1",   GloValueUtils.IsInRange(1f, 0f, 1f));
        testLog.Add("!IsInRange -1 in 0-1", !GloValueUtils.IsInRange(-1f, 0f, 1f));
        testLog.Add("!IsInRange 2 in 0-1",  !GloValueUtils.IsInRange(2f, 0f, 1f));

        // Test for Interpolate operation
        testLog.Add("Interpolate 0.1 between 0 and 1",    GloValueUtils.EqualsWithinTolerance(GloValueUtils.Interpolate(0f, 1f, 0.1f), 0.1f));
        testLog.Add("Interpolate 0.9 between 0 and 100",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.Interpolate(0f, 100f, 0.9f), 90f));
        testLog.Add("Interpolate 1.1 between 0 and 100",  GloValueUtils.EqualsWithinTolerance(GloValueUtils.Interpolate(0f, 100f, 1.1f), 110f));
        testLog.Add("Interpolate -0.1 between 0 and 100", GloValueUtils.EqualsWithinTolerance(GloValueUtils.Interpolate(0f, 100f, -0.1f), -10f));
    }

    public static void TestFloat1DArray_Basics(GloTestLog testLog)
    {
        GloFloat1DArray array = new GloFloat1DArray(5);
        array[0] = 1.0f;
        array[1] = 2.0f;
        array[2] = 3.0f;
        array[3] = 4.0f;
        array[4] = 5.0f;

        // Test Length
        testLog.Add("Array Length", array.Length == 5);

        // Test individual element access
        testLog.Add("Array[0]", GloValueUtils.EqualsWithinTolerance(array[0], 1.0f));
        testLog.Add("Array[1]", GloValueUtils.EqualsWithinTolerance(array[1], 2.0f));
        testLog.Add("Array[2]", GloValueUtils.EqualsWithinTolerance(array[2], 3.0f));
        testLog.Add("Array[3]", GloValueUtils.EqualsWithinTolerance(array[3], 4.0f));
        testLog.Add("Array[4]", GloValueUtils.EqualsWithinTolerance(array[4], 5.0f));

        // Test Max
        testLog.Add("Array Max", GloValueUtils.EqualsWithinTolerance(array.Max(), 5.0f));

        // Test Min
        testLog.Add("Array Min", GloValueUtils.EqualsWithinTolerance(array.Min(), 1.0f));

        // Test Average
        testLog.Add("Array Average", GloValueUtils.EqualsWithinTolerance(array.Average(), 3.0f));

        // Test Sum
        testLog.Add("Array Sum", GloValueUtils.EqualsWithinTolerance(array.Sum(), 15.0f));

        // Additional tests for boundary conditions and invalid inputs
        // Assuming GloFloat1DArray handles negative indices or out-of-bound indices gracefully
        // testLog.Add("Array[-1]", GloValueUtils.EqualsWithinTolerance(array[-1], /* expected value */));
        // testLog.Add("Array[5]", GloValueUtils.EqualsWithinTolerance(array[5], /* expected value */));
    }

}