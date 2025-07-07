using System.Collections.Generic;
using System.Text;


// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/GlobeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes


public class GloCommandEleForPos : GloCommand
{
    public GloCommandEleForPos()
    {
        Signature.Add("ele");
        Signature.Add("forpos");
    }

    public override string HelpString => $"{SignatureString} <lat degs> <lon degs>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count < 2)
        {
            return "GloCommandEleForPos.Execute -> insufficient parameters";
        }

        int count = 0;
        foreach (string param in parameters)
        {
            sb.AppendLine($"param[{count}]: {param}");
            count++;
        }
        // return sb.ToString();

        int inLatRes = int.Parse(parameters[0]);
        int inLonRes = int.Parse(parameters[1]);

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

            GloLLPoint pos = new GloLLPoint() { LatDegs = inLatRes, LonDegs = inLonRes };
            float newEle = GloAppFactory.Instance.EleManager.ElevationAtPos(pos);

            //string eleWithReport = GloAppFactory.Instance.EleManager.ElevationAtPosWithReport(pos);


            sb.AppendLine($"Elevation at position: {pos} = {newEle:F2}");
            //sb.AppendLine(eleWithReport);

        }

        // -------------------------------------------------

        return sb.ToString();
    }
}