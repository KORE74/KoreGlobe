// using System.Collections.Generic;

// // GloCommandModelJsonWrite

// public class GloCommandModelJsonWrite : GloCommand
// {

//     public GloCommandModelJsonWrite()
//     {
//         Signature.Add("model");
//         Signature.Add("jsonwrite");
//     }

//     public override string HelpString => $"{SignatureString} <filename>";

//     public override string Execute(List<string> parameters)
//     {
//         if (parameters.Count < 1)
//         {
//             return "GloCommandModelJsonWrite.Execute -> insufficient parameters";
//         }

//         string filename = parameters[0];
//         string retString = "";

//         // Delete the platform
//         GloAppFactory.Instance.EventDriver.ModelToJsonFile(filename);

//         retString = $"Model written to {filename}.";

//         return retString;
//     }
// }
