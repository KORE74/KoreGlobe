
using System;
using System.Collections.Generic;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Route
    // ---------------------------------------------------------------------------------------------

    public void PlatformSetRoute(string platName, List<FssLLAPoint> points)
    {
        string elemName = $"{platName}_Route";
        FssPlatformElementRoute? route = GetElement(platName, elemName) as FssPlatformElementRoute;

        if (route == null)
        {
            route = new FssPlatformElementRoute() { Name = elemName };
            route.AddPoints(points);

            FssPlatform? platform = PlatformForName(platName);
            if (platform == null)
            {
                FssCentralLog.AddEntry($"EC0-0017: PlatformSetRoute: Platform {platName} not found.");
                return;
            }

            platform.AddElement(route);
        }
        else
        {
            route.Clear();
            route.AddPoints(points);
        }
    }

    public List<FssLLAPoint> PlatformGetRoutePoints(string platName)
    {
        string elemName = $"{platName}_Route";
        FssPlatformElementRoute? route = GetElement(platName, elemName) as FssPlatformElementRoute;

        if (route == null)
        {
            FssCentralLog.AddEntry($"EC0-0018: PlatformGetRoutePoints: Route {elemName} not found.");
            return new List<FssLLAPoint>();
        }

        // Return a new list copying the points (so the caller can't modify the route)
        return new List<FssLLAPoint>(route.Points);
    }

    public void PlatformClearRoute(string platName)
    {
        string elemName = $"{platName}_Route";
        FssPlatformElementRoute? route = GetElement(platName, elemName) as FssPlatformElementRoute;

        if (route == null)
        {
            FssCentralLog.AddEntry($"EC0-0019: PlatformClearRoute: Route {elemName} not found.");
            return;
        }

        // Clear the route
        route.Clear();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Helper Routines
    // ---------------------------------------------------------------------------------------------

    private FssPlatformElementRoute? GetRouteElement(string platName, string elemName)
    {
        FssPlatformElement? element = GetElement(platName, elemName);
        if (element == null)
        {
            FssCentralLog.AddEntry($"EC0-0020: GetRouteElement: Element {elemName} not found.");
            return null;
        }

        if (element is FssPlatformElementRoute)
        {
            return element as FssPlatformElementRoute;
        }
        else
        {
            FssCentralLog.AddEntry($"EC0-0021: GetRouteElement: Element {elemName} is not a route.");
            return null;
        }
    }



}
