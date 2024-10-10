using System.Collections.Generic;
using System.Text;


// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/GlobeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes


public class FssCommandEleSaveTile : FssCommand
{
    public FssCommandEleSaveTile()
    {
        Signature.Add("ele");
        Signature.Add("savetile");
    }

    public override string HelpString => $"{SignatureString} <Ele Tile Filename>  <BL <min lat degs><min lon degs>>  <TR <max lat degs> <max lon degs>>  <lon res> <lat res>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count < 7)
        {
            return "FssCommandEleSaveTile.Execute -> insufficient parameters";
        }

        foreach (string param in parameters)
        {
            sb.AppendLine($"param: {param}");
        }
        // return sb.ToString();

        string inEleFilename = parameters[0];
        double inMinLatDegs  = double.Parse(parameters[1]);
        double inMinLonDegs  = double.Parse(parameters[2]);
        double inMaxLatDegs  = double.Parse(parameters[3]);
        double inMaxLonDegs  = double.Parse(parameters[4]);
        int inLonRes         = int.Parse(parameters[5]);
        int inLatRes         = int.Parse(parameters[6]);

        // sb.AppendLine($"Elevation ve Tile:");
        // sb.AppendLine($"- inEleFilename: {inEleFilename}");

        FssLLBox llBox = new FssLLBox() {
            MinLatDegs = inMinLatDegs,
            MinLonDegs = inMinLonDegs,
            MaxLatDegs = inMaxLatDegs,
            MaxLonDegs = inMaxLonDegs };

        bool validOperation = true;

        // -------------------------------------------------

        // // Convert and validate the inputs
        // if (!System.IO.File.Exists(inEleFilename))
        // {
        //     sb.AppendLine($"File not found: {inEleFilename}");
        //     validOperation = false;
        // }

        // -------------------------------------------------

        if (validOperation)
        {
            sb.AppendLine($"Valid operation: Progressing...");

            FssElevationTile newTile = FssAppFactory.Instance.EleSystem.CreateTile(llBox, inLatRes, inLonRes);

            FssElevationTileIO.WriteToTextFile(newTile, inEleFilename);
        }

        // -------------------------------------------------

        // sb.AppendLine($"Elevation System Report:");
        // sb.AppendLine(FssAppFactory.Instance.EleSystem.Report());

        sb.AppendLine($"Tile saved to: {inEleFilename}");

        return sb.ToString();
    }
}