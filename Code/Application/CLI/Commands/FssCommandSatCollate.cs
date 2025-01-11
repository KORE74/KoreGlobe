using System.Collections.Generic;
using System.Text;
using System.IO;

using Godot;

#nullable enable

// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/GlobeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes

public class FssCommandSatCollate : FssCommand
{
    public FssCommandSatCollate()
    {
        Signature.Add("sat");
        Signature.Add("collate");
    }

    public override string HelpString => $"{SignatureString} <TileCode>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count != 1)
            return "FssCommandSatCollate.Execute -> parameter count error";

        foreach (string param in parameters)
        {
            sb.AppendLine($"param: {param}");
        }
        // return sb.ToString();

        string inTilecode  = parameters[0];

        bool validOperation = true;

        FssMapTileCode? tileCode = FssMapTileOperations.TileCodeFromString(inTilecode);
        if ((tileCode == null) || (!tileCode.IsValid()))
        {
            sb.AppendLine($"Invalid tile code: {inTilecode}");
            validOperation = false;
        }

        // -------------------------------------------------

        if (validOperation)
        {
            sb.AppendLine($"Valid operation: Progressing...");

            // Get the file paths
            // FssMapTileFilepaths filepaths = new FssMapTileFilepaths(tileCode);

            // if (filepaths.ImageWebpFileExists)
            // {
            //     // Load the parent tile image
            //     Image? srcImage = FssGodotImageOperations.LoadImage(inImagePath);
            //     if (srcImage == null)
            //     {
            //         sb.AppendLine($"Failed to load image: {inImagePath}");
            //         return sb.ToString();
            //     }
            // }


            // // Setup the image division
            // int imageSize       = 1024;
            // int childTilesHoriz = tileCode.ChildCodesWidth();
            // int childTilesVert  = tileCode.ChildCodesHeight();

            // // Get the image region for each child tile
            // Image[,] childImages = FssGodotImageOperations.DivideImage(srcImage, childTilesHoriz, childTilesVert, imageSize, imageSize);

            // // Get the child tile codes
            // List<FssMapTileCode> childTileCodes = tileCode.ChildCodesList();
            // foreach(FssMapTileCode currChildCode in childTileCodes)
            // {
            //     // Get the position in the parent tile (for the image region), and the filenames
            //     Fss2DGridPos        pos       = currChildCode.GridPos;
            //     FssMapTileFilepaths filepaths = new FssMapTileFilepaths(currChildCode);

            //     // Save the image region to the child tile file
            //     FssFileOperations.CreateDirectoryForPath(filepaths.ImageWebpFilepath);
            //     FssGodotImageOperations.SaveImageWebp(childImages[pos.PosX, pos.PosY], filepaths.ImageWebpFilepath);

            //     // Report the operation
            //     sb.AppendLine($"Saved image to: {filepaths.ImageWebpFilepath}");
            // }
        }

        // -------------------------------------------------

        return sb.ToString();
    }
}