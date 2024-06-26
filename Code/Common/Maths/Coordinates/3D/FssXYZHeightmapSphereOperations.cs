using System;

public static class FssXYZHeightmapSphereOperations
{

    // Create a simple heightmap sphere with a single radius

    public static FssXYZHeightmapSphere CreateSimpleHeightmapSphere(FssXYZPoint center, double radius)
    {
        Float2DArray hm = new (360, 180);
        hm.SetAllVals((float)radius);
        return new FssXYZHeightmapSphere(center, hm);
    }

    // Create a heightmap with a lobe pattern derived from a sine wave

    public static FssXYZHeightmapSphere CreateLobeHeightmapSphere(FssXYZPoint center, double radius, double lobeHeight, double lobeWidth)
    {
        Float2DArray hm = new (360, 180);

        float core = 1f;
        float mainlobe = 10f;
        float sidelobe1 = 2f;
        float sidelobe2 = 1f;

        hm.SetAllVals(core);

        double val;
        for (int el = 0; el < 50; el++)
        {
            val = core + mainlobe * Math.Sin(el * Math.PI / 50);
            hm.SetRow(el, (float)val);
        }
        for (int el = 50; el < 70; el++)
        {
            val = core + sidelobe1 * Math.Sin(el * Math.PI / 50);
            hm.SetRow(el, (float)val);
        }
        for (int el = 70; el < 90; el++)
        {
            val = core + sidelobe2 * Math.Sin(el * Math.PI / 50);
            hm.SetRow(el, (float)val);
        }

        return new FssXYZHeightmapSphere(center, hm);
    }

}