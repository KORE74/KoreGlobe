using Godot;
using System.Collections.Generic;

public partial class GloLineMesh3D : Node3D
{
    // Convert the mesh List to a byte array for serialisation and recovery.
    // GloLineMesh3D.ToBytes
    public byte[] ToBytes()
    {
        GloByteArrayWriter bwriter = new GloByteArrayWriter();

        // write out the number of list items
        bwriter.WriteInt(_lines.Count);

        // Loop through each list item
        foreach (var line in _lines)
        {
            Vector3 p1      = line.Item1;
            Vector3 p2      = line.Item2;
            Color   p1color = line.Item3;
            Color   p2color = line.Item4;


            // Write the data to the byte array
            bwriter.WriteFloat(p1.X);
            bwriter.WriteFloat(p1.Y);
            bwriter.WriteFloat(p1.Z);

            bwriter.WriteFloat(p2.X);
            bwriter.WriteFloat(p2.Y);
            bwriter.WriteFloat(p2.Z);

            bwriter.WriteFloat(p1color.R);
            bwriter.WriteFloat(p1color.G);
            bwriter.WriteFloat(p1color.B);

            bwriter.WriteFloat(p1color.R);
            bwriter.WriteFloat(p1color.G);
            bwriter.WriteFloat(p1color.B);
    }

        // Return the byte array
        return bwriter.ToArray();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Read from bytes
    // --------------------------------------------------------------------------------------------

    public void FromBytes(byte[] data)
    {
        GloByteArrayReader breader = new GloByteArrayReader(data);

        // Clear out the current lines
       // Clear();

        // read out the number of list items
        int numLines = breader.ReadInt();

        // Loop through each list item
        for (int i = 0; i < numLines; i++)
        {
            float p1x      = breader.ReadFloat();
            float p1y      = breader.ReadFloat();
            float p1z      = breader.ReadFloat();

            float p2x      = breader.ReadFloat();
            float p2y      = breader.ReadFloat();
            float p2z      = breader.ReadFloat();

            float p1r      = breader.ReadFloat();
            float p1g      = breader.ReadFloat();
            float p1b      = breader.ReadFloat();

            float p2r      = breader.ReadFloat();
            float p2g      = breader.ReadFloat();
            float p2b      = breader.ReadFloat();

            // Create the line
            AddLine(new Vector3(p1x, p1y, p1z), new Vector3(p2x, p2y, p2z), new Color(p1r, p1g, p1b), new Color(p2r, p2g, p2b));
        }
    }
}
