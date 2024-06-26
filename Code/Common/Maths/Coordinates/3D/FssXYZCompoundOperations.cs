using System;
using System.Collections.Generic;

/*
FssXYZCompound: A shape that is made up of multiple FssXY shapes, such as a polygon or a multi-line.

Design Decisions:
- The object is immutable
*/

public static class FssXYZCompoundOperations
{
    public static FssXYZCompound AppendShape(FssXYZCompound compound, FssXYZ shape)
    {
        List<FssXYZ> newShapes = new List<FssXYZ>();
        foreach (FssXYZ currshape in compound.Shapes)
            newShapes.Add(currshape);

        newShapes.Add(shape);

        return new FssXYZCompound(newShapes);
    }

}