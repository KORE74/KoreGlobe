using System.Collections.Generic;
using System.Text;

// GloCommandElePrep


// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/GlobeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes

#nullable enable

public class GloCommandEleLoadArc : GloCommand
{
    public GloCommandEleLoadArc()
    {
        Signature.Add("ele");
        Signature.Add("loadarc");
    }

    public override string HelpString => $"{SignatureString} <ASCII Arc Filename> <min lat degs> <min lon degs> <max lat degs> <max lon degs> ";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count < 5)
        {
            return "GloCommandEleLoadArc.Execute -> insufficient parameters";
        }

        string inEleFilename = parameters[0];
        double inMinLatDegs  = double.Parse(parameters[1]);
        double inMinLonDegs  = double.Parse(parameters[2]);
        double inMaxLatDegs  = double.Parse(parameters[3]);
        double inMaxLonDegs  = double.Parse(parameters[4]);

        GloLLBox llBox = new GloLLBox() {
            MinLatDegs = inMinLatDegs,
            MinLonDegs = inMinLonDegs,
            MaxLatDegs = inMaxLatDegs,
            MaxLonDegs = inMaxLonDegs };

        sb.AppendLine($"Elevation Prep:");
        sb.AppendLine($"- inEleFilename: {inEleFilename}");
        sb.AppendLine($"- LLBox: {llBox}");

        bool validOperation = true;

        // -------------------------------------------------

        // Convert and validate the inputs
        if (!System.IO.File.Exists(inEleFilename))
        {
            sb.AppendLine($"File not found: {inEleFilename}");
            validOperation = false;
        }

        // -------------------------------------------------

        if (validOperation)
        {
            sb.AppendLine($"Valid operation: Progressing...");

            GloAppFactory.Instance.EleManager.LoadArcASCIIGridFile(inEleFilename, llBox);

            //float testEleVal = newTile!.ElevationData[1,1];

            //sb.AppendLine($"ele[1,1] = {testEleVal}");
        }

        // -------------------------------------------------

        sb.AppendLine($"Elevation System Report:");
        //sb.AppendLine(GloAppFactory.Instance.EleSystem.Report());

        return sb.ToString();

    }
}