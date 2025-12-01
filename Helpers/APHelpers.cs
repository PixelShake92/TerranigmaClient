using System;
using System.Collections.Generic;
using Archipelago.Core;
using Archipelago.Core.Models;
using Archipelago.MultiClient.Net.Models;

namespace Helpers
{
    public static class APHelpers
    {
        private static BizHawkSNIClient _gameClient;
        
        public static void SetGameClient(BizHawkSNIClient client)
        {
            _gameClient = client;
        }
        
        /// <summary>
        /// Check if the player is currently in the game (not in menus, etc.)
        /// </summary>
        public static bool isInTheGame()
        {
            // TODO: Implement proper check by reading game state from memory
            // For now, always return true if connected
            return _gameClient?.IsConnected ?? false;
        }
        
        public static async void OnConnectedLogic(object sender, EventArgs args, ArchipelagoClient client)
        {
            Console.WriteLine("Connected to Archipelago server!");
            
            // Get slot data if available
            if (client.Options != null)
            {
                Console.WriteLine("Slot options received:");
                foreach (var kvp in client.Options)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }
            
            // Note: Item syncing is handled by the ItemReceived event
            // No need to manually sync here
        }
        
        public static async void OnDisconnectedLogic(object sender, ConnectionChangedEventArgs args, ArchipelagoClient client)
        {
            Console.WriteLine("Disconnected from Archipelago server.");
            Console.WriteLine("Attempting to reconnect...");
        }
        
        public static void ItemReceivedLogic(object sender, ItemReceivedEventArgs args, ArchipelagoClient client)
        {
            try
            {
                var item = args.Item;
                Console.WriteLine($"Received item: {item.Name} (AP ID: {item.Id})");
                
                // Convert AP item ID to game item ID
                // AP items use base ID 0x54450000 (1413808128)
                const long AP_BASE_ID = 0x54450000;
                int gameItemId = (int)(item.Id - AP_BASE_ID);
                
                Console.WriteLine($"  Game Item ID: 0x{gameItemId:X2}");
                
                // Give the item to the player
                PlayerStateHelpers.GiveItem(gameItemId, item.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AP] Error receiving item: {ex.Message}");
            }
        }
        
        public static void Client_MessageReceivedLogic(object sender, MessageReceivedEventArgs args, ArchipelagoClient client)
        {
            // Display messages from the server
            Console.WriteLine($"[AP] {args.Message}");
        }
        
        public static void Client_LocationCompletedLogic(object sender, LocationCompletedEventArgs e, ArchipelagoClient client)
        {
            // LocationCompletedEventArgs may have different properties depending on library version
            try
            {
                Console.WriteLine($"Location completed event received");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Location completed error: {ex.Message}");
            }
        }
        
        public static void Locations_CheckedLocationsUpdated(System.Collections.ObjectModel.ReadOnlyCollection<long> newCheckedLocations)
        {
            Console.WriteLine($"Locations updated. New checks: {newCheckedLocations.Count}");
            foreach (var locId in newCheckedLocations)
            {
                Console.WriteLine($"  - Location {locId} checked");
            }
        }
    }
}
