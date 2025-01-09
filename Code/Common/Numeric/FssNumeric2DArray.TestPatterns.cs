
// Fssbal using FssFloat2DArray  = FssNumeric2DArray<float>;
// Fssbal using FssDouble2DArray = FssNumeric2DArray<double>;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public partial class FssNumeric2DArray<T> where T : struct, INumber<T>
{
    // Create a test pattern array, to detail the specific LL values in an interpolated grid, so we can start to see where calculations are going wrong
    // Odd lines are longitude and even lines are latitude, interpolated across the llbox values
    //
    // Usage example: FssNumeric2DArray<float> testPattern = FssNumeric2DArray<float>.LLATestPattern(4008, 2004, new FssLLBox(-180.0, -90.0, 180.0, 90.0));
    //
    // Note: Need to have the array upside down, so the top left corner is the top left corner of the map
    static public FssNumeric2DArray<T> LLATestPattern(int width, int height, FssLLBox llBox)
    {
        FssNumeric2DArray<T> retGrid = new FssNumeric2DArray<T>(width, height);

        double minLatDegs = llBox.MinLatDegs;
        double latIncDegs = llBox.DeltaLatDegs / (height - 1);

        double minLonDegs = llBox.MinLonDegs;
        double lonIncDegs = llBox.DeltaLonDegs / (width - 1);

        for (int x=0; x<width; x++)
        {
            for (int y=0; y<height; y++)
            {
                // Check if y is odd or even
                if (y % 2 == 0)
                    retGrid[x, y] = T.CreateChecked(minLatDegs + (y * latIncDegs));
                else
                    retGrid[x, y] = T.CreateChecked(minLonDegs + (x * lonIncDegs));
            }
        }

        FssNumeric2DArray<T> retGridrev = retGrid.ReverseRows();

        return retGridrev;
    }


    static public FssNumeric2DArray<T> LatTestPattern(int width, int height, FssLLBox llBox)
    {
        FssNumeric2DArray<T> retGrid = new FssNumeric2DArray<T>(width, height);

        double minLatDegs = llBox.MinLatDegs;
        double latIncDegs = llBox.DeltaLatDegs / (height - 1);

        double minLonDegs = llBox.MinLonDegs;
        double lonIncDegs = llBox.DeltaLonDegs / (width - 1);

        for (int x=0; x<width; x++)
        {
            for (int y=0; y<height; y++)
            {
                retGrid[x, y] = T.CreateChecked(minLatDegs + (y * latIncDegs));
            }
        }

        FssNumeric2DArray<T> retGridrev = retGrid.ReverseRows();

        return retGridrev;
    }

    static public FssNumeric2DArray<T> LonTestPattern(int width, int height, FssLLBox llBox)
    {
        FssNumeric2DArray<T> retGrid = new FssNumeric2DArray<T>(width, height);

        double minLatDegs = llBox.MinLatDegs;
        double latIncDegs = llBox.DeltaLatDegs / (height - 1);

        double minLonDegs = llBox.MinLonDegs;
        double lonIncDegs = llBox.DeltaLonDegs / (width - 1);

        for (int x=0; x<width; x++)
        {
            for (int y=0; y<height; y++)
            {
                retGrid[x, y] = T.CreateChecked(minLonDegs + (x * lonIncDegs));
            }
        }

        FssNumeric2DArray<T> retGridrev = retGrid.ReverseRows();

        return retGridrev;
    }


}
