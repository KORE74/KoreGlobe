using System.Collections.Generic;
using System.Text;

// FssCommandElePatchLoad

#nullable enable

public class FssCommandElePatchLoad : FssCommand
{
    public FssCommandElePatchLoad()
    {
        Signature.Add("ele");
        Signature.Add("patch");
        Signature.Add("load");
    }

    public override string HelpString => $"{SignatureString} <Patch Filename>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count != 1)
            return "FssCommandElePatchLoad -> parameter count mismatch";


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

            FssAppFactory.Instance.EleManager.LoadPatchFile(inPatchFilepath);
        }

        // -------------------------------------------------

        //sb.AppendLine($"Elevation System Report:");
        //sb.AppendLine(FssAppFactory.Instance.EleSystem.Report());

        return sb.ToString();

    }
}
