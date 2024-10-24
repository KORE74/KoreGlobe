using System.Collections.Generic;

// FssCommandVersion

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

        string entName  = parameters[0];

        if (FssAppFactory.Instance.EventDriver.DoesEntityExist(entName))
        {
            FssLLAPoint? lla = FssAppFactory.Instance.EventDriver.GetPlatformPosition(entName);
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
