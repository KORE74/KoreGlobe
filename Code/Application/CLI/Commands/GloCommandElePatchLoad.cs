using System.Collections.Generic;
using System.Text;

// GloCommandElePrep


// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/GlobeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes

#nullable enable

public class GloCommandElePatchLoad : GloCommand
{
    public GloCommandElePatchLoad()
    {
        Signature.Add("ele");
        Signature.Add("patch");
        Signature.Add("load");
    }

    public override string HelpString => $"{SignatureString} <Patch Filename>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count != 1)
            return "GloCommandElePatchLoad -> parameter count mismatch";


        StringBuilder sb = new StringBuilder();

        string inPatchFilepath = parameters[0];

        sb.AppendLine($"Elevation Prep:");
        sb.AppendLine($"- inEleFilename: {inPatchFilepath}");

        bool validOperation = true;

        // -------------------------------------------------

        // Convert and validate the inputs
        if (!System.IO.File.Exists(inPatchFilepath))
        {
            sb.AppendLine($"File not found: {inPatchFilepath}");
            validOperation = false;
        }

        // -------------------------------------------------

        if (validOperation)
        {
            sb.AppendLine($"Valid operation: Progressing...");

            GloAppFactory.Instance.EleManager.LoadPatchFile(inPatchFilepath);
        }

        // -------------------------------------------------

        //sb.AppendLine($"Elevation System Report:");
        //sb.AppendLine(GloAppFactory.Instance.EleSystem.Report());

        return sb.ToString();

    }
}