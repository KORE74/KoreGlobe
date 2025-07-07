using System;
using System.Collections.Generic;
using System.Text;

// GloCommandElePrep


// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/GlobeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes

#nullable enable

public class GloCommandElePatchSave : GloCommand
{
    public GloCommandElePatchSave()
    {
        Signature.Add("ele");
        Signature.Add("patch");
        Signature.Add("save");
    }

    public override string HelpString => $"{SignatureString} <Output Patch Filepath>   <longitude num points> <min lon degs> <max lon degs>   <latitude num points> <min lat degs> <max lat degs>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count != 7)
            return "GloCommandEleLoadArc.Execute -> parameter count mismatch";

        string inPatchFilepath = parameters[0];
        int    inNumLonPoints  =    int.Parse(parameters[1]);
        double inMinLonDegs    = double.Parse(parameters[2]);
        double inMaxLonDegs    = double.Parse(parameters[3]);
        int    inNumLatPoints  =    int.Parse(parameters[4]);
        double inMinLatDegs    = double.Parse(parameters[5]);
        double inMaxLatDegs    = double.Parse(parameters[6]);

        GloLLBox llBox = new GloLLBox() {
            MinLatDegs = inMinLatDegs,
            MinLonDegs = inMinLonDegs,
            MaxLatDegs = inMaxLatDegs,
            MaxLonDegs = inMaxLonDegs };

        sb.AppendLine($"Elevation Prep:");
        sb.AppendLine($"- inPatchFilepath: {inPatchFilepath}");
        sb.AppendLine($"- LLBox: {llBox}");


        bool validOperation = true;

        // -------------------------------------------------

        // Convert and validate the inputs
        // if (!System.IO.File.Exists(inPatchFilepath))
        // {
        //     sb.AppendLine($"File not found: {inEleFilename}");
        //     validOperation = false;
        // }

        // -------------------------------------------------

        if (validOperation)
        {
            sb.AppendLine($"Valid operation: Progressing...");

            try
            {
                GloAppFactory.Instance.EleManager.CreatePatchFile(inPatchFilepath, llBox, inNumLatPoints, inNumLonPoints);
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Exception: {ex.Message}");
            }

        }

        // -------------------------------------------------

        sb.AppendLine($"Elevation System Report:");
        //sb.AppendLine(GloAppFactory.Instance.EleSystem.Report());

        return sb.ToString();

    }
}