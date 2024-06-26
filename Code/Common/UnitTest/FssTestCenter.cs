using System;

public static class FssTestCenter
{
    public static FssTestLog RunCoreTests()
    {
        FssTestLog testLog = new FssTestLog();

        try
        {
            FssTestMath.RunTests(testLog);
            FssTestPosition.RunTests(testLog);
            //FssTestPlotter.RunTests(testLog);
        }
        catch (Exception)
        {
            testLog.Add("Test Centre Run", false, "Exception");
        }

        return testLog;
    }
}

