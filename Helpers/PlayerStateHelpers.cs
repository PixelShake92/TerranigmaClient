using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Models;
using TerranigmaClient;

namespace Helpers
{
    public static class PlayerStateHelpers
    {
        private static BizHawkSNIClient _gameClient;
        
        // Queue for items that couldn't be given due to full inventory
        private static Queue<(int itemId, string itemName)> _pendingItems = new Queue<(int, string)>();
        
        public static void SetGameClient(BizHawkSNIClient client)
        {
            _gameClient = client;
        }
        
        /// <summary>
        /// Check if there are pending items waiting for inventory space
        /// </summary>
        public static bool HasPendingItems => _pendingItems.Count > 0;
        
        /// <summary>
        /// Try to give pending items if there's now space
        /// </summary>
        public static void TryGivePendingItems()
        {
            if (_pendingItems.Count == 0) return;
            
            // Try to give each pending item
            var stillPending = new Queue<(int itemId, string itemName)>();
            
            while (_pendingItems.Count > 0)
            {
                var (itemId, itemName) = _pendingItems.Dequeue();
                
                // Try to give the item
                if (!TryGiveItemInternal(itemId, itemName))
                {
                    // Still can't give it, re-queue
                    stillPending.Enqueue((itemId, itemName));
                }
            }
            
            // Put any still-pending items back
            _pendingItems = stillPending;
            
            if (_pendingItems.Count > 0)
            {
                Console.WriteLine($"[Pending] Still waiting to give {_pendingItems.Count} items (inventory full)");
            }
        }
        
        /// <summary>
        /// Give an item to the player by writing to game memory
        /// </summary>
        public static void GiveItem(int itemId, string itemName)
        {
            if (_gameClient == null || !_gameClient.IsConnected)
            {
                Console.WriteLine($"Cannot give item {itemName}: Not connected to game");
                return;
            }
            
            Console.WriteLine($"Giving item: {itemName} (0x{itemId:X4})");
            
            if (!TryGiveItemInternal(itemId, itemName))
            {
                // Item couldn't be given (inventory full), queue it
                _pendingItems.Enqueue((itemId, itemName));
                Console.WriteLine($"[Pending] Queued {itemName} - inventory full ({_pendingItems.Count} items waiting)");
            }
        }
        
        /// <summary>
        /// Internal method to try giving an item. Returns false if inventory is full.
        /// </summary>
        private static bool TryGiveItemInternal(int itemId, string itemName)
        {
            var itemInfo = Items.GetItemInfo(itemId);
            
            switch (itemInfo.Type)
            {
                case ItemType.Consumable:
                case ItemType.KeyItem:
                    // Consumables and key items share the same 27-slot inventory
                    return GiveInventoryItem(itemId);
                    
                case ItemType.Weapon:
                    return GiveWeapon(itemId);
                    
                case ItemType.Armor:
                    return GiveArmor(itemId);
                    
                case ItemType.Ring:
                    return GiveRing(itemId);
                    
                case ItemType.Gems:
                    GiveGems(itemId);
                    return true; // Gems always succeed
                    
                default:
                    Console.WriteLine($"Unknown item type for {itemName}");
                    return true; // Don't queue unknown items
            }
        }
        
        /// <summary>
        /// Give a consumable or key item to the shared 27-slot inventory
        /// Special ability items (Speed Shoes, Giant Leaves, Sharp Claws, Air Herb) have fixed slots
        /// Returns false if inventory is full
        /// </summary>
        private static bool GiveInventoryItem(int itemId)
        {
            // Check for special ability items first - they have fixed slots
            switch (itemId)
            {
                case 0x7B: // Speed Shoes
                    _gameClient.WriteMemory(Addresses.ToSNIAddress(Addresses.KEY_ITEM_SPEED_SHOES), new byte[] { (byte)itemId, 0x01 });
                    Console.WriteLine("Added Speed Shoes to ability slot");
                    return true;
                case 0x7C: // Giant Leaves
                    _gameClient.WriteMemory(Addresses.ToSNIAddress(Addresses.KEY_ITEM_GIANT_LEAVES), new byte[] { (byte)itemId, 0x01 });
                    Console.WriteLine("Added Giant Leaves to ability slot");
                    return true;
                case 0x7D: // Sharp Claws
                    _gameClient.WriteMemory(Addresses.ToSNIAddress(Addresses.KEY_ITEM_SHARP_CLAWS), new byte[] { (byte)itemId, 0x01 });
                    Console.WriteLine("Added Sharp Claws to ability slot");
                    return true;
                case 0x7E: // Air Herb
                    _gameClient.WriteMemory(Addresses.ToSNIAddress(Addresses.KEY_ITEM_AIR_HERB), new byte[] { (byte)itemId, 0x01 });
                    Console.WriteLine("Added Air Herb to ability slot");
                    return true;
            }
            
            // Regular inventory items (consumables and key items share the 27 slots)
            const int SLOT_COUNT = 27;
            const int BYTES_PER_SLOT = 2;
            const int EXPECTED_SIZE = SLOT_COUNT * BYTES_PER_SLOT;
            uint baseAddress = Addresses.ToSNIAddress(Addresses.INVENTORY_START);
            
            // Read current inventory
            byte[] inventory = _gameClient.ReadMemory(baseAddress, EXPECTED_SIZE);
            
            if (inventory == null || inventory.Length < EXPECTED_SIZE)
            {
                Console.WriteLine($"Failed to read inventory (got {inventory?.Length ?? 0} bytes, expected {EXPECTED_SIZE})");
                return false;
            }
            
            // For consumables (0x10-0x1A), check if already exists and increment quantity
            bool isConsumable = itemId >= 0x10 && itemId <= 0x1A;
            
            if (isConsumable)
            {
                for (int i = 0; i < SLOT_COUNT; i++)
                {
                    int slotOffset = i * BYTES_PER_SLOT;
                    byte slotItemId = inventory[slotOffset];
                    byte slotQuantity = inventory[slotOffset + 1];
                    
                    if (slotItemId == itemId)
                    {
                        // Item exists, increment quantity (cap at 99)
                        byte newQuantity = (byte)Math.Min(slotQuantity + 1, 99);
                        _gameClient.WriteMemory(baseAddress + (uint)slotOffset + 1, new byte[] { newQuantity });
                        Console.WriteLine($"Incremented {Items.GetItemName(itemId)} quantity to {newQuantity} (slot {i})");
                        return true;
                    }
                }
            }
            else
            {
                // For key items, check if already owned (don't duplicate)
                for (int i = 0; i < SLOT_COUNT; i++)
                {
                    int slotOffset = i * BYTES_PER_SLOT;
                    byte slotItemId = inventory[slotOffset];
                    
                    if (slotItemId == itemId)
                    {
                        Console.WriteLine($"Key item {Items.GetItemName(itemId)} already owned");
                        return true; // Already have it, success
                    }
                }
            }
            
            // Find empty slot (both bytes must be 0x00)
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                int slotOffset = i * BYTES_PER_SLOT;
                
                if (inventory[slotOffset] == 0x00 && inventory[slotOffset + 1] == 0x00)
                {
                    // Write item ID and quantity 1
                    _gameClient.WriteMemory(baseAddress + (uint)slotOffset, new byte[] { (byte)itemId, 0x01 });
                    Console.WriteLine($"Added {Items.GetItemName(itemId)} to slot {i}");
                    return true;
                }
            }
            
            // No empty slots!
            Console.WriteLine($"No empty inventory slots for {Items.GetItemName(itemId)}!");
            return false;
        }
        
        /// <summary>
        /// Give a weapon
        /// Weapon inventory: $7E8048-$7E8067 (16 slots × 2 bytes = ID + Quantity)
        /// Returns false if inventory is full
        /// </summary>
        private static bool GiveWeapon(int itemId)
        {
            const int WEAPON_SLOTS = 16;
            const int BYTES_PER_SLOT = 2;
            uint baseAddress = Addresses.ToSNIAddress(Addresses.WEAPONS_START);
            
            byte[] weapons = _gameClient.ReadMemory(baseAddress, WEAPON_SLOTS * BYTES_PER_SLOT);
            
            if (weapons == null || weapons.Length == 0)
            {
                Console.WriteLine("Failed to read weapon inventory");
                return false;
            }
            
            // Check if weapon already owned
            for (int i = 0; i < weapons.Length; i += BYTES_PER_SLOT)
            {
                if (weapons[i] == itemId)
                {
                    Console.WriteLine($"Weapon {Items.GetItemName(itemId)} already owned");
                    return true;
                }
            }
            
            // Find empty slot
            for (int i = 0; i < weapons.Length; i += BYTES_PER_SLOT)
            {
                if (weapons[i] == 0x00 && weapons[i + 1] == 0x00)
                {
                    _gameClient.WriteMemory(baseAddress + (uint)i, new byte[] { (byte)itemId, 0x01 });
                    Console.WriteLine($"Added weapon {Items.GetItemName(itemId)} to slot {i / BYTES_PER_SLOT}");
                    return true;
                }
            }
            
            Console.WriteLine($"No empty weapon slots for {Items.GetItemName(itemId)}!");
            return false;
        }
        
        /// <summary>
        /// Give armor
        /// Armor inventory: $7E8068-$7E807F (12 slots × 2 bytes = ID + Quantity)
        /// Returns false if inventory is full
        /// </summary>
        private static bool GiveArmor(int itemId)
        {
            const int ARMOR_SLOTS = 12;
            const int BYTES_PER_SLOT = 2;
            uint baseAddress = Addresses.ToSNIAddress(Addresses.ARMOR_START);
            
            byte[] armor = _gameClient.ReadMemory(baseAddress, ARMOR_SLOTS * BYTES_PER_SLOT);
            
            if (armor == null || armor.Length == 0)
            {
                Console.WriteLine("Failed to read armor inventory");
                return false;
            }
            
            // Check if armor already owned
            for (int i = 0; i < armor.Length; i += BYTES_PER_SLOT)
            {
                if (armor[i] == itemId)
                {
                    Console.WriteLine($"Armor {Items.GetItemName(itemId)} already owned");
                    return true;
                }
            }
            
            // Find empty slot
            for (int i = 0; i < armor.Length; i += BYTES_PER_SLOT)
            {
                if (armor[i] == 0x00 && armor[i + 1] == 0x00)
                {
                    _gameClient.WriteMemory(baseAddress + (uint)i, new byte[] { (byte)itemId, 0x01 });
                    Console.WriteLine($"Added armor {Items.GetItemName(itemId)} to slot {i / BYTES_PER_SLOT}");
                    return true;
                }
            }
            
            Console.WriteLine($"No empty armor slots for {Items.GetItemName(itemId)}!");
            return false;
        }
        
        /// <summary>
        /// Give a ring/spell
        /// Ring inventory: $7E8080-$7E8093 (10 slots × 2 bytes = ID + Quantity)
        /// Returns false if inventory is full
        /// </summary>
        private static bool GiveRing(int itemId)
        {
            const int RING_SLOTS = 10;
            const int BYTES_PER_SLOT = 2;
            uint baseAddress = Addresses.ToSNIAddress(Addresses.SPELLS_START);
            
            byte[] rings = _gameClient.ReadMemory(baseAddress, RING_SLOTS * BYTES_PER_SLOT);
            
            if (rings == null || rings.Length == 0)
            {
                Console.WriteLine("Failed to read ring inventory");
                return false;
            }
            
            // Check if ring already owned - if so, increment quantity
            for (int i = 0; i < rings.Length; i += BYTES_PER_SLOT)
            {
                if (rings[i] == itemId)
                {
                    byte newQuantity = (byte)Math.Min(rings[i + 1] + 1, 99);
                    _gameClient.WriteMemory(baseAddress + (uint)i + 1, new byte[] { newQuantity });
                    Console.WriteLine($"Incremented {Items.GetItemName(itemId)} quantity to {newQuantity}");
                    return true;
                }
            }
            
            // Find empty slot
            for (int i = 0; i < rings.Length; i += BYTES_PER_SLOT)
            {
                if (rings[i] == 0x00 && rings[i + 1] == 0x00)
                {
                    _gameClient.WriteMemory(baseAddress + (uint)i, new byte[] { (byte)itemId, 0x01 });
                    Console.WriteLine($"Added ring {Items.GetItemName(itemId)} to slot {i / BYTES_PER_SLOT}");
                    return true;
                }
            }
            
            Console.WriteLine($"No empty ring slots for {Items.GetItemName(itemId)}!");
            return false;
        }
        
        /// <summary>
        /// Give gems to the player
        /// Gem items are encoded as 0x1XYZ where XYZ is the gem amount in BCD-like format
        /// </summary>
        private static void GiveGems(int itemId)
        {
            // Extract gem value using the helper
            int gemValue = Items.GetGemValue(itemId);
            
            if (gemValue <= 0)
            {
                Console.WriteLine($"Invalid gem value for item 0x{itemId:X}");
                return;
            }
            
            // Read current gems (3 bytes, BCD format) from low WRAM
            uint gemsAddress = Addresses.ToSNIAddress(Addresses.GEMS_LOW);
            byte[] currentGems = _gameClient.ReadMemory(gemsAddress, 3);
            
            if (currentGems == null || currentGems.Length < 3)
            {
                Console.WriteLine("Failed to read current gems");
                return;
            }
            
            // Convert BCD to decimal
            int currentValue = BCDToDecimal(currentGems);
            int newValue = Math.Min(currentValue + gemValue, 999999); // Cap at max
            
            // Convert back to BCD
            byte[] newGems = DecimalToBCD(newValue);
            
            // Write new value
            _gameClient.WriteMemory(gemsAddress, newGems);
            Console.WriteLine($"Added {gemValue} gems (new total: {newValue})");
        }
        
        /// <summary>
        /// Convert BCD bytes to decimal
        /// </summary>
        private static int BCDToDecimal(byte[] bcd)
        {
            int result = 0;
            int multiplier = 1;
            
            foreach (byte b in bcd)
            {
                int low = b & 0x0F;
                int high = (b >> 4) & 0x0F;
                result += low * multiplier;
                multiplier *= 10;
                result += high * multiplier;
                multiplier *= 10;
            }
            
            return result;
        }
        
        /// <summary>
        /// Convert decimal to BCD bytes
        /// </summary>
        private static byte[] DecimalToBCD(int value)
        {
            byte[] result = new byte[3];
            
            for (int i = 0; i < 3; i++)
            {
                int low = value % 10;
                value /= 10;
                int high = value % 10;
                value /= 10;
                result[i] = (byte)((high << 4) | low);
            }
            
            return result;
        }
        
        /// <summary>
        /// Update player state from received items
        /// </summary>
        public static void UpdatePlayerState(System.Collections.ObjectModel.ReadOnlyCollection<Archipelago.MultiClient.Net.Models.ItemInfo> allItemsReceived)
        {
            Console.WriteLine($"Updating player state with {allItemsReceived.Count} items");
            
            // First, try to give any pending items
            TryGivePendingItems();
            
            // Give each received item to the player
            foreach (var item in allItemsReceived)
            {
                // AP items use base ID 0x54450000 (1413808128)
                long apBaseId = 0x54450000;
                int gameItemId = (int)(item.ItemId - apBaseId);
                string itemName = item.ItemName ?? "Unknown";
                
                Console.WriteLine($"  Processing: {itemName}");
                Console.WriteLine($"    AP Item ID: {item.ItemId} (0x{item.ItemId:X})");
                Console.WriteLine($"    Game Item ID: {gameItemId} (0x{gameItemId:X2})");
                
                if (gameItemId > 0 && gameItemId < 0x2000)
                {
                    System.Threading.Thread.Sleep(100);
                    GiveItem(gameItemId, itemName);
                }
                else
                {
                    Console.WriteLine($"    WARNING: Item ID {gameItemId} seems invalid, skipping");
                }
            }
            
            // Report pending items
            if (_pendingItems.Count > 0)
            {
                Console.WriteLine($"\n[WARNING] {_pendingItems.Count} items are pending due to full inventory!");
                Console.WriteLine("Clear some inventory space and run 'update' again.");
            }
        }
    }
}
