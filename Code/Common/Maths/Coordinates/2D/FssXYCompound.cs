using System;
using System.Collections.Generic;

/*
FssXYCompound: A shape that is made up of multiple FssXY shapes, such as a polygon or a multi-line.

Design Decisions:
- The object is immutable
*/

public class FssXYCompound : FssXY
{
    public List<FssXY> Shapes { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYCompound(List<FssXY> shapes)
    {
        Shapes = shapes;
    }

    public FssXYCompound(FssXYCompound compound)
    {
        Shapes = new List<FssXY>();
        foreach (FssXY shape in compound.Shapes)
            Shapes.Add(shape);
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Look at each shape in the compound and offset it by the given amount
    // Design Descisions:
    // - Not wanting to override a common Offset function, and start encumbering the class hierarchy with
    //   a forced set of functions. So accepting the burden of type checking in the compound shape where it will be used.

    public FssXYCompound Offset(double x, double y)
    {
        List<FssXY> newShapes = new List<FssXY>();
        foreach (FssXY shape in Shapes)
        {
            // determine the type of each shape, and then call the appropriate offset method
            if      (shape is FssXYPoint)    { newShapes.Add(((FssXYPoint)shape).Offset(x, y)); }
            else if (shape is FssXYLine)     { newShapes.Add(((FssXYLine)shape).Offset(x, y)); }
            else if (shape is FssXYCircle)   { newShapes.Add(((FssXYCircle)shape).Offset(x, y)); }
            else if (shape is FssXYCompound) { newShapes.Add(((FssXYCompound)shape).Offset(x, y)); }
        }
        return new FssXYCompound(newShapes);
    }

}