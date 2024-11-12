// using System.Collections.Generic;

// // FssCommandModelJsonWrite

// public class FssCommandModelJsonWrite : FssCommand
// {

//     public FssCommandModelJsonWrite()
//     {
//         Signature.Add("model");
//         Signature.Add("jsonwrite");
//     }

//     public override string HelpString => $"{SignatureString} <filename>";

//     public override string Execute(List<string> parameters)
//     {
//         if (parameters.Count < 1)
//         {
//             return "FssCommandModelJsonWrite.Execute -> insufficient parameters";
//         }

//         string filename = parameters[0];
//         string retString = "";

//         // Delete the platform
//         FssEventDriver.ModelToJsonFile(filename);

//         retString = $"Model written to {filename}.";

//         return retString;
//     }
// }
