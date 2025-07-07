// using System;
// using System.Collections.Generic;

// /*
// GloXYCompound: A shape that is made up of multiple GloXY shapes, such as a polygon or a multi-line.

// Design Decisions:
// - The object is immutable
// */

// public static class GloXYCompoundOperations
// {
//     public static GloXYCompound AppendShape(GloXYCompound compound, GloXY shape)
//     {
//         List<GloXY> newShapes = new List<GloXY>();
//         foreach (GloXY currshape in compound.Shapes)
//             newShapes.Add(currshape);

//         newShapes.Add(shape);

//         return new GloXYCompound(newShapes);
//     }

// }