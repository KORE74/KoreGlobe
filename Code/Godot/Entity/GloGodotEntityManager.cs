// using System;
// using System.Collections.Generic;
// using System.Text;

// using Godot;

// #nullable enable

// public partial class KoreGodotEntityManager : Node3D
// {
//     // List<GloGodotEntity> EntityList = new List<GloGodotEntity>();

//     public Node3D EntityRootNode   = new Node3D() { Name = "EntityRootNode" };
//     public Node3D UnlinkedRootNode = new Node3D() { Name = "UnlinkedRootNode" };

//     float TimerModelCheck = 0.0f;

//     private Glo1DMappedRange InfographicScaleRange = new Glo1DMappedRange();

//     // --------------------------------------------------------------------------------------------
//     // MARK: Node3D Functions
//     // --------------------------------------------------------------------------------------------

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         Name = "EntityManager";

//         // Setup the Root Nodes.
//         AddChild(EntityRootNode);
//         AddChild(UnlinkedRootNode);

//         // Setup the Infographic Scale Range
//         InfographicScaleRange.AddEntry(1,     1);
//         InfographicScaleRange.AddEntry(4,    16);
//         InfographicScaleRange.AddEntry(8,   100);
//         InfographicScaleRange.AddEntry(10,  400);
//     }

//     // Called every frame. 'delta' is the elapsed time since the previous frame.
//     public override void _Process(double delta)
//     {
//         if (TimerModelCheck < GloCentralTime.RuntimeSecs)
//         {
//             TimerModelCheck = GloCentralTime.RuntimeSecs + 1f;
//             MatchModelPlatforms();

//             // DeleteOrphanedEntities();
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Entities
//     // --------------------------------------------------------------------------------------------
//     // Entities, mainly platforms, are the moving objects in the simulation. They are parented off
//     // EntityRootNode. We need routines to add, delete list and edit them.

//     public bool EntityExists(string entityName)
//     {
//         // Look in the child nodes of the EntityRootNode for the entity.
//         foreach (Node3D currNode in EntityRootNode.GetChildren())
//         {
//             if (currNode.Name == entityName)
//                 return true;
//         }
//         return false;
//     }

//     public void AddEntity(string entityName)
//     {
//         if (!EntityExists(entityName))
//             EntityRootNode.AddChild( new GloGodotEntity() { EntityName = entityName } );
//     }

//     public void RemoveEntity(string entityName)
//     {
//         foreach (Node3D currNode in EntityRootNode.GetChildren())
//         {
//             if (currNode.Name == entityName)
//             {
//                 currNode.QueueFree();
//                 RemoveUnlinkedPlatform(entityName);
//                 return;
//             }
//         }
//     }

//     public GloGodotEntity? GetEntity(string entityName)
//     {
//         foreach (Node3D currNode in EntityRootNode.GetChildren())
//         {
//             if (currNode.Name == entityName)
//                 return currNode as GloGodotEntity;
//         }

//         return null;
//     }

//     public List<string> EntityNames()
//     {
//         List<string> names = new List<string>();

//         foreach (Node3D currNode in EntityRootNode.GetChildren())
//             names.Add(currNode.Name);

//         return names;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Create
//     // --------------------------------------------------------------------------------------------

//     // Create and delete entities to keep uptodate with the model.

//     public void MatchModelPlatforms()
//     {
//         // Get the model
//         List<string> platNames        = GloAppFactory.Instance.EventDriver.PlatformNames();
//         List<string> godotEntityNames = EntityNames();

//         // Compare the two lists, to find the new, the deleted and the consistent
//         List<string> omittedInPresentation = GloStringListOperations.ListOmittedInSecond(platNames, godotEntityNames);
//         List<string> noLongerInModel       = GloStringListOperations.ListOmittedInSecond(godotEntityNames, platNames);
//         List<string> maintainedEnitites    = GloStringListOperations.ListIntersection(platNames, godotEntityNames);

//         bool  addInfographicScale = GloGodotFactory.Instance.UIState.IsRwScale;
//         float scaleModifier       = GloValueUtils.Clamp(GloGodotFactory.Instance.UIState.InfographicScale, 1f, 10f);

//         // Loop through the list of platform names, and the EntityList, match them up.
//         foreach (string currModelName in omittedInPresentation)
//         {
//             AddEntity(currModelName);
//             // AddChaseCam(currModelName);
//         }

//         foreach (string currModelName in noLongerInModel)
//             RemoveEntity(currModelName);

//         foreach (string currModelName in maintainedEnitites)
//         {
//             MatchModelPlatformElements(currModelName);
//             MatchModelPlatform3DModel(currModelName);

//             // Set the scale of the model
//             string platformType = GloAppFactory.Instance.EventDriver.PlatformType(currModelName) ?? "default";

//             SetModelScale(currModelName, platformType, addInfographicScale, scaleModifier);

//             //CheckAABB(currModelName);
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Update - Add Elements
//     // --------------------------------------------------------------------------------------------

//     private void MatchModelPlatformElements(string platName)
//     {
//         List<string> modelElementNames = GloAppFactory.Instance.EventDriver.PlatformElementNames(platName);

//         // List the unlinked elements for the platform.
//         List<string> unlinkedElementNames = UnlinkedElementNames(platName);
//         List<string> linkedElementNames   = LinkedElementNames(platName);

//         // Filter out some keyword elements
//         linkedElementNames = GloStringListOperations.FilterOut(linkedElementNames, "aabb");
//         linkedElementNames = GloStringListOperations.FilterOut(linkedElementNames, "model");

//         {
//             // Debug print out all the platforms and element names
//             string allUnlinkedNames = string.Join(", ", unlinkedElementNames);
//             string allLinkedNames   = string.Join(", ", linkedElementNames);
//             string matchDebug = $"MatchModelPlatformElements: {platName} Unlinked: {allUnlinkedNames} Linked: {allLinkedNames}";
//             GloCentralLog.AddEntry(matchDebug);

//             string allModelNames = string.Join(", ", modelElementNames);
//             GloCentralLog.AddEntry($"MatchModelPlatformElements: {platName} Model: {allModelNames}");
//         }

//         // Join the two lists
//         List<string> allElementNames = new List<string>();
//         allElementNames.AddRange(unlinkedElementNames);
//         allElementNames.AddRange(linkedElementNames);

//         // Loop through the list of model elements, and the EntityList, match them up.
//         List<string> omittedInPresentation = GloStringListOperations.ListOmittedInSecond(modelElementNames, allElementNames);
//         List<string> noLongerInModel       = GloStringListOperations.ListOmittedInSecond(allElementNames, modelElementNames);
//         List<string> maintainedEnitites    = GloStringListOperations.ListIntersection(allElementNames, modelElementNames);

//         foreach (string currElemName in omittedInPresentation)
//         {
//             GloPlatformElement? element = GloAppFactory.Instance.EventDriver.GetElement(platName, currElemName);
//             if (element != null)
//             {
//                 if (element is GloPlatformElementRoute)           AddPlatformElementRoute(platName, currElemName);
//                 if (element is GloPlatformElementBeam)            AddPlatformElementBeam(platName, currElemName);
//                 if (element is GloPlatformElementAntennaPatterns) AddPlatformElementAntennaPatterns(platName, currElemName);

//                 // Contrail
//                 // if (element is GloPlatformElementDome)            AddPlatformElementDome(platName, currElemName);
//             }
//         }

//         // Process each of the maintained elements - updating for differences in the model data.
//         foreach (string currElemName in maintainedEnitites)
//         {
//             GloPlatformElement? element = GloAppFactory.Instance.EventDriver.GetElement(platName, currElemName);
//             if (element != null)
//             {
//                 if (element is GloPlatformElementRoute)           UpdatePlatformElementRoute(platName, currElemName);
//                 if (element is GloPlatformElementBeam)            UpdatePlatformElementBeam(platName, currElemName);
//                 if (element is GloPlatformElementAntennaPatterns) AddPlatformElementAntennaPatterns(platName, currElemName);
//             }
//         }

//         foreach (string currElemName in noLongerInModel)
//         {
//             GD.Print($"DELETING ELEMENT: {platName} {currElemName}");

//             RemoveLinkedElement(platName, currElemName);
//             RemoveUnlinkedElement(platName, currElemName);
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Routes
//     // --------------------------------------------------------------------------------------------

//     public void AddPlatformElementRoute(string platName, string currElemName)
//     {
//         // Get the Route Details
//         GloPlatformElementRoute? route = GloAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as GloPlatformElementRoute;

//         GloGodotPlatformElementRoute newRoute = new GloGodotPlatformElementRoute();

//         if ((route != null) && (newRoute != null))
//         {
//             newRoute.Name = currElemName;
//             newRoute.SetRoutePoints(route!.RoutePoints);

//             // Update visibility based on the UI state
//             newRoute.SetVisibility(GloGodotFactory.Instance.UIState.ShowRoutes);

//             // Add the route to the entity and scene tree
//             AddUnlinkedElement(platName, newRoute);

//             GloCentralLog.AddEntry($"Added route element {currElemName} to {platName}");
//         }
//     }

//     public void UpdatePlatformElementRoute(string platName, string currElemName)
//     {
//         // Get the Route Details
//         GloPlatformElementRoute? route = GloAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as GloPlatformElementRoute;

//         // Get the godot route we'll update
//         GloGodotPlatformElementRoute? routeNode = GetUnlinkedElement(platName, currElemName) as GloGodotPlatformElementRoute;

//         if ((route != null) && (routeNode != null))
//         {
//             // Update visibility based on the UI state
//             routeNode!.SetVisibility(GloGodotFactory.Instance.UIState.ShowRoutes);
//             routeNode!.SetRoutePoints(route.RoutePoints);
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Beams
//     // --------------------------------------------------------------------------------------------

//     public void AddPlatformElementBeam(string platName, string currElemName)
//     {
//         // Get the Beam details: shape, range, etc.
//         GloPlatformElementBeam? beam = GloAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as GloPlatformElementBeam;
//         GloGodotEntity? ent = GetEntity(platName);

//         if ((ent != null) && (beam != null))
//         {
//             // Get the shape: important to determine the type of element to create

//             GloPlatformElementBeam.ScanPatternShape shape = beam.GetScanPatternShape();

//             if (shape == GloPlatformElementBeam.ScanPatternShape.Wedge)
//             {
//                 GloAzElBox azElBox = new GloAzElBox() { MinAzDegs=-10, MaxAzDegs=10, MinElDegs=-10, MaxElDegs=10 };

//                 GloGodotPlatformElementWedge newBeam = new GloGodotPlatformElementWedge();

//                 newBeam.Name            = currElemName;
//                 newBeam.AzElBox         = beam.AzElBox;
//                 newBeam.ElementAttitude = beam.PortAttitude;
//                 newBeam.TxDistanceM     = (float)(beam.DetectionRangeTxM);
//                 newBeam.RxDistanceM     = (float)(beam.DetectionRangeRxM);

//                 GD.Print($"AddPlatformElementBeam: {platName} {currElemName} {newBeam.ElementAttitude}");

//                 AddLinkedElement(platName, newBeam);

//                 // rotate (after adding to parent) to accomodate the -ve Z axis
//                 newBeam.Rotation = new Vector3(0, (float)GloValueUtils.DegsToRads(180), 0);
//             }

//             if (shape == GloPlatformElementBeam.ScanPatternShape.Dome)
//             {
//                 GloGodotPlatformElementDome newDome = new GloGodotPlatformElementDome();
//                 newDome.Name             = currElemName;
//                 newDome.TxDistanceM      = (float)(beam.DetectionRangeTxM);
//                 newDome.RxDistanceM      = (float)(beam.DetectionRangeRxM);
//                 newDome.RotateDegsPerSec = 360f / beam.PeriodSecs;
//                 AddLinkedElement(platName, newDome);
//             }

//             if (shape == GloPlatformElementBeam.ScanPatternShape.Cone)
//             {
//                 GloGodotPlatformElementCone newCone = new GloGodotPlatformElementCone();
//                 newCone.Name = currElemName;
//                 newCone.TxConeLengthM = (float)(beam.DetectionRangeTxM);
//                 newCone.RxConeLengthM = (float)(beam.DetectionRangeRxM);
//                 newCone.ConeAzDegs    = (float)beam.AzElBox.HalfArcAzDegs;
//                 newCone.SourcePlatformName = platName;
//                 newCone.TargetPlatformName = beam.TargetPlatName;
//                 newCone.Targeted           = beam.Targeted;
//                 AddLinkedElement(platName, newCone);

//                 // rotate (after adding to parent) to accomodate the -ve Z axis
//                 newCone.Rotation = new Vector3(0, (float)GloValueUtils.DegsToRads(180), 0);
//             }

//             // if (shape == GloPlatformElementBeam.ScanPatternShape.DomeSector)
//             // {
//             //     GloGodotPlatformElementDomeSector newDomeSector = new GloGodotPlatformElementDomeSector();
//             //     newDomeSector.Name = currElemName;
//             //     newDomeSector.TxDistanceM = (float)(beam.DetectionRangeTxM);
//             //     newDomeSector.RxDistanceM = (float)(beam.DetectionRangeRxM);
//             //     newDomeSector.SectorAzElBox = beam.AzElBox;
//             //     AddLinkedElement(platName, newDomeSector);
//             // }

//             // GloGodotPlatformElementDome newDome = new GloGodotPlatformElementDome();
//             // newDome.RxDistanceM = 50000f;
//             // AddLinkedElement(platName, newDome);
//         }
//     }

//     public void UpdatePlatformElementBeam(string platName, string currElemName)
//     {
//         // Get the Beam details: shape, range, etc.
//         GloPlatformElementBeam? beam = GloAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as GloPlatformElementBeam;
//         GloGodotEntity? ent = GetEntity(platName);

//         GloCentralLog.AddEntry($"################### =====> UpdatePlatformElementBeam: {platName} {currElemName}");

//         if ((ent != null) && (beam != null))
//         {
//             if (beam.ScanShape == GloPlatformElementBeam.ScanPatternShape.Wedge)
//             {
//                 GloGodotPlatformElementWedge? beamNode = GetLinkedElement(platName, currElemName) as GloGodotPlatformElementWedge;
//                 if (beamNode != null)
//                 {
//                     GloAzElBox azElBox = beam.AzElBox;

//                     beamNode.TxDistanceM = (float)(beam.DetectionRangeTxM);
//                     beamNode.RxDistanceM = (float)(beam.DetectionRangeRxM);
//                     beamNode.AzElBox = azElBox;

//                     beamNode.SetVisibility(
//                         GloGodotFactory.Instance.UIState.ShowEmitters && beam.Enabled,
//                         GloGodotFactory.Instance.UIState.ShowRx,
//                         GloGodotFactory.Instance.UIState.ShowTx);
//                 }
//             }
//             if (beam.ScanShape == GloPlatformElementBeam.ScanPatternShape.Dome)
//             {
//                 GloGodotPlatformElementDome? beamNode = GetLinkedElement(platName, currElemName) as GloGodotPlatformElementDome;
//                 beamNode?.SetVisibility(
//                     GloGodotFactory.Instance.UIState.ShowEmitters && beam.Enabled,
//                     GloGodotFactory.Instance.UIState.ShowRx,
//                     GloGodotFactory.Instance.UIState.ShowTx);
//             }
//             if (beam.ScanShape == GloPlatformElementBeam.ScanPatternShape.Cone)
//             {
//                 GloGodotPlatformElementCone? beamNode = GetLinkedElement(platName, currElemName) as GloGodotPlatformElementCone;
//                 beamNode?.SetVisibility(
//                     GloGodotFactory.Instance.UIState.ShowEmitters && beam.Enabled,
//                     GloGodotFactory.Instance.UIState.ShowRx,
//                     GloGodotFactory.Instance.UIState.ShowTx);
//             }
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Antenna Patterns
//     // --------------------------------------------------------------------------------------------

//     public void AddPlatformElementAntennaPatterns(string platName, string currElemName)
//     {
//         GloCentralLog.AddEntry($"AddPlatformElementAntennaPatterns: {platName} {currElemName}");

//         // Get the Antenna Pattern details - from the model
//         GloPlatformElementAntennaPatterns? antPat = GloAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as GloPlatformElementAntennaPatterns;

//         // See if we can get the antenna patterns from the godot presentation
//         GloGodotPlatformElementAntennaPatterns? antPatNode = GetLinkedElement(platName, currElemName) as GloGodotPlatformElementAntennaPatterns;

//         // Get the godot platform we'll attach the element to
//         GloGodotEntity? entNode = GetEntity(platName);

//         // If the node and antenna pattern are not null, we have enough to proceed.
//         if ((entNode != null) && (antPat != null))
//         {
//             // Create a new Antenna Pattern element if it was null
//             if (antPatNode == null)
//             {
//                 // Get the size and distance to the AP to fit with the platform.
//                 double rwLongestAABBOffset = entNode.ModelInfo.RwAABB.LongestOffset();
//                 double geApOffsetDist      = GloZeroOffset.RwToGeDistanceMultiplier * (rwLongestAABBOffset * 2);
//                 double geApMaxAmplitude    = geApOffsetDist * 0.5;

//                 antPatNode = new GloGodotPlatformElementAntennaPatterns();
//                 antPatNode.Name = currElemName;
//                 antPatNode.Name = currElemName;
//                 antPatNode.SetSizeAndDistance((float)geApMaxAmplitude, (float)geApOffsetDist);
//                 AddLinkedElement(platName, antPatNode);
//             }

//             // Now the task is to loop through the antenna patterns and add the ones we don't have
//             // Lets start with lists of the pattern names
//             List<string> modelNames = antPat.PatternNames();
//             List<string> nodeNames  = antPatNode.PatternsList();

//             // Debug print out all the platforms and element names
//             string allModelNames = string.Join(", ", modelNames);
//             string allNodeNames  = string.Join(", ", nodeNames);
//             string matchDebug    = $"AddPlatformElementAntennaPatterns: {platName} {currElemName} Model: {allModelNames} Node: {allNodeNames}";
//             GloCentralLog.AddEntry(matchDebug);

//             // Loop through the model names, then the node names, and add the missing ones
//             List<string> omittedInPresentation = GloStringListOperations.ListOmittedInSecond(modelNames, nodeNames);
//             List<string> noLongerInModel       = GloStringListOperations.ListOmittedInSecond(nodeNames, modelNames);
//             List<string> maintainedEnitites    = GloStringListOperations.ListIntersection(modelNames, nodeNames);

//             foreach (string currPatternName in omittedInPresentation)
//             {
//                 GloAntennaPattern? pattern = antPat.PatternForPortName(currPatternName);
//                 if (pattern != null)
//                 {
//                     antPatNode.AddPattern(pattern);
//                 }
//             }

//             // Update the visibility
//             UpdatePlatformElementAntennaPatterns(platName, currElemName);

//             // Update the scale
//             float scaleModifier = GloValueUtils.Clamp(GloGodotFactory.Instance.UIState.InfographicScale, 1f, 10f);
//             if (GloGodotFactory.Instance.UIState.IsRwScale)
//                 scaleModifier = 1f;
//             SetAPScale(platName, currElemName, scaleModifier);
//         }
//     }

//     public void UpdatePlatformElementAntennaPatterns(string platName, string currElemName)
//     {
//         // Get the godot platform we'll attach the element to
//         GloGodotEntity? ent = GetEntity(platName);

//         // Get the Antenna Pattern details
//         GloPlatformElementAntennaPatterns? antPat = GloAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as GloPlatformElementAntennaPatterns;

//         // Get the linked element node for APs
//         GloGodotPlatformElementAntennaPatterns? antPatNode = GetLinkedElement(platName, currElemName) as GloGodotPlatformElementAntennaPatterns;

//         if ((ent != null) && (antPat != null) && (antPatNode != null))
//         {
//             // Update the visibility of the APs based on the UI state
//             antPatNode.SetVisibility(GloGodotFactory.Instance.UIState.ShowAntennaPatterns);
//         }
//     }

//     public void SetAPScale(string platName, string currElemName, float scaleModifier)
//     {
//         scaleModifier = (float)InfographicScaleRange.GetValue(scaleModifier);

//         // Get the godot platform we'll attach the element to
//         GloGodotEntity? ent = GetEntity(platName);

//         // Get the Antenna Pattern details
//         GloGodotPlatformElementAntennaPatterns? antPatNode = GetLinkedElement(platName, currElemName) as GloGodotPlatformElementAntennaPatterns;

//         if (antPatNode != null)
//             antPatNode.Scale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Reporting
//     // --------------------------------------------------------------------------------------------

//     public string FullReport()
//     {
//         StringBuilder sb = new StringBuilder();


//         List<string> namesList = EntityNames();

//         sb.AppendLine($"Entity Count: {namesList.Count}");
//         foreach (string currName in namesList)
//         {
//             sb.AppendLine($"Entity: {currName}");
//         }

//         return sb.ToString();
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Update - Delete
//     // --------------------------------------------------------------------------------------------

// }


