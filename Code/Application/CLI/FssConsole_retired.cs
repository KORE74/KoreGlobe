// using System;
// using System.Threading;
// using System.Collections.Generic;
// using System.Linq.Expressions;

// #nullable enable

// // Class to run a thread for command line input.
// // This class can't do anything, it must delgate all actual processing to the FssEventDriver class.
// public class FssConsole
// {
//     // Thread control
//     private Thread? consoleThread = null;
//     private bool running;
//     private Dictionary<string, Action<string[]>> commandHandlers;

//     public FssEventDriver? EventDriver;

//     // ---------------------------------------------------------------------------------------------
//     // Constructor
//     // ---------------------------------------------------------------------------------------------

//     public FssConsole()
//     {
//         running = false;

//         commandHandlers = new Dictionary<string, Action<string[]>>();
//         InitializeCommands();
//     }

//     // ---------------------------------------------------------------------------------------------
//     // Thread control
//     // ---------------------------------------------------------------------------------------------

//     public void Start()
//     {
//         running = true;
//         consoleThread = new Thread(ConsoleLoop);
//         consoleThread?.Start();
//     }

//     public void Stop()
//     {
//         running = false;
//         consoleThread = null;
//     }

//     public void WaitForExit()
//     {
//         FssCentralLog.AddEntry("Network: Waiting on Join()...");
//         consoleThread?.Join(); // This will block until consoleThread finishes execution
//         FssCentralLog.AddEntry("Network: Join() returned.");
//     }

//     // ---------------------------------------------------------------------------------------------
//     // Console commands setup and run
//     // ---------------------------------------------------------------------------------------------

//     private void InitializeCommands()
//     {
//         // Register commands and their handlers here

//         // General app control commands
//         commandHandlers.Add("help", CmdHelp);
//         commandHandlers.Add("version", CmdVersion);
//         commandHandlers.Add("#", CmdComment);
//         commandHandlers.Add("quit", CmdQuit);
//         commandHandlers.Add("exit", CmdQuit);
//         commandHandlers.Add("runfile", CmdRunFile);

//         // MapUtils
//         commandHandlers.Add("tilecode", CmdTileCode);

//         // Map handlers
//         commandHandlers.Add("setroot", CmdSetRoot);
//         commandHandlers.Add("getroot", CmdGetRoot);
//         commandHandlers.Add("create", CmdCreate);

//         // Networking
//         commandHandlers.Add("network", CmdNetwork);
//     }

//     private void ConsoleLoop()
//     {
//         FssCentralLog.AddEntry("Console: thread started.");
//         while (running)
//         {
//             string? input = Console.ReadLine();
//             ProcessCommand(input ?? string.Empty);
//         }
//         FssCentralLog.AddEntry("Console: thread ended.");
//     }

//     private void ProcessCommand(string input)
//     {
//         // Split the input into tokens
//         string[] tokens = input.Split(' ');
//         string command = tokens[0].ToLower().Trim();

//         // ignore run attempt if the string is plainly invalid
//         if (string.IsNullOrEmpty(command))
//             return;

//         // echo the command
//         // Console.WriteLine($">> Running >> {input}");

//         if (commandHandlers.TryGetValue(command, out Action<string[]>? handler))
//         {
//             handler?.Invoke(tokens);
//         }
//         else
//         {
//             Console.WriteLine("Unknown command. Type 'help' for a list of commands.");
//         }
//     }

//     // ---------------------------------------------------------------------------------------------
//     // Command functions:
//     // ---------------------------------------------------------------------------------------------
//     // - private void Cmd<Name>(string[] args)

//     private void CmdHelp(string[] args)
//     {
//         Console.WriteLine("Available commands:");
//         foreach (var cmd in commandHandlers.Keys)
//         {
//             Console.WriteLine($"- {cmd}");
//         }
//     }

//     private void CmdVersion(string[] args)
//     {
//         Console.WriteLine($"Version: {FssGlobals.VersionString}");
//     }

//     // ---------------------------------------------------------------------------------------------

//     // Command to run a file of commands - blocking on each one.

//     private void CmdRunFile(string[] args)
//     {
//         if (args.Length < 2)
//         {
//             Console.WriteLine("Usage: run <filename>");
//             return;
//         }

//         string filename = args[1];

//         if (!System.IO.File.Exists(filename))
//         {
//             Console.WriteLine($"File does not exist: {filename}");
//             return;
//         }

//         Console.WriteLine($"Running file: {filename}");

//         string[] lines = System.IO.File.ReadAllLines(filename);

//         foreach (string line in lines)
//         {
//             Console.WriteLine($"FILE>> {line}");
//             ProcessCommand(line);
//         }
//     }

//     // ---------------------------------------------------------------------------------------------

//     private void CmdComment(string[] args)
//     {
//     }

//     private void CmdQuit(string[] args)
//     {
//         EventDriver?.ExitApplication();
//     }

//     // ---------------------------------------------------------------------------------------------

//     private void CmdSetRoot(string[] args)
//     {
//         if (args.Length < 2)
//         {
//             Console.WriteLine("Usage: setroot <path>");
//             return;
//         }

//         string rootPath = args[1];

//         Console.WriteLine($"Setting root to: {rootPath}");

//         EventDriver?.SetRootDir(rootPath);

//         string? feedbackStr = EventDriver?.ReportRootDir();
//         Console.WriteLine($"Root dir: {feedbackStr}");
//     }

//     private void CmdGetRoot(string[] args)
//     {
//         if (args.Length != 1)
//         {
//             Console.WriteLine("Usage: getroot");
//             return;
//         }

//         string? feedbackStr = EventDriver?.ReportRootDir();
//         Console.WriteLine($"Root dir: {feedbackStr}");
//     }

//     // ---------------------------------------------------------------------------------------------

//     private void CmdTileCode(string[] args)
//     {
//         if (args.Length < 3)
//         {
//             Console.WriteLine("Usage: tilecode frompos <lvl> <latDegs> <lonDegs>");
//             Console.WriteLine("Usage: tilecode fromstr <tilecodestr>");
//             return;
//         }

//         string commandtype = args[1];

//         switch (commandtype)
//         {
//             // case "frompos":
//             //     {
//             //         if (args.Length != 5)
//             //         {
//             //             Console.WriteLine("Usage: tilecode frompos <lvl> <latDegs> <lonDegs>");
//             //             return;
//             //         }

//             //         int   lvl     = int.Parse(args[2]);
//             //         float latDegs = float.Parse(args[3]);
//             //         float lonDegs = float.Parse(args[4]);

//             //         // Convert the user-input geo lat/lon to a map (0,0 top left) lat/lon for the tile operations
//             //         float mapLatDegs = (float)FssMapTileCode.GeoLatToMapLat(latDegs);
//             //         float mapLonDegs = (float)FssMapTileCode.GeoLonToMapLon(lonDegs);
//             //         Console.WriteLine($"GeoLatDegs to MapLatDegs: {latDegs:F2} -> {mapLatDegs:F2}");
//             //         Console.WriteLine($"GeoLonDegs to MapLonDegs: {lonDegs:F2} -> {mapLonDegs:F2}");

//             //         FssMapTileCode tileCode = new FssMapTileCode(lvl, mapLatDegs, mapLonDegs);
//             //         Console.WriteLine($"TileCodeStr: {tileCode.TileCodeStr}");
//             //     }
//             //     break;
//             // case "fromlvlpos":
//             //     {
//             //         if (args.Length != 5)
//             //         {
//             //             Console.WriteLine("Usage: tilecode fromlvlpos <lvl> <latDegs> <lonDegs>");
//             //             return;
//             //         }

//             //         int   lvl     = int.Parse(args[2]);
//             //         float latDegs = float.Parse(args[3]);
//             //         float lonDegs = float.Parse(args[4]);

//             //         // Convert the user-input geo lat/lon to a map (0,0 top left) lat/lon for the tile operations
//             //         float mapLatDegs = (float)FssMapTileCode.GeoLatToMapLat(latDegs);
//             //         float mapLonDegs = (float)FssMapTileCode.GeoLonToMapLon(lonDegs);
//             //         Console.WriteLine($"GeoLatDegs to MapLatDegs: {latDegs:F2} -> {mapLatDegs:F2}");
//             //         Console.WriteLine($"GeoLonDegs to MapLonDegs: {lonDegs:F2} -> {mapLonDegs:F2}");

//             //         string tileCode = FssMapTileCode.GetTileCodeStr(lvl, (double)mapLatDegs, (double)mapLonDegs);
//             //         Console.WriteLine($"TileCodeStr: {tileCode}");
//             //     }
//             //     break;
//             case "fromstr":
//                 {
//                     if (args.Length != 3)
//                     {
//                         Console.WriteLine("Usage: tilecode fromstr <tilecodestr>");
//                         return;
//                     }

//                     string tilecodeStr = args[2];

//                     // FssMapTileCode tileCode = new FssMapTileCode(tilecodeStr);
//                     // Console.WriteLine($"TileCodeStr: {tileCode.TileCodeStr}");
//                 }
//                 break;
//             default:
//                 Console.WriteLine("Unknown command type: " + commandtype);
//                 break;
//         }


//     }

//     // ---------------------------------------------------------------------------------------------

//     private void CmdCreate(string[] args)
//     {
//         if (args.Length < 2)
//         {
//             Console.WriteLine("Usage: create dirs // Create the basic directory structure for a previously defined root directory");
//             //Console.WriteLine("Usage: create tile <TileCode> <LonRes> // LatRes is autocalculated to keep the tile square");
//             return;
//         }

//         string type = args[1];

//         switch (type)
//         {
//             case "dirs":
//             {
//                 if (args.Length != 2)
//                 {
//                     Console.WriteLine("Usage: create base // Create the basic directory structure for a previously defined root directory");
//                     return;
//                 }
//                 Console.WriteLine("Creating base directories...");
//                 EventDriver?.CreateBaseDirectories();
//                 break;
//             }

//             case "tileele":
//             {
//                 if (args.Length < 4)
//                 {
//                     Console.WriteLine("Usage: create tileele <TileCode> <horizres>)> // VertRes is autocalculated to keep the grid points square");
//                     return;
//                 }
//                 string tilecodeStr = args[2];
//                 int horizRes = int.Parse(args[3]);

//                 // EventDriver?.CreateTileEle(tilecodeStr, horizRes);
//                 break;
//             }
//             // case "map":
//             //     //CreateMap(name);
//             //     break;
//             default:
//                 Console.WriteLine("Unknown type: " + type);
//                 break;
//         }
//     }

//     // ---------------------------------------------------------------------------------------------

//     private void CmdNetwork(string[] args)
//     {
//         if (args.Length < 2)
//         {
//             Console.WriteLine("Usage: network start      // Start the network comms hub");
//             Console.WriteLine("Usage: network [end|stop] // End the network comms hub");
//             Console.WriteLine("Usage: network stats      // Show network stats");
//             //Console.WriteLine("Usage: create tile <TileCode> <LonRes> // LatRes is autocalculated to keep the tile square");
//             return;
//         }

//         string type = args[1];

//         switch (type)
//         {
//             case "start":
//             {
//                 if (args.Length != 2)
//                 {
//                     Console.WriteLine("Usage: network start // Start the network comms hub");
//                     return;
//                 }
//                 EventDriver?.StartNetworking();
//                 break;
//             }
//             case "stop":
//             case "end":
//             {
//                 if (args.Length != 2)
//                 {
//                     Console.WriteLine("Usage: network end // End the network comms hub");
//                     return;
//                 }
//                 EventDriver?.StopNetworking();
//                 break;
//             }

//             case "localip":
//             {
//                 if (args.Length != 2)
//                 {
//                     Console.WriteLine("Usage: network localip // Show local IP address");
//                     return;
//                 }
//                 string reportStr = EventDriver?.ReportLocalIP() ?? "Unknown IP";
//                 Console.WriteLine($"Local IP: {reportStr}");
//                 break;
//             }

//             case "stats":
//             {
//                 if (args.Length != 2)
//                 {
//                     Console.WriteLine("Usage: network stats // Show network stats");
//                     return;
//                 }
//                 string reportStr = EventDriver?.ReportNetworkingStatus() ?? "No Network Comms Hub";
//                 Console.WriteLine($"{reportStr}");
//                 break;
//             }

//             default:
//                 Console.WriteLine("Unknown type: " + type);
//                 break;
//         }
//     }
// }
