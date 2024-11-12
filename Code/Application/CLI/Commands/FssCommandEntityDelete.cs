using System.Collections.Generic;

public class FssCommandEntityDelete : FssCommand
{

    public FssCommandEntityDelete()
    {
        Signature.Add("ent");
        Signature.Add("del");
    }

    public override string HelpString => $"{SignatureString} <entity_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "FssCommandEntityDelete.Execute -> insufficient parameters";
        }

        string entName  = parameters[0];
        string retString = "";

        // Delete the platform
        if (FssEventDriver.DoesEntityExist(entName))
        {
            FssEventDriver.DeleteEntity(entName);
            retString = $"Entity {entName} deleted.";
        }
        else
        {
            retString = $"Entity {entName} not found.";
        }

        FssCentralLog.AddEntry($"FssCommandEntityDelete.Execute -> {retString}");
        return retString;
    }
}
