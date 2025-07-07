// using System;
// using System.Collections.Generic;

// /*
// GloXYCompound: A shape that is made up of multiple GloXY shapes, such as a polygon or a multi-line.

// Design Decisions:
// - The object is immutable
// */

// public class GloXYCompound : GloXY
// {
//     public List<GloXY> Shapes { get; }

//     // --------------------------------------------------------------------------------------------
//     // Constructor
//     // --------------------------------------------------------------------------------------------

//     public GloXYCompound(List<GloXY> shapes)
//     {
//         Shapes = shapes;
//     }

//     public GloXYCompound(GloXYCompound compound)
//     {
//         Shapes = new List<GloXY>();
//         foreach (GloXY shape in compound.Shapes)
//             Shapes.Add(shape);
//     }

//     // --------------------------------------------------------------------------------------------
//     // Public methods
//     // --------------------------------------------------------------------------------------------

//     // Look at each shape in the compound and offset it by the given amount
//     // Design Descisions:
//     // - Not wanting to override a common Offset function, and start encumbering the class hierarchy with
//     //   a forced set of functions. So accepting the burden of type checking in the compound shape where it will be used.

//     public GloXYCompound Offset(double x, double y)
//     {
//         List<GloXY> newShapes = new List<GloXY>();
//         foreach (GloXY shape in Shapes)
//         {
//             // determine the type of each shape, and then call the appropriate offset method
//             if      (shape is GloXYPoint)    { newShapes.Add(((GloXYPoint)shape).Offset(x, y));    }
//             else if (shape is GloXYLine)     { newShapes.Add(((GloXYLine)shape).Offset(x, y));     }
//             else if (shape is GloXYCircle)   { newShapes.Add(((GloXYCircle)shape).Offset(x, y));   }
//             else if (shape is GloXYCompound) { newShapes.Add(((GloXYCompound)shape).Offset(x, y)); }
//         }
//         return new GloXYCompound(newShapes);
//     }

// }