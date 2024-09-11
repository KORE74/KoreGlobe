
using System;
using System.Collections.Generic;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Add Beam
    // ---------------------------------------------------------------------------------------------

    // Beams are the basic unit of a scan setting up an angular port, scanpatterns are added to them

    public void PlatformAddAntennaPattern(string platName, string elemName)
    {

    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Set Beam Components
    // ---------------------------------------------------------------------------------------------


    // ---------------------------------------------------------------------------------------------
    // MARK: Access AP Element
    // ---------------------------------------------------------------------------------------------

    // Passing the data out of the EventDriver layer piecemeal in simple data types is currently considered
    // too repetitive, without concrete benefit.

    public FssPlatformElementAntennaPatterns? GetElementAntennaPattern(string platName, string elemName)
    {
        FssPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem == null)
        {
            if (basicElem is FssPlatformElementAntennaPatterns elemAP)
            {
                return elemAP;
            }
        }
        return null;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Name Helpers
    // ---------------------------------------------------------------------------------------------

    // public static string ElementNameForBeam(string platName, string emitName, string beamName)
    // {
    //     return $"{platName}_{emitName}_{beamName}";
    // }
//
    //public FssPlatformElementBeam? GetBeamElement(string platName, string elemName)
    //{
        //FssPlatformElement? basicElem = GetElement(platName, elemName);
        //if (basicElem == null)
        //{
            //if (basicElem is FssPlatformElementBeam beam)
            //{
                //return beam;
            //}
        //}
        //return null;
    //}

}
