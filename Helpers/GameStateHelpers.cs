using System;
using TerranigmaClient;

namespace Helpers
{
    public static class GameStateHelpers
    {
        private static BizHawkSNIClient _gameClient;
        
        public static void SetGameClient(BizHawkSNIClient client)
        {
            _gameClient = client;
        }
        
        /// <summary>
        /// Check if a specific chest has been opened
        /// </summary>
        public static bool IsChestOpened(int chestId)
        {
            if (_gameClient == null || !_gameClient.IsConnected)
                return false;
            
            // Chest flags are at $7E0500 + chest_id
            uint flagAddress = Addresses.GetChestFlagSNIAddress(chestId);
            byte[] data = _gameClient.ReadMemory(flagAddress, 1);
            
            // If the byte is non-zero, the chest has been opened
            return data[0] != 0;
        }
        
        /// <summary>
        /// Get the current map ID
        /// </summary>
        public static int GetCurrentMapId()
        {
            if (_gameClient == null || !_gameClient.IsConnected)
                return -1;
            
            // TODO: Read current map from memory
            // uint mapAddress = Addresses.ToSNIAddress(Addresses.CURRENT_MAP);
            // byte[] data = _gameClient.ReadMemory((uint)mapAddress, 2);
            // return data[0] | (data[1] << 8);
            
            return 0;
        }
        
        /// <summary>
        /// Get the player's current gem count
        /// </summary>
        public static int GetGemCount()
        {
            if (_gameClient == null || !_gameClient.IsConnected)
                return 0;
            
            uint gemsAddress = Addresses.ToSNIAddress(Addresses.GEMS_LOW);
            byte[] gems = _gameClient.ReadMemory((uint)gemsAddress, 3);
            
            return BCDToDecimal(gems);
        }
        
        /// <summary>
        /// Get the player's current level
        /// </summary>
        public static int GetPlayerLevel()
        {
            if (_gameClient == null || !_gameClient.IsConnected)
                return 0;
            
            uint levelAddress = Addresses.ToSNIAddress(Addresses.LEVEL_EXP_LOW);
            byte[] data = _gameClient.ReadMemory((uint)levelAddress, 2);
            
            // Level is stored in some format - need to decode
            // This is a placeholder
            return data[1]; // Assuming level is in high byte
        }
        
        /// <summary>
        /// Check if the player has a specific item
        /// </summary>
        public static bool HasItem(int itemId)
        {
            if (_gameClient == null || !_gameClient.IsConnected)
                return false;
            
            var itemInfo = Items.GetItemInfo(itemId);
            
            // Check appropriate inventory section based on item type
            switch (itemInfo.Type)
            {
                case ItemType.Consumable:
                    return HasConsumable(itemId);
                    
                case ItemType.KeyItem:
                    return HasKeyItem(itemId);
                    
                case ItemType.Weapon:
                    return HasWeapon(itemId);
                    
                case ItemType.Armor:
                    return HasArmor(itemId);
                    
                case ItemType.Ring:
                    return HasRing(itemId);
                    
                default:
                    return false;
            }
        }
        
        private static bool HasConsumable(int itemId)
        {
            uint address = Addresses.ToSNIAddress(Addresses.INVENTORY_START);
            byte[] inventory = _gameClient.ReadMemory((uint)address, 26);
            
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] == itemId)
                    return true;
            }
            return false;
        }
        
        private static bool HasKeyItem(int itemId)
        {
            // Check special key item slots first
            switch (itemId)
            {
                case 0x7B: // Speed Shoes
                    var speedShoes = _gameClient.ReadMemory(Addresses.ToSNIAddress(Addresses.KEY_ITEM_SPEED_SHOES), 1);
                    return speedShoes[0] == itemId;
                case 0x7C: // Giant Leaves
                    var giantLeaves = _gameClient.ReadMemory(Addresses.ToSNIAddress(Addresses.KEY_ITEM_GIANT_LEAVES), 1);
                    return giantLeaves[0] == itemId;
                case 0x7D: // Sharp Claws
                    var sharpClaws = _gameClient.ReadMemory(Addresses.ToSNIAddress(Addresses.KEY_ITEM_SHARP_CLAWS), 1);
                    return sharpClaws[0] == itemId;
                case 0x7E: // Air Herb
                    var airHerb = _gameClient.ReadMemory(Addresses.ToSNIAddress(Addresses.KEY_ITEM_AIR_HERB), 1);
                    return airHerb[0] == itemId;
            }
            
            // Check regular key items
            uint address = Addresses.ToSNIAddress(Addresses.KEY_ITEMS_START);
            byte[] keyItems = _gameClient.ReadMemory((uint)address, 16);
            
            for (int i = 0; i < keyItems.Length; i += 2)
            {
                if (keyItems[i] == itemId)
                    return true;
            }
            return false;
        }
        
        private static bool HasWeapon(int itemId)
        {
            uint address = Addresses.ToSNIAddress(Addresses.WEAPONS_START);
            int length = (int)(Addresses.WEAPONS_END - Addresses.WEAPONS_START);
            byte[] weapons = _gameClient.ReadMemory((uint)address, length);
            
            for (int i = 0; i < weapons.Length; i += 2)
            {
                if (weapons[i] == itemId)
                    return true;
            }
            return false;
        }
        
        private static bool HasArmor(int itemId)
        {
            uint address = Addresses.ToSNIAddress(Addresses.ARMOR_START);
            byte[] armor = _gameClient.ReadMemory((uint)address, 20);
            
            for (int i = 0; i < armor.Length; i += 2)
            {
                if (armor[i] == itemId)
                    return true;
            }
            return false;
        }
        
        private static bool HasRing(int itemId)
        {
            uint address = Addresses.ToSNIAddress(Addresses.SPELLS_START);
            byte[] spells = _gameClient.ReadMemory((uint)address, 30);
            
            for (int i = 0; i < spells.Length; i += 2)
            {
                if (spells[i] == itemId)
                    return true;
            }
            return false;
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
    }
}
