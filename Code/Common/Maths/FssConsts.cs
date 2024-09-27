using System;

public static class FssConsts
{
    public const double kPi    = Math.PI;
    public const double kTwoPi = 2 * Math.PI;

    public const double DegsInRads90 = (90 * DegsToRadsMultiplier);

    // --------------------------------------------------------------------------------------------

    // Basic Conversion Consts

    public const double RadsToDegsMultiplier = 180 / Math.PI;
    public const double DegsToRadsMultiplier = Math.PI / 180;

    public const double ArbitraryMinDouble  = 0.00001; // Used to check for values too close to 0.
    public const float  ArbitraryMinFloat   = 0.00001f; // Used to check for values too close to 0.
}

// FssConsts.DegsToRadsMultiplier