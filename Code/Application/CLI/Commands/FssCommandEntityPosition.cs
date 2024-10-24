using System.Collections.Generic;

// FssCommandPlatDelete

public class FssCommandEntityPosition : FssCommand
{
    public FssCommandEntityPosition()
    {
        Signature.Add("ent");
        Signature.Add("pos");
    }

    public override string HelpString => $"{SignatureString} <entity_name> <latdegs> <longdegs> <altmslm>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 4)
        {
            return "FssCommandEntityPosition.Execute -> insufficient parameters";
        }

        string entName  = parameters[0];
        double latDegs  = double.Parse(parameters[1]);
        double longDegs = double.Parse(parameters[2]);
        double altMslM  = double.Parse(parameters[3]);

        string retString = "";

        if (FssAppFactory.Instance.EventDriver.DoesEntityExist(entName))
        {
            FssLLAPoint newLLA = new FssLLAPoint { LatDegs = latDegs, LonDegs = longDegs, AltMslM = altMslM };

            FssAppFactory.Instance.EventDriver.SetEntityCurrLLA(entName, newLLA);
            retString = $"Entity {entName} Updated: Course: {newLLA}.";
        }
        else
        {
            retString = $"Entity {entName} not found.";
        }

        FssCentralLog.AddEntry($"FssCommandEntityPosition.Execute -> {retString}");
        return retString;
    }
}
