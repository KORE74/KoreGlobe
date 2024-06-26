using System;
using System.Collections.Generic;

/*
FssXYZCompound: A shape that is made up of multiple FssXY shapes, such as a polygon or a multi-line.

Design Decisions:
- The object is immutable
*/

public class FssXYZCompound : FssXY
{
    public List<FssXYZ> Shapes { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYZCompound(List<FssXYZ> shapes)
    {
        Shapes = shapes;
    }

    public FssXYZCompound(FssXYZCompound compound)
    {
        Shapes = new List<FssXYZ>();
        foreach (FssXYZ shape in compound.Shapes)
            Shapes.Add(shape);
    }

}