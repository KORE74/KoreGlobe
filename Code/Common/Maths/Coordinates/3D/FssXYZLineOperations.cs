using System;

public static class FssXYZLineOperations
{
    public static bool IsParallel(this FssXYZLine l1, FssXYZLine l2)
    {
        // create the two direction vectors
        FssXYZPoint l1Direction = l1.DirectionUnitVector;
        FssXYZPoint l2Direction = l2.DirectionUnitVector;

        // check if the two direction vectors are parallel.
        // The dot product returns the cosine of the angle between the two vectors, so parallel equals very close to 1.
        double dp = FssXYZPointOperations.DotProduct(l1Direction, l2Direction);
        return (Math.Abs(dp - 1) < FssConsts.ArbitraryMinDouble);
    }
}
