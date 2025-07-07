
using System;
using System.Collections.Generic;

using GloNetworking;

#nullable enable

// Design Decisions:
// - The GloEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GloEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Route
    // ---------------------------------------------------------------------------------------------

    public void PlatformSetRoute(string platName, List<GloLLAPoint> points)
    {
        string elemName = $"{platName}_Route";
        GloPlatformElementRoute? route = GetElement(platName, elemName) as GloPlatformElementRoute;

        if (route == null)
        {
            route = new GloPlatformElementRoute() { Name = elemName };
            route.AddPoints(points);

            GloPlatform? platform = PlatformForName(platName);
            if (platform == null)
            {
                GloCentralLog.AddEntry($"EC0-0017: PlatformSetRoute: Platform {platName} not found.");
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

    public List<GloLLAPoint> PlatformGetRoutePoints(string platName)
    {
        string elemName = $"{platName}_Route";
        GloPlatformElementRoute? route = GetElement(platName, elemName) as GloPlatformElementRoute;

        if (route == null)
        {
            GloCentralLog.AddEntry($"EC0-0018: PlatformGetRoutePoints: Route {elemName} not found.");
            return new List<GloLLAPoint>();
        }

        // Return a new list copying the points (so the caller can't modify the route)
        return new List<GloLLAPoint>(route.Points);
    }

    public void PlatformClearRoute(string platName)
    {
        string elemName = $"{platName}_Route";
        GloPlatformElementRoute? route = GetElement(platName, elemName) as GloPlatformElementRoute;

        if (route == null)
        {
            GloCentralLog.AddEntry($"EC0-0019: PlatformClearRoute: Route {elemName} not found.");
            return;
        }

        // Clear the route
        route.Clear();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Helper Routines
    // ---------------------------------------------------------------------------------------------

    private GloPlatformElementRoute? GetRouteElement(string platName, string elemName)
    {
        GloPlatformElement? element = GetElement(platName, elemName);
        if (element == null)
        {
            GloCentralLog.AddEntry($"EC0-0020: GetRouteElement: Element {elemName} not found.");
            return null;
        }

        if (element is GloPlatformElementRoute)
        {
            return element as GloPlatformElementRoute;
        }
        else
        {
            GloCentralLog.AddEntry($"EC0-0021: GetRouteElement: Element {elemName} is not a route.");
            return null;
        }
    }



}
