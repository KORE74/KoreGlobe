using System.Collections.Generic;

// FssCommandVersion

public class FssCommandEntityReportElem : FssCommand
{
    public FssCommandEntityReportElem()
    {
        Signature.Add("plat");
        Signature.Add("report");
        Signature.Add("elem");
    }

    public override string Execute(List<string> parameters)
    {
        return $"Entity Elements";
    }
}
