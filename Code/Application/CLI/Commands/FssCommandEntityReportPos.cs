using System.Collections.Generic;

public class FssCommandEntityReportPos : FssCommand
{
    public FssCommandEntityReportPos()
    {
        Signature.Add("plat");
        Signature.Add("report");
        Signature.Add("pos");
    }

    public override string HelpString => $"{SignatureString} <entity_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "FssCommandEntityAdd.Execute -> insufficient parameters";
        }

        string entName = parameters[0];

        if (FssEventDriver.DoesEntityExist(entName))
        {
            FssLLAPoint? lla = FssEventDriver.EntityCurrLLA(entName);
            if (lla != null)
            {
                return $"Platform {entName} Position: {lla}";
            }
            else
            {
                return $"FssCommandEntityAdd: Platform {entName} Position: No Position Set";
            }
        }
        return $"FssCommandEntityAdd: Platform {entName} not found.";
    }
}
