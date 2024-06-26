using System;

// An angle direction, useful when needing to specify angle deltas. - = anticlockwise, + = clockwise
public enum AngleDirection { Clockwise, Anticlockwise } 
public enum AngleBehavior  { Clamp, Wrap}

// ------------------------------------------------------------------------------------------------

// An angle class to represent an absolute angle in radians, with accessors for degrees. 

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Anything beyond the core responibilites will be in a separate "operations" class.
// - Class will be immutable, as operations will return new instances.

public class FssAngle
{
    // Genuine attributes
    public double Radians { get; private set; }
    public FssDoubleRange Range { get; private set; }

    // Derived attributes

    public double Degrees { get { return FssValueUtils.RadsToDegs(Radians); } }



    public FssAngle(double angRads, FssDoubleRange range)
    {
        Radians = FssValueUtils.NormalizeAngle2Pi(angRads);
        Range = range;
    }

    // --------------------------------------------------------------------------------------------

    // Adding an angle is naturally an anticlockwise rotation. Adding a negative is clockwise.

    public static FssAngle operator +(FssAngle a, double value)
    {
        double newValue = a.Range.Apply(a.Radians + value);
        return new FssAngle(newValue, a.Range);
    }

    public static FssAngle operator +(FssAngle a, FssAngle b)
    {
        if (a.Range != b.Range)
        {
            throw new InvalidOperationException("Cannot add angles with different ranges.");
        }

        double newValue = a.Range.Apply(a.Radians + b.Radians);
        return new FssAngle(newValue, a.Range);
    }

    // --------------------------------------------------------------------------------------------

    // Subtracting an angle is naturally a clockwise rotation. Subtracting a negative is anticlockwise.

    public static FssAngle operator -(FssAngle a, double value)
    {
        double newValue = a.Range.Apply(a.Radians - value);
        return new FssAngle(newValue, a.Range);
    }

    public static FssAngle operator -(FssAngle a, FssAngle b)
    {
        if (a.Range != b.Range)
        {
            throw new InvalidOperationException("Cannot subtract angles with different ranges.");
        }

        double newValue = a.Range.Apply(a.Radians - b.Radians);
        return new FssAngle(newValue, a.Range);
    }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"[Angle:{Radians} rads, {Degrees} degs]";
    }
}

// ------------------------------------------------------------------------------------------------

// A class to represent an angle delta, a relative (not absolute), with a value in radians, and a value in degrees.

// Design Decisions:
// - Class is immutable, and will return new instances for any operations that change the value.


public class FssAngleDelta
{
    // Genuine attributes
    public double Radians { get; private set; }
    public double Degrees { get { return FssValueUtils.RadsToDegs(Radians); } }

    // Derived attributes
    public AngleDirection Direction { get { return Radians < 0 ? AngleDirection.Anticlockwise : AngleDirection.Clockwise; } }

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------
    public FssAngleDelta(double radians)
    {
        Radians = radians;
    }

    
    // Optional: Method to create an AngleDelta in degrees
    public static FssAngleDelta FromDegrees(double degrees)
    {
        return new FssAngleDelta(FssValueUtils.DegsToRads(degrees));
    }

    public override string ToString()
    {
        return $"AngleDelta: {Radians} radians ({Degrees} degrees)";
    }
}

// ------------------------------------------------------------------------------------------------

// define an angle range, with a start, and end, and a rotation direction, all building on top of the 
// basic 0 == east (right) and increasing anticlockwise.
// The class is immutable, and will return new instances for any operations that change the range.



// public class FssAngleRange
// {
//     // Genuine attributes
//     public double StartRads { get; private set; }
//     public FssAngleDelta Delta { get; private set; }
    
//     // Derived attributes
//     public double EndRads { get { return StartRads + Delta.Radians; } }



//     public FssAngleRange(double startRads, double endRads, AngleDirection direction = AngleDirection.Anticlockwise)
//     {
//         StartRads = startRads;

//         Delta = new FssAngleDelta(endRads - startRads);

//         End       = endRads;
//         Direction = direction;
//     }

//     public FssAngleRange(double startRads, FssAngleDelta delta)
//     {
//         Start     = startRads;
//         End       = startRads + delta.Radians;
//         Direction = delta.Direction;
//     }

// }
