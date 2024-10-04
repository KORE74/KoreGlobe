using System;
using System.Collections.Generic;
using System.Text;

using Godot;

#nullable enable

public partial class FssGodotEntityManager : Node3D
{
    // List<FssGodotEntity> EntityList = new List<FssGodotEntity>();

    public Node3D EntityRootNode;
    public Node3D UnlinkedRootNode;

    float TimerModelCheck = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "EntityManager";

        // Setup the Entity Root Node.
        EntityRootNode = new Node3D() { Name = "EntityRootNode" };
        AddChild(EntityRootNode);

        // Setup the Element Root Node.
        UnlinkedRootNode = new Node3D() { Name = "UnlinkedRootNode" };
        AddChild(UnlinkedRootNode);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (TimerModelCheck < FssCoreTime.RuntimeSecs)
        {
            TimerModelCheck = FssCoreTime.RuntimeSecs + 1f;
            MatchModelPlatforms();

            // DeleteOrphanedEntities();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Entities
    // --------------------------------------------------------------------------------------------
    // Entities, mainly platforms, are the moving objects in the simulation. They are parented off
    // EntityRootNode. We need routines to add, delete list and edit them.

    public bool EntityExists(string entityName)
    {
        // Look in the child nodes of the EntityRootNode for the entity.
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
                return true;
        }

        return false;
    }

    public void AddEntity(string entityName)
    {
        if (!EntityExists(entityName))
            EntityRootNode.AddChild( new FssGodotEntity() { EntityName = entityName } );
    }

    public void RemoveEntity(string entityName)
    {
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
            {
                currNode.QueueFree();
                RemoveUnlinkedPlatform(entityName);
                return;
            }
        }
    }

    public FssGodotEntity? GetEntity(string entityName)
    {
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
                return currNode as FssGodotEntity;
        }

        return null;
    }

    public List<string> EntityNames()
    {
        List<string> names = new List<string>();

        foreach (Node3D currNode in EntityRootNode.GetChildren())
            names.Add(currNode.Name);

        return names;
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    // Create and delete entities to keep uptodate with the model.

    public void MatchModelPlatforms()
    {
        // Get the model
        List<string> platNames = FssAppFactory.Instance.EventDriver.PlatformNames();
        List<string> godotEntityNames = EntityNames();

        // Compare the two lists, to find the new, the deleted and the consistent
        List<string> omittedInPresentation = FssStringListOperations.ListOmittedInSecond(platNames, godotEntityNames);
        List<string> noLongerInModel       = FssStringListOperations.ListOmittedInSecond(godotEntityNames, platNames);
        List<string> maintainedEnitites    = FssStringListOperations.ListInBoth(platNames, godotEntityNames);

        bool  addInfographicScale = FssGodotFactory.Instance.UIState.IsRwScale;
        float scaleModifier       = FssValueUtils.Clamp(FssGodotFactory.Instance.UIState.InfographicScale, 1f, 10f);


        // Loop through the list of platform names, and the EntityList, match them up.
        foreach (string currModelName in omittedInPresentation)
        {
            AddEntity(currModelName);
            AddChaseCam(currModelName);
        }

        foreach (string currModelName in noLongerInModel)
            RemoveEntity(currModelName);

        foreach (string currModelName in maintainedEnitites)
        {
            MatchModelPlatformElements(currModelName);
            MatchModelPlatform3DModel(currModelName);

            // Set the scale of the model
            string platformType = FssAppFactory.Instance.EventDriver.PlatformType(currModelName) ?? "default";

            SetModelScale(currModelName, platformType, addInfographicScale, scaleModifier);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update - Add Elements
    // --------------------------------------------------------------------------------------------

    private void MatchModelPlatformElements(string platName)
    {
        List<string> modelElementNames = FssAppFactory.Instance.EventDriver.PlatformElementNames(platName);

        // List the unlinked elements for the platform.
        List<string> unlinkedElementNames = UnlinkedElementNames(platName);
        List<string> linkedElementNames   = LinkedElementNames(platName);

        {
            // Debug print out all the platforms and element names
            string allUnlinkedNames = string.Join(", ", unlinkedElementNames);
            string allLinkedNames   = string.Join(", ", linkedElementNames);
            string matchDebug = $"MatchModelPlatformElements: {platName} Unlinked: {allUnlinkedNames} Linked: {allLinkedNames}";
            FssCentralLog.AddEntry(matchDebug);

            string allModelNames = string.Join(", ", modelElementNames);
            FssCentralLog.AddEntry($"MatchModelPlatformElements: {platName} Model: {allModelNames}");
        }

        // Join the two lists
        List<string> allElementNames = new List<string>();
        allElementNames.AddRange(unlinkedElementNames);
        allElementNames.AddRange(linkedElementNames);

        // Loop through the list of model elements, and the EntityList, match them up.
        List<string> omittedInPresentation = FssStringListOperations.ListOmittedInSecond(modelElementNames, allElementNames);
        List<string> noLongerInModel       = FssStringListOperations.ListOmittedInSecond(allElementNames, modelElementNames);
        List<string> maintainedEnitites    = FssStringListOperations.ListInBoth(allElementNames, modelElementNames);

        foreach (string currElemName in omittedInPresentation)
        {
            FssPlatformElement? element = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName);
            if (element != null)
            {
                if (element is FssPlatformElementRoute)           AddPlatformElementRoute(platName, currElemName);
                if (element is FssPlatformElementBeam)            AddPlatformElementBeam(platName, currElemName);
                if (element is FssPlatformElementAntennaPatterns) AddPlatformElementAntennaPatterns(platName, currElemName);

                // Contrail
                // if (element is FssPlatformElementDome)            AddPlatformElementDome(platName, currElemName);
            }
        }

        // Process each of the maintained elements - updating for differences in the model data.
        foreach (string currElemName in maintainedEnitites)
        {
            FssPlatformElement? element = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName);
            if (element != null)
            {
                if (element is FssPlatformElementRoute)           UpdatePlatformElementRoute(platName, currElemName);
                if (element is FssPlatformElementBeam)            UpdatePlatformElementBeam(platName, currElemName);
                if (element is FssPlatformElementAntennaPatterns) UpdatePlatformElementAntennaPatterns(platName, currElemName);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Routes
    // --------------------------------------------------------------------------------------------

    public void AddPlatformElementRoute(string platName, string currElemName)
    {
        // Get the Route Details
        FssPlatformElementRoute? route = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as FssPlatformElementRoute;

        FssGodotPlatformElementRoute newRoute = new FssGodotPlatformElementRoute();
        newRoute.Name = currElemName;
        newRoute.SetRoutePoints(route.RoutePoints);

        // Add the route to the entity and scene tree
        AddUnlinkedElement(platName, newRoute);

        FssCentralLog.AddEntry($"Added route element {currElemName} to {platName}");
    }

    public void UpdatePlatformElementRoute(string platName, string currElemName)
    {
        // Get the Route Details
        FssPlatformElementRoute? route = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as FssPlatformElementRoute;

        // Get the godot route we'll update
        FssGodotPlatformElementRoute? routeNode = GetLinkedElement(platName, currElemName) as FssGodotPlatformElementRoute;

        if ((route != null) && (routeNode != null))
        {
            routeNode.SetRoutePoints(route.RoutePoints);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Beams
    // --------------------------------------------------------------------------------------------

    public void AddPlatformElementBeam(string platName, string currElemName)
    {
        // Get the Beam details: shape, range, etc.
        FssPlatformElementBeam? beam = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as FssPlatformElementBeam;
        FssGodotEntity? ent = GetEntity(platName);

        FssCentralLog.AddEntry($"################### =====> AddPlatformElementBeam: {platName} {currElemName}");

        if ((ent != null) && (beam != null))
        {
            // Get the shape: important to determine the type of element to create

            if (beam.ScanShape == FssPlatformElementBeam.ScanPatternShape.Wedge)
            {
                FssAzElBox azElBox = new FssAzElBox() { MinAzDegs=-10, MaxAzDegs=10, MinElDegs=-10, MaxElDegs=10 };

                FssGodotPlatformElementWedge newBeam = new FssGodotPlatformElementWedge();

                newBeam.Name        = currElemName;
                newBeam.AzElBox     = beam.AzElBox;
                newBeam.TxDistanceM = (float)(beam.DetectionRangeTxM);
                newBeam.RxDistanceM = (float)(beam.DetectionRangeRxM);

                AddLinkedElement(platName, newBeam);

                // rotate (after adding to parent) to accomodate the -ve Z axis
                newBeam.Rotation = new Vector3(0, (float)FssValueUtils.DegsToRads(180), 0);
            }

            if (beam.ScanShape == FssPlatformElementBeam.ScanPatternShape.Dome)
            {
                FssGodotPlatformElementDome newDome = new FssGodotPlatformElementDome();
                newDome.Name        = currElemName;
                newDome.TxDistanceM = (float)(beam.DetectionRangeTxM);
                newDome.RxDistanceM = (float)(beam.DetectionRangeRxM);
                AddLinkedElement(platName, newDome);
            }



            // FssGodotPlatformElementDome newDome = new FssGodotPlatformElementDome();
            // newDome.RxDistanceM = 50000f;
            // AddLinkedElement(platName, newDome);
        }
    }

    public void UpdatePlatformElementBeam(string platName, string currElemName)
    {
        // Get the Beam details: shape, range, etc.
        FssPlatformElementBeam? beam = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as FssPlatformElementBeam;
        FssGodotEntity? ent = GetEntity(platName);

        FssCentralLog.AddEntry($"################### =====> UpdatePlatformElementBeam: {platName} {currElemName}");

        if ((ent != null) && (beam != null))
        {
            FssGodotPlatformElementWedge? beamNode = GetLinkedElement(platName, currElemName) as FssGodotPlatformElementWedge;

            if (beamNode != null)
            {
                FssAzElBox azElBox = beam.AzElBox;

                beamNode.TxDistanceM = (float)(beam.DetectionRangeTxM);
                beamNode.RxDistanceM = (float)(beam.DetectionRangeRxM);
                beamNode.AzElBox = azElBox;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Antenna Patterns
    // --------------------------------------------------------------------------------------------

    public void AddPlatformElementAntennaPatterns(string platName, string currElemName)
    {
        FssCentralLog.AddEntry($"AddPlatformElementAntennaPatterns: {platName} {currElemName}");

        // Get the Antenna Pattern details
        FssPlatformElementAntennaPatterns? antPat = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as FssPlatformElementAntennaPatterns;

        // Get the godot platform we'll attach the element to
        FssGodotEntity? ent = GetEntity(platName);

        if ((ent != null) && (antPat != null))
        {
            FssGodotPlatformElementAntennaPatterns newAntPat = new FssGodotPlatformElementAntennaPatterns();
            newAntPat.Name = currElemName;

            // Get the size and distance to the AP to fit with the platform.
            double rwLongestAABBOffset = ent.ModelInfo.RwAABB.LongestOffset();
            double geApOffsetDist      = FssZeroOffset.RwToGeDistanceMultiplierM * (rwLongestAABBOffset * 3);
            double geApMaxAmplitude    = geApOffsetDist * 0.25;

            // Add the element to the entity and scene tree
            AddLinkedElement(platName, newAntPat);
            newAntPat.SetSizeAndDistance((float)geApMaxAmplitude, (float)geApOffsetDist);

            // transfer the named pattern into the godot element - creating the mesh
            List<string> patternNames = antPat.PatternNames();

            FssCentralLog.AddEntry($"======> AddPlatformElementAntennaPatterns: {platName} {currElemName} Patterns: {string.Join(", ", patternNames)}");

            foreach (string currPatternName in patternNames)
            {
                FssAntennaPattern? pattern = antPat.PatternForPortName(currPatternName);
                if (pattern != null)
                {
                    newAntPat.AddPattern(pattern);
                }
            }
        }
    }

    public void UpdatePlatformElementAntennaPatterns(string platName, string currElemName)
    {
        // Get the godot platform we'll attach the element to
        FssGodotEntity? ent = GetEntity(platName);

        // Get the Antenna Pattern details
        FssPlatformElementAntennaPatterns? antPat = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as FssPlatformElementAntennaPatterns;

        // Get the linked element node for APs
        FssGodotPlatformElementAntennaPatterns? antPatNode = GetLinkedElement(platName, currElemName) as FssGodotPlatformElementAntennaPatterns;


    }

    // --------------------------------------------------------------------------------------------
    // MARK: Reporting
    // --------------------------------------------------------------------------------------------

    public string FullReport()
    {
        StringBuilder sb = new StringBuilder();


        List<string> namesList = EntityNames();

        sb.AppendLine($"Entity Count: {namesList.Count}");
        foreach (string currName in namesList)
        {
            sb.AppendLine($"Entity: {currName}");
        }

        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update - Delete
    // --------------------------------------------------------------------------------------------

}


