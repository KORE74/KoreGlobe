using System.Collections.Generic;


public class FssCommandEntityAdd : FssCommand
{
    public FssCommandEntityAdd()
    {
        Signature.Add("ent");
        Signature.Add("add");
    }

    public override string HelpString => $"{SignatureString} <entity_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "FssCommandEntityAdd.Execute -> insufficient parameters";
        }

        string entName = parameters[0];
        string retString = "";

        if (!FssEventDriver.DoesEntityExist(entName))
        {
            FssEventDriver.AddEntity(entName);
            retString += $"Entity {entName} added.";
        }
        else
        {
            retString = $"Entity {entName} already exists. Not Changed.";
        }

        FssCentralLog.AddEntry($"FssCommandEntityAdd.Execute -> {retString}");
        return retString;
    }
}
