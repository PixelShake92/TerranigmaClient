using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Archipelago.Core;
using Archipelago.Core.Models;
using Archipelago.Core.Util;
using TerranigmaClient;
using Helpers;

internal class Program
{
    // Static fields for AP reconnection
    private static string apUrl;
    private static string apPort;
    private static string apSlot;
    private static string apPassword;
    private static string apGameName = "Terranigma";
    private static ArchipelagoClient apClient;
    private static DateTime lastApReconnectAttempt = DateTime.MinValue;
    private static readonly TimeSpan ApReconnectCooldown = TimeSpan.FromSeconds(15);
    private static bool isReconnecting = false;
    
    private static async Task Main(string[] args)
    {
        // Global exception handler to prevent crashes
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Console.WriteLine($"[ERROR] Unhandled exception: {(e.ExceptionObject as Exception)?.Message}");
            Console.WriteLine("The client will continue running. Type 'reconnect' to restore AP connection.");
        };
        
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Console.WriteLine($"[ERROR] Unobserved task exception: {e.Exception?.Message}");
            e.SetObserved(); // Prevent the exception from terminating the process
        };
        
        // Connection details
        string url;
        string port;
        string slot;
        string password;
        string gameName = "Terranigma";

        List<ILocation> GameLocations = null;

        ////////////////////////////
        //
        // Main Program Flow
        //
        ////////////////////////////

        // Initialize SNI connection (replaces DuckstationClient)
        BizHawkSNIClient gameClient = null;
        bool clientInitializedAndConnected = false;
        int retryAttempt = 0;

        while (!clientInitializedAndConnected)
        {
            Console.Clear();
            retryAttempt++;
            Console.WriteLine($"\nAttempt #{retryAttempt}:");

            try
            {
                gameClient = new BizHawkSNIClient();
                
                // Connect returns true if successful
                if (gameClient.Connect())
                {
                    clientInitializedAndConnected = true;
                }
                else
                {
                    throw new Exception("Failed to connect to BizHawk SNI");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not find BizHawk with SNI running: {ex.Message}");
                Console.WriteLine("\nPlease ensure:");
                Console.WriteLine("1. BizHawk is running with your SNES ROM loaded");
                Console.WriteLine("2. SNI Lua script is running (Tools -> Lua Console -> Open Script -> SNI.lua)");
                
                // Wait for 5 seconds before the next retry
                await Task.Delay(5000);
            }
        }

        var archipelagoClient = new ArchipelagoClient(gameClient);

        archipelagoClient.CancelMonitors();
        archipelagoClient.Connected -= (sender, args) => APHelpers.OnConnectedLogic(sender, args, archipelagoClient);
        archipelagoClient.Disconnected -= (sender, args) => APHelpers.OnDisconnectedLogic(sender, args, archipelagoClient);
        archipelagoClient.ItemReceived -= (sender, args) => APHelpers.ItemReceivedLogic(sender, args, archipelagoClient);
        archipelagoClient.LocationCompleted -= (sender, args) => APHelpers.Client_LocationCompletedLogic(sender, args, archipelagoClient);

        Console.WriteLine("Successfully connected to BizHawk via SNI.");

        // ============================================
        // Terranigma Chest Flags discovered!
        // Base: $7E0770 (SNI: $F50770)
        // Each chest = 1 bit flag
        // Flag ID = ((address - 0x770) * 8) + bit
        // ============================================
        
        // Remove debug code - go straight to AP connection
        // ============================================
        // END DEBUG
        // ============================================

        // Pass game client to helpers
        Helpers.APHelpers.SetGameClient(gameClient);
        Helpers.PlayerStateHelpers.SetGameClient(gameClient);
        Helpers.GameStateHelpers.SetGameClient(gameClient);

        Console.WriteLine("Enter AP url: eg,archipelago.gg");
        string lineUrl = Console.ReadLine();
        url = string.IsNullOrWhiteSpace(lineUrl) ? "archipelago.gg" : lineUrl;

        Console.WriteLine("Enter Port: eg, 38281");
        port = Console.ReadLine();

        Console.WriteLine("Enter Slot Name:");
        slot = Console.ReadLine();

        Console.WriteLine("Room Password:");
        string linePassword = Console.ReadLine();
        password = string.IsNullOrWhiteSpace(linePassword) ? null : linePassword;

        Console.WriteLine("Details:");
        Console.WriteLine($"URL:{url}:{port}");
        Console.WriteLine($"Slot: {slot}");
        Console.WriteLine($"Password: {password}");

        if (string.IsNullOrWhiteSpace(slot))
        {
            Console.WriteLine("Slot name cannot be empty. Please provide a valid slot name.");
            return;
        }

        Console.WriteLine("Got the details! Attempting to connect to Archipelago server");
        
        // Store for reconnection
        apUrl = url;
        apPort = port;
        apSlot = slot;
        apPassword = password;
        apClient = archipelagoClient;

        // Register event handlers
        archipelagoClient.Connected += (sender, args) => APHelpers.OnConnectedLogic(sender, args, archipelagoClient);
        archipelagoClient.Disconnected += (sender, args) => APHelpers.OnDisconnectedLogic(sender, args, archipelagoClient);
        archipelagoClient.ItemReceived += (sender, args) => APHelpers.ItemReceivedLogic(sender, args, archipelagoClient);
        archipelagoClient.MessageReceived += (sender, args) => APHelpers.Client_MessageReceivedLogic(sender, args, archipelagoClient);
        archipelagoClient.LocationCompleted += (sender, args) => APHelpers.Client_LocationCompletedLogic(sender, args, archipelagoClient);
        archipelagoClient.EnableLocationsCondition = () => Helpers.APHelpers.isInTheGame();

        var cts = new CancellationTokenSource();
        try
        {
            await archipelagoClient.Connect(url + ":" + port, gameName);
            Console.WriteLine("Connected. Attempting to Log in...");
            await archipelagoClient?.Login(slot, password);
            Console.WriteLine("Logged in!");

            while (archipelagoClient.CurrentSession == null)
            {
                Console.WriteLine("Waiting for current session");
                await Task.Delay(1000);
            }

            archipelagoClient.ShouldSaveStateOnItemReceived = false;
            archipelagoClient.CurrentSession.Locations.CheckedLocationsUpdated += APHelpers.Locations_CheckedLocationsUpdated;

            GameLocations = LocationHelpers.BuildLocationList(archipelagoClient.Options);
            
            // DEBUG: Print location info
            Console.WriteLine($"\n=== Monitoring {GameLocations.Count} locations ===");
            foreach (var loc in GameLocations.Take(10))
            {
                var location = loc as Location;
                Console.WriteLine($"  {loc.Name}: ID={loc.Id}, Addr=${location?.Address:X6}");
            }
            Console.WriteLine("  ...");

            // Start our CUSTOM bit-based location monitoring instead of the default
            // The default MonitorLocations uses byte checking which doesn't work for bit flags
            _ = CustomBitBasedLocationMonitor(gameClient, archipelagoClient, cts.Token);

            // Command loop
            while (!cts.Token.IsCancellationRequested)
            {
                var input = Console.ReadLine();
                if (input?.Trim().ToLower() == "exit")
                {
                    cts.Cancel();
                    break;
                }
                else if (input?.Trim().ToLower() == "reconnect")
                {
                    if (isReconnecting)
                    {
                        Console.WriteLine("Already attempting to reconnect...");
                        continue;
                    }
                    
                    isReconnecting = true;
                    Console.WriteLine("Attempting to reconnect to Archipelago...");
                    try
                    {
                        // Disconnect first
                        try { archipelagoClient.Disconnect(); } catch { }
                        await Task.Delay(2000);
                        
                        await archipelagoClient.Connect(url + ":" + port, gameName);
                        await Task.Delay(1000);
                        
                        await archipelagoClient?.Login(slot, password);
                        
                        int waitCount = 0;
                        while (archipelagoClient.CurrentSession == null && waitCount < 15)
                        {
                            Console.WriteLine("Waiting for session...");
                            await Task.Delay(500);
                            waitCount++;
                        }
                        
                        // Extra wait for stability
                        await Task.Delay(3000);
                        
                        if (archipelagoClient.CurrentSession?.Socket?.Connected == true)
                        {
                            Console.WriteLine("Reconnected successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Reconnection failed - session not established");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Reconnection failed: {ex.Message}");
                    }
                    finally
                    {
                        isReconnecting = false;
                    }
                }
                else if (input?.Trim().ToLower().Contains("hint") == true)
                {
                    string hintString = input?.Trim().ToLower() == "hint" ? "!hint" : $"!hint {input.Substring(5).Trim()}";
                    archipelagoClient.SendMessage(hintString);
                }
                else if (input?.Trim().ToLower() == "update")
                {
                    Console.WriteLine($"Updating player state with {archipelagoClient.CurrentSession.Items.AllItemsReceived.Count} items");
                    if (archipelagoClient.GameState.CompletedLocations != null)
                    {
                        PlayerStateHelpers.UpdatePlayerState(archipelagoClient.CurrentSession.Items.AllItemsReceived);
                        Console.WriteLine($"Player state updated. Total Count: {archipelagoClient.CurrentSession.Items.AllItemsReceived.Count}");
                    }
                    else
                    {
                        Console.WriteLine("Cannot update player state: GameState or CompletedLocations is null.");
                    }
                }
                else if (input?.Trim().ToLower() == "debug")
                {
                    // DEBUG: Re-read chest flags using BIT system
                    Console.WriteLine("\n=== DEBUG: Current chest flag values (BIT-BASED) ===");
                    Console.WriteLine("Base address: $7E0770 (SNI: $F50770)");
                    Console.WriteLine("Each chest = 1 bit at (flag_id / 8), bit (flag_id % 8)\n");
                    
                    // Read the flag bytes for Tower chests (128-140)
                    // They map to flag IDs 896-908 which are at $7E0770 byte 0-1
                    for (int testChest = 128; testChest <= 140; testChest++)
                    {
                        uint addr = Addresses.GetChestFlagSNIAddress(testChest);
                        int bit = Addresses.GetChestFlagBit(testChest);
                        byte[] data = gameClient.ReadMemory(addr, 1);
                        bool isSet = (data[0] & (1 << bit)) != 0;
                        Console.WriteLine($"Chest {testChest}: ${addr:X6} bit {bit} = {(isSet ? "OPENED" : "closed")} (byte={data[0]:X2})");
                    }
                    
                    // Also show raw bytes in the flag area
                    Console.WriteLine("\nRaw flag bytes at $F50770-$F50780:");
                    for (int offset = 0; offset < 16; offset++)
                    {
                        uint addr = (uint)(0xF50770 + offset);
                        byte[] data = gameClient.ReadMemory(addr, 1);
                        if (data[0] != 0)
                        {
                            Console.WriteLine($"  ${addr:X6} = {data[0]:X2} ({Convert.ToString(data[0], 2).PadLeft(8, '0')})");
                        }
                    }
                }
                else if (input?.Trim().ToLower() == "locations")
                {
                    // DEBUG: Show completed locations
                    Console.WriteLine($"\n=== Completed Locations: {archipelagoClient.GameState.CompletedLocations?.Count ?? 0} ===");
                    if (archipelagoClient.GameState.CompletedLocations != null)
                    {
                        foreach (var loc in archipelagoClient.GameState.CompletedLocations.Take(20))
                        {
                            Console.WriteLine($"  {loc}");
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"Unknown command: '{input}'. Commands: exit, reconnect, hint, update, debug, locations");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine(ex);
        }
        finally
        {
            Console.WriteLine("Shutting down...");
            gameClient?.Disconnect();
        }
    }
    
    /// <summary>
    /// Custom bit-based location monitoring for Terranigma
    /// Terranigma uses bit flags for chest opened states, not bytes
    /// </summary>
    private static async Task CustomBitBasedLocationMonitor(
        BizHawkSNIClient gameClient, 
        ArchipelagoClient archipelagoClient,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("[Monitor] Starting custom bit-based location monitoring...");
        
        // Track which locations we've already sent
        HashSet<long> sentLocations = new HashSet<long>();
        
        // Track locations that need to be sent when AP reconnects
        HashSet<long> pendingLocations = new HashSet<long>();
        
        // Pre-populate with already completed locations from server
        if (archipelagoClient.GameState?.CompletedLocations != null)
        {
            foreach (var loc in archipelagoClient.GameState.CompletedLocations)
            {
                sentLocations.Add(loc.Id);
            }
        }
        
        // Also check the session's checked locations
        if (archipelagoClient.CurrentSession?.Locations?.AllLocationsChecked != null)
        {
            foreach (var locId in archipelagoClient.CurrentSession.Locations.AllLocationsChecked)
            {
                sentLocations.Add(locId);
            }
        }
        
        Console.WriteLine($"[Monitor] {sentLocations.Count} locations already completed on server");
        
        // Calculate the actual address range we need to read
        // Find min and max flag addresses
        uint minAddr = uint.MaxValue;
        uint maxAddr = uint.MinValue;
        
        var chestInfos = new List<(int chestId, uint sniAddr, int bit, long apId, string name)>();
        
        foreach (var loc in Locations.AllLocations)
        {
            if (loc.ChestId == Locations.PORTRAIT_CHEST_ID) continue;
            
            uint sniAddr = Addresses.GetChestFlagSNIAddress(loc.ChestId);
            int bit = Addresses.GetChestFlagBit(loc.ChestId);
            long apId = Locations.ToAPLocationId(loc.ChestId);
            
            chestInfos.Add((loc.ChestId, sniAddr, bit, apId, $"{loc.MapName} - {loc.OriginalItemName}"));
            
            if (sniAddr < minAddr) minAddr = sniAddr;
            if (sniAddr > maxAddr) maxAddr = sniAddr;
        }
        
        // Calculate read range (add 1 because maxAddr is inclusive)
        uint readSize = maxAddr - minAddr + 1;
        Console.WriteLine($"[Monitor] Flag address range: ${minAddr:X6} - ${maxAddr:X6} ({readSize} bytes)");
        Console.WriteLine($"[Monitor] Watching {chestInfos.Count} chests");
        
        // Show first few chest mappings for debugging
        foreach (var info in chestInfos.Take(5))
        {
            Console.WriteLine($"[Monitor]   Chest {info.chestId}: ${info.sniAddr:X6} bit {info.bit}");
        }
        if (chestInfos.Count > 5)
        {
            Console.WriteLine($"[Monitor]   ... and {chestInfos.Count - 5} more");
        }
        
        int errorCount = 0;
        const int MAX_ERRORS_BEFORE_RECONNECT = 3;
        
        // Main monitoring loop
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Read ALL flag bytes in ONE request
                byte[] flagData = gameClient.ReadMemory(minAddr, (int)readSize);
                
                // Check if read failed
                if (flagData == null || flagData.Length == 0 || !gameClient.IsConnected)
                {
                    if (errorCount == 0)
                    {
                        Console.WriteLine("[Monitor] Connection issue, will auto-reconnect...");
                    }
                    errorCount++;
                    await Task.Delay(2000, cancellationToken); // Wait before retry
                    continue;
                }
                
                // Reset error count on successful read
                if (errorCount > 0)
                {
                    Console.WriteLine("[Monitor] Connection restored!");
                    errorCount = 0;
                }
                
                // Check if AP is connected before processing
                bool apConnected = archipelagoClient?.CurrentSession?.Socket?.Connected ?? false;
                
                // If AP is disconnected, try to reconnect
                if (!apConnected)
                {
                    bool reconnected = await TryReconnectAP();
                    if (reconnected)
                    {
                        apConnected = true;
                    }
                }
                
                // If AP just reconnected and we have pending locations, send them
                if (apConnected && pendingLocations.Count > 0)
                {
                    Console.WriteLine($"[Monitor] AP reconnected - sending {pendingLocations.Count} pending locations");
                    var locationsToSend = pendingLocations.ToArray();
                    pendingLocations.Clear();
                    
                    foreach (var apId in locationsToSend)
                    {
                        try
                        {
                            archipelagoClient.CurrentSession.Locations.CompleteLocationChecks(apId);
                            sentLocations.Add(apId);
                            Console.WriteLine($"[Monitor] Sent pending location {apId} to server");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Monitor] Error sending pending location: {ex.Message}");
                            pendingLocations.Add(apId); // Re-queue for next attempt
                        }
                    }
                }
                
                // Check each chest
                foreach (var (chestId, sniAddr, bit, apId, name) in chestInfos)
                {
                    // Skip if already sent
                    if (sentLocations.Contains(apId)) continue;
                    
                    // Skip if already pending
                    if (pendingLocations.Contains(apId)) continue;
                    
                    // Calculate offset into our read buffer
                    int byteOffset = (int)(sniAddr - minAddr);
                    if (byteOffset < 0 || byteOffset >= flagData.Length) continue;
                    
                    // Check if this chest's bit is set
                    byte flagByte = flagData[byteOffset];
                    bool isOpened = (flagByte & (1 << bit)) != 0;
                    
                    if (isOpened)
                    {
                        // Only log once per chest
                        Console.WriteLine($"[Monitor] CHEST OPENED: {name} (ID={chestId}, ${sniAddr:X6} bit {bit})");
                        
                        // Send to Archipelago if connected
                        if (apConnected)
                        {
                            try
                            {
                                archipelagoClient.CurrentSession.Locations.CompleteLocationChecks(apId);
                                sentLocations.Add(apId);
                                Console.WriteLine($"[Monitor] Sent location {apId} to server");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Monitor] Error sending location: {ex.Message}");
                                pendingLocations.Add(apId); // Queue for retry
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[Monitor] AP disconnected - queuing location {apId}");
                            pendingLocations.Add(apId); // Queue for when AP reconnects
                        }
                    }
                }
                
                // Wait between checks
                await Task.Delay(1000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                if (errorCount == 0)
                {
                    Console.WriteLine($"[Monitor] Error: {ex.Message}");
                }
                errorCount++;
                await Task.Delay(2000, cancellationToken);
            }
        }
        
        Console.WriteLine("[Monitor] Location monitoring stopped");
    }
    
    /// <summary>
    /// Attempt to reconnect to the Archipelago server
    /// </summary>
    private static async Task<bool> TryReconnectAP()
    {
        if (apClient == null || string.IsNullOrEmpty(apUrl)) return false;
        
        // Prevent concurrent reconnection attempts
        if (isReconnecting) return false;
        
        // Cooldown to prevent spam
        if (DateTime.Now - lastApReconnectAttempt < ApReconnectCooldown)
        {
            return false;
        }
        
        isReconnecting = true;
        lastApReconnectAttempt = DateTime.Now;
        
        try
        {
            Console.WriteLine("[Monitor] Attempting to reconnect to Archipelago...");
            
            // Disconnect first if partially connected
            try 
            { 
                apClient.Disconnect(); 
            } 
            catch { }
            
            await Task.Delay(2000); // Let it fully disconnect
            
            await apClient.Connect(apUrl + ":" + apPort, apGameName);
            await Task.Delay(1000); // Let connection establish
            
            await apClient.Login(apSlot, apPassword);
            
            // Wait for session to be established
            int waitCount = 0;
            while (apClient.CurrentSession == null && waitCount < 15)
            {
                await Task.Delay(500);
                waitCount++;
            }
            
            // Extra wait to let internal state settle
            await Task.Delay(3000);
            
            if (apClient.CurrentSession?.Socket?.Connected == true)
            {
                Console.WriteLine("[Monitor] Reconnected to Archipelago!");
                isReconnecting = false;
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Monitor] AP reconnect failed: {ex.Message}");
        }
        
        isReconnecting = false;
        return false;
    }
}

// BizHawk SNI Client that implements IGameClient interface
// This works as a drop-in replacement for DuckstationClient
public class BizHawkSNIClient : Archipelago.Core.IGameClient
{
    private ClientWebSocket webSocket;
    private CancellationTokenSource cancellationTokenSource;
    private string attachedDevice;
    private readonly string host = "localhost";
    private readonly int port = 23074; // Default SNI port
    private readonly object socketLock = new object(); // Thread safety
    private DateTime lastSuccessfulRead = DateTime.MinValue;

    // IGameClient interface properties
    public bool IsConnected { get; set; }
    public int ProcId { get; set; }
    public string ProcessName { get; set; }

    public BizHawkSNIClient()
    {
        cancellationTokenSource = new CancellationTokenSource();
        IsConnected = false;
        ProcId = 0;
        ProcessName = "BizHawk-SNI";
    }

    private DateTime lastConnectionAttempt = DateTime.MinValue;
    private static readonly TimeSpan ConnectionCooldown = TimeSpan.FromSeconds(10);

    private bool EnsureConnected(bool silent = true)
    {
        // If socket is open, we're good
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            return true;
        }
        
        // Cooldown to prevent connection spam
        if (DateTime.Now - lastConnectionAttempt < ConnectionCooldown)
        {
            return false;
        }
        lastConnectionAttempt = DateTime.Now;
        
        // Need to reconnect
        try
        {
            // Clean up old socket properly
            if (webSocket != null)
            {
                try 
                { 
                    if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
                    {
                        webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", CancellationToken.None).Wait(1000);
                    }
                    webSocket.Dispose(); 
                } 
                catch { }
                webSocket = null;
            }
            
            // Cancel old token and create new one
            try { cancellationTokenSource?.Cancel(); } catch { }
            cancellationTokenSource = new CancellationTokenSource();
            
            webSocket = new ClientWebSocket();
            // Disable keep-alive pings - SNI doesn't handle them well
            webSocket.Options.KeepAliveInterval = TimeSpan.Zero;
            
            var uri = new Uri($"ws://{host}:{port}");
            var connectTask = webSocket.ConnectAsync(uri, cancellationTokenSource.Token);
            if (!connectTask.Wait(TimeSpan.FromSeconds(3)))
            {
                return false;
            }

            if (webSocket.State != WebSocketState.Open)
            {
                return false;
            }

            // Re-attach to device if we had one
            if (!string.IsNullOrEmpty(attachedDevice))
            {
                AttachSilent(attachedDevice);
            }
            else
            {
                // Need to find and attach to a device
                var devices = GetDevicesSilent();
                if (devices.Count > 0)
                {
                    AttachSilent(devices[0]);
                    attachedDevice = devices[0];
                }
                else
                {
                    return false;
                }
            }
            
            IsConnected = true;
            return true;
        }
        catch
        {
            IsConnected = false;
            return false;
        }
    }
    
    private List<string> GetDevicesSilent()
    {
        // Same as GetDevices but without printing
        var devices = new List<string>();
        try
        {
            var request = new { Opcode = "DeviceList", Space = "SNES" };
            SendRequest(request);

            var buffer = new byte[4096];
            var segment = new ArraySegment<byte>(buffer);
            var receiveTask = webSocket.ReceiveAsync(segment, cancellationTokenSource.Token);
            receiveTask.Wait(TimeSpan.FromSeconds(2));

            if (receiveTask.Result.MessageType == WebSocketMessageType.Text)
            {
                var json = Encoding.UTF8.GetString(buffer, 0, receiveTask.Result.Count);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("Results", out var results))
                {
                    foreach (var device in results.EnumerateArray())
                    {
                        devices.Add(device.GetString() ?? "");
                    }
                }
            }
        }
        catch { }
        return devices;
    }
    
    private void AttachSilent(string device)
    {
        // Same as Attach but without printing
        try
        {
            var request = new
            {
                Opcode = "Attach",
                Space = "SNES",
                Operands = new[] { device }
            };
            SendRequest(request);
            
            // Wait briefly for the attach to take effect
            // SNI doesn't send a response for Attach, but we need to give it time
            Thread.Sleep(100);
            
            attachedDevice = device;
        }
        catch { }
    }

    private bool hasConnectedOnce = false;
    
    public bool Connect()
    {
        try
        {
            // Connect to SNI WebSocket
            webSocket = new ClientWebSocket();
            // Disable keep-alive pings - SNI doesn't handle them well
            webSocket.Options.KeepAliveInterval = TimeSpan.Zero;
            
            var uri = new Uri($"ws://{host}:{port}");
            var connectTask = webSocket.ConnectAsync(uri, cancellationTokenSource.Token);
            connectTask.Wait(TimeSpan.FromSeconds(5));

            if (webSocket.State != WebSocketState.Open)
            {
                return false;
            }

            // Get available devices
            var devices = GetDevicesSilent();
            
            if (devices.Count == 0)
            {
                if (!hasConnectedOnce)
                    Console.WriteLine("No SNES devices found. Make sure BizHawk is running with a ROM loaded.");
                return false;
            }

            // Only display devices on first connection
            if (!hasConnectedOnce)
            {
                Console.WriteLine($"Found {devices.Count} SNES device(s):");
                for (int i = 0; i < devices.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}: {devices[i]}");
                }
            }

            // Select device (auto-select if only one)
            string deviceToAttach = devices[0];
            if (devices.Count > 1 && !hasConnectedOnce)
            {
                Console.WriteLine($"Enter device number (1-{devices.Count}) or press Enter for first device:");
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input) && 
                    int.TryParse(input, out int deviceIndex) && 
                    deviceIndex > 0 && deviceIndex <= devices.Count)
                {
                    deviceToAttach = devices[deviceIndex - 1];
                }
            }

            // Attach to the selected device
            AttachSilent(deviceToAttach);
            IsConnected = true;
            ProcessName = attachedDevice;
            
            if (!hasConnectedOnce)
            {
                Console.WriteLine($"Attached to: {deviceToAttach}");
                hasConnectedOnce = true;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            if (!hasConnectedOnce)
                Console.WriteLine($"Connection failed: {ex.Message}");
            return false;
        }
    }

    public byte[] ReadMemory(uint address, int length)
    {
        lock (socketLock)
        {
            if (!EnsureConnected())
            {
                return new byte[length];
            }

            try
            {
                var request = new
                {
                    Opcode = "GetAddress",
                    Space = "SNES",
                    Operands = new[] { address.ToString("X"), length.ToString("X") }
                };

                SendRequest(request);

                // Receive binary response - may need multiple receives
                var result = new List<byte>();
                var buffer = new byte[4096];
                var segment = new ArraySegment<byte>(buffer);
                
                while (result.Count < length)
                {
                    var receiveTask = webSocket.ReceiveAsync(segment, cancellationTokenSource.Token);
                    
                    if (!receiveTask.Wait(TimeSpan.FromSeconds(2)))
                    {
                        Console.WriteLine($"[Read] Timeout reading from ${address:X}");
                        IsConnected = false;
                        return new byte[length];
                    }
                    
                    if (receiveTask.Result.MessageType == WebSocketMessageType.Binary)
                    {
                        for (int i = 0; i < receiveTask.Result.Count; i++)
                        {
                            result.Add(buffer[i]);
                        }
                    }
                    else if (receiveTask.Result.MessageType == WebSocketMessageType.Close)
                    {
                        IsConnected = false;
                        return new byte[length];
                    }
                    else
                    {
                        // Got text response (maybe error?) - skip it and try again
                        Console.WriteLine($"[Read] Got text instead of binary, retrying...");
                    }
                }
                
                lastSuccessfulRead = DateTime.Now;
                return result.ToArray();
            }
            catch (Exception ex)
            {
                IsConnected = false;
                return new byte[length];
            }
        }
    }

public void WriteMemory(uint address, byte[] data)
{
    lock (socketLock)
    {
        if (!EnsureConnected())
        {
            Console.WriteLine($"[Write] Cannot write - not connected");
            return;
        }

        try
        {
            var request = new
            {
                Opcode = "PutAddress",
                Space = "SNES",
                Operands = new[] { address.ToString("X"), data.Length.ToString("X") }
            };

            Console.WriteLine($"[Write] Sending PutAddress for ${address:X}, {data.Length} bytes");
            SendRequest(request);

            // Wait a moment for the command to be processed
            Thread.Sleep(50);

            // Send binary data
            Console.WriteLine($"[Write] Sending binary data: {BitConverter.ToString(data)}");
            var sendTask = webSocket.SendAsync(
                new ArraySegment<byte>(data),
                WebSocketMessageType.Binary,
                true,
                cancellationTokenSource.Token);
            
            if (!sendTask.Wait(TimeSpan.FromSeconds(2)))
            {
                Console.WriteLine($"[Write] Timeout");
                IsConnected = false;
                return;
            }
            
            Console.WriteLine($"[Write] Complete");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Write] Error: {ex.Message}");
            IsConnected = false;
        }
    }
}

    public void Disconnect()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            try
            {
                var closeTask = webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing connection",
                    cancellationTokenSource.Token);
                closeTask.Wait(TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during disconnect: {ex.Message}");
            }
        }
        IsConnected = false;
        cancellationTokenSource?.Dispose();
    }

    public void Dispose()
    {
        Disconnect();
        webSocket?.Dispose();
    }

    private List<string> GetDevices()
    {
        var request = new
        {
            Opcode = "DeviceList",
            Space = "SNES"
        };

        SendRequest(request);
        var response = ReceiveTextResponse();

        if (!string.IsNullOrEmpty(response))
        {
            try
            {
                using (var doc = JsonDocument.Parse(response))
                {
                    if (doc.RootElement.TryGetProperty("Results", out var results))
                    {
                        return results.EnumerateArray()
                            .Select(x => x.GetString())
                            .Where(x => x != null)
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing device list: {ex.Message}");
            }
        }

        return new List<string>();
    }

    private void Attach(string deviceName)
    {
        var request = new
        {
            Opcode = "Attach",
            Space = "SNES",
            Operands = new[] { deviceName }
        };

        SendRequest(request);
        attachedDevice = deviceName;
    }

    private void SendRequest(object request)
    {
        var json = JsonSerializer.Serialize(request);
        var bytes = Encoding.UTF8.GetBytes(json);
        
        var sendTask = webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            cancellationTokenSource.Token);
        sendTask.Wait();
    }

    private string ReceiveTextResponse()
    {
        var buffer = new byte[4096];
        var segment = new ArraySegment<byte>(buffer);
        var receiveTask = webSocket.ReceiveAsync(segment, cancellationTokenSource.Token);
        receiveTask.Wait();

        if (receiveTask.Result.MessageType == WebSocketMessageType.Text)
        {
            return Encoding.UTF8.GetString(buffer, 0, receiveTask.Result.Count);
        }

        return null;
    }
}


























































