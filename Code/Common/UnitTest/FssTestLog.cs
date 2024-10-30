using System;
using System.Collections.Generic;
using System.Text;

// File to contain a test entry and the overall test log, such that we can run tests and record the
// outputs with enough information to isolate a single test failure.
// Includes a simple output report we can print every time and a detailed report when we need to debug.

public class FssTestEntry
{
    public string Name { get; }
    public bool Result { get; }
    public string Comment { get; }

    public FssTestEntry(string name, bool result, string comment)
    {
        Name    = name;
        Result  = result;
        Comment = comment;
    }
}

// ------------------------------------------------------------------------

public class FssTestLog
{
    private List<FssTestEntry> ResultList = new List<FssTestEntry>();

    // --------------------------------------------------------------------------------------------
    // Add test results
    // --------------------------------------------------------------------------------------------

    public void Add(string name, bool result, string comment)
    {
        ResultList.Add(new FssTestEntry(name, result, comment));
    }

    public void Add(string name, bool result)
    {
        ResultList.Add(new FssTestEntry(name, result, ""));
    }

    // --------------------------------------------------------------------------------------------
    // Report generation
    // --------------------------------------------------------------------------------------------

    public bool OverallPass()
    {
        bool result = true;

        foreach (FssTestEntry entry in ResultList)
            result &= entry.Result;

        return result;
    }

    public string OneLineReport()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        if (OverallPass())
        {
            return $"Overall:PASS // NumTests:{ResultList.Count} // Time:{timestamp}";
        }
        else
        {
            int passCount = 0;
            int failCount = 0;

            foreach (FssTestEntry entry in ResultList)
            {
                if (entry.Result)
                    passCount++;
                else
                    failCount++;
            }
            return $"Overall:FAIL // NumTests:{ResultList.Count} Passed:{passCount} Failed:{failCount} // Time:{timestamp}";
        }
    }

    public string FullReport()
    {
        StringBuilder sb = new StringBuilder();

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string overallpassfail  = OverallPass() ? "PASS" : "FAIL";

        // Add report header
        sb.AppendLine($"Overall:{overallpassfail} TestCases:{ResultList.Count}");
        sb.AppendLine($"Time:{timestamp}");

        foreach (FssTestEntry entry in ResultList)
        {
            string passfail = entry.Result ? "PASS" : "FAIL";
            string comment  = String.IsNullOrEmpty(entry.Comment) ? "" : $" - {entry.Comment}";

            sb.AppendLine($"{(entry.Result ? "PASS" : "FAIL")} - {entry.Name}{comment}");
        }
        return sb.ToString();
    }
}
