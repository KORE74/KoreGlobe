using System;

public static class GloXYZLineOperations
{
    public static bool IsParallel(this GloXYZLine l1, GloXYZLine l2)
    {
        // create the two direction vectors
        GloXYZVector l1Direction = l1.DirectionUnitVector;
        GloXYZVector l2Direction = l2.DirectionUnitVector;

        // check if the two direction vectors are parallel.
        // The dot product returns the cosine of the angle between the two vectors, so parallel equals very close to 1.
        double dp = GloXYZVector.DotProduct(l1Direction, l2Direction);
        return (Math.Abs(dp - 1) < GloConsts.ArbitraryMinDouble);
    }
}
