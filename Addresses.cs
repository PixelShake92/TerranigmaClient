namespace TerranigmaClient
{
    /// <summary>
    /// Memory addresses for Terranigma (SNES)
    /// All addresses are SNES WRAM addresses ($7E:xxxx)
    /// For SNI, these need to be converted to the appropriate address space
    /// </summary>
    public static class Addresses
    {
        // ============================================
        // SNI Address Mapping Notes:
        // BizHawk's WRAM domain has different offsets for different regions:
        // - Low WRAM ($7E0000-$7E7FFF): Maps to SNI $F50000 + offset
        // - High WRAM ($7E8000-$7EFFFF): Maps to SNI $F60000 + (offset - 0x8000)
        // ============================================
        
        // Base offsets for WRAM in SNI
        public const uint SNI_WRAM_BASE_LOW = 0xF50000;   // For $7E0000-$7E7FFF (chest flags, game state)
        public const uint SNI_WRAM_BASE_HIGH = 0xF60000;  // For $7E8000+ (inventory, stats)
        
        // ============================================
        // Player Stats
        // ============================================
        public const uint LEVEL_EXP_LOW = 0x0690;      // Level/EXP byte 0
        public const uint LEVEL_EXP_HIGH = 0x0691;     // Level/EXP byte 1
        public const uint GEMS_LOW = 0x0694;           // Gems byte 0 (BCD)
        public const uint GEMS_MID = 0x0695;           // Gems byte 1 (BCD)
        public const uint GEMS_HIGH = 0x0696;          // Gems byte 2 (BCD)
        
        // ============================================
        // Game Progress Flags
        // ============================================
        public const uint PROGRESS_FLAGS_1 = 0x06C4;   // Game progress flags
        public const uint PROGRESS_FLAGS_2 = 0x06C5;   // More progress flags
        public const uint PROGRESS_FLAGS_3 = 0x06C7;   // More progress flags
        public const uint PROGRESS_FLAGS_4 = 0x06DF;   // More progress flags
        public const uint TOWER_ACCESS = 0x06E0;       // Tower access flags
        public const uint CRYSTAL_THREAD_SEQ = 0x06E3; // Crystal Thread sequence flag
        public const uint PROGRESS_FLAGS_5 = 0x0708;   // Progress flags
        public const uint TOWER1_DOORS = 0x0710;       // Tower 1 doors
        public const uint GATE_FLAGS = 0x0712;         // Gate flags
        
        // ============================================
        // Inventory - Consumables & Key Items
        // All inventory uses ID + Quantity format (2 bytes per slot)
        // ============================================
        public const uint INVENTORY_START = 0x8000;    // Consumable inventory start
        public const uint INVENTORY_END = 0x8035;      // Consumable inventory end (27 slots × 2 bytes = 54 bytes)
        public const uint KEY_ITEMS_START = 0x8036;    // Key items block start
        public const uint KEY_ITEMS_END = 0x8047;      // Key items block end (9 slots × 2 bytes = 18 bytes)
        
        // Key item slots (each is 2 bytes: item ID + quantity)
        public const uint KEY_ITEM_SPEED_SHOES = 0x8038;   // Slot 1 - Speed Shoes (0x7B)
        public const uint KEY_ITEM_GIANT_LEAVES = 0x803A;  // Slot 2 - Giant Leaves (0x7C)
        public const uint KEY_ITEM_SHARP_CLAWS = 0x803C;   // Slot 3 - Sharp Claws (0x7D)
        public const uint KEY_ITEM_AIR_HERB = 0x803E;      // Slot 4 - Air Herb (0x7E)
        
        // ============================================
        // Inventory - Equipment
        // All equipment uses ID + Quantity format (2 bytes per slot)
        // ============================================
        public const uint WEAPONS_START = 0x8048;      // Weapons block start
        public const uint WEAPONS_END = 0x8067;        // Weapons block end (16 slots × 2 bytes = 32 bytes)
        public const uint ARMOR_START = 0x8068;        // Armor block start (12 slots × 2 bytes = 24 bytes)
        public const uint ARMOR_END = 0x807F;          // Armor block end
        public const uint RINGS_START = 0x8080;        // Ring/Spell block start
        public const uint RINGS_END = 0x8093;          // Ring/Spell block end (10 slots × 2 bytes = 20 bytes)
        
        // Legacy alias for backwards compatibility
        public const uint SPELLS_START = RINGS_START;
        
        // ============================================
        // Current Equipment
        // ============================================
        // TODO: Find addresses for currently equipped weapon/armor
        
        // ============================================
        // Map/Location
        // ============================================
        // TODO: Find current map ID address
        public const uint CURRENT_MAP = 0x0000;        // Placeholder - need to find
        
        // ============================================
        // Chest Flags - BIT-BASED SYSTEM (VERIFIED)
        // ============================================
        // Two separate flag regions based on chest ID:
        //
        // Chests with ID < 128 (surface world):
        //   Base: $7E0760
        //   flag_id = chestId
        //   Example: Chest 13 (L.Bulb) → $7E0761 bit 5 ✓
        //   Example: Chest 14 (500 Gems) → $7E0761 bit 6 ✓
        //   Example: Chest 15 (Mushroom) → $7E0761 bit 7 ✓
        //
        // Chests with ID >= 128 (towers/dungeons):
        //   Base: $7E0770
        //   flag_id = chestId - 128
        //   Example: Chest 128 (Tower 1) → $7E0770 bit 0 ✓
        //   Example: Chest 132 (Sleepless Seal) → $7E0770 bit 4 ✓
        //   Example: Chest 137 (Crystal Thread) → $7E0771 bit 1 ✓
        //   Example: Chest 157 (STR Potion) → $7E0773 bit 5 ✓
        // ============================================
        public const uint CHEST_FLAGS_BASE_LOW = 0x0760;   // For chests < 128
        public const uint CHEST_FLAGS_BASE_HIGH = 0x0770;  // For chests >= 128
        
        /// <summary>
        /// Get the flag ID for a chest (VERIFIED)
        /// </summary>
        public static int GetChestFlagId(int chestId)
        {
            if (chestId >= 128)
            {
                return chestId - 128;
            }
            else
            {
                return chestId;
            }
        }
        
        /// <summary>
        /// Get the WRAM address for a chest's flag byte (VERIFIED)
        /// </summary>
        public static uint GetChestFlagAddress(int chestId)
        {
            int flagId = GetChestFlagId(chestId);
            uint baseAddr = (chestId >= 128) ? CHEST_FLAGS_BASE_HIGH : CHEST_FLAGS_BASE_LOW;
            return baseAddr + (uint)(flagId / 8);
        }
        
        /// <summary>
        /// Get the bit index within the flag byte (0-7)
        /// </summary>
        public static int GetChestFlagBit(int chestId)
        {
            int flagId = GetChestFlagId(chestId);
            return flagId % 8;
        }
        
        /// <summary>
        /// Get the SNI address for a chest's flag byte
        /// Chest flags are in LOW WRAM, so use SNI_WRAM_BASE_LOW
        /// </summary>
        public static uint GetChestFlagSNIAddress(int chestId)
        {
            return SNI_WRAM_BASE_LOW + GetChestFlagAddress(chestId);
        }
        
        /// <summary>
        /// Check if a chest is opened by reading its flag bit
        /// </summary>
        public static bool IsChestOpened(byte flagByte, int chestId)
        {
            int bit = GetChestFlagBit(chestId);
            return (flagByte & (1 << bit)) != 0;
        }
        
        // ============================================
        // Archipelago Communication
        // ============================================
        // These are custom addresses we'll use for AP communication
        // Using high WRAM area that's unlikely to be used by the game
        public const uint AP_RECEIVED_INDEX = 0x7F00;  // Index of last received item
        public const uint AP_ITEM_QUEUE = 0x7F02;      // Item queue for receiving
        
        /// <summary>
        /// Convert a WRAM offset to SNI address
        /// Automatically uses correct base depending on address range
        /// 
        /// BizHawk WRAM mapping (verified working):
        /// - Low WRAM ($7E0000-$7E7FFF): SNI $F50000 + offset
        /// - High WRAM ($7E8000+): SNI $F60000 + offset (NOT offset-0x8000!)
        /// 
        /// Example: $7E8000 → $F60000 + $8000 = $F68000 ✓
        /// Example: $7E0760 → $F50000 + $0760 = $F50760 ✓
        /// </summary>
        public static uint ToSNIAddress(uint wramOffset)
        {
            if (wramOffset >= 0x8000)
            {
                // High WRAM ($7E8000+) - inventory, stats, etc
                // Maps to $F60000 + offset (keep full offset!)
                return SNI_WRAM_BASE_HIGH + wramOffset;
            }
            else
            {
                // Low WRAM ($7E0000-$7E7FFF) - chest flags, game state, etc
                return SNI_WRAM_BASE_LOW + wramOffset;
            }
        }
        
        /// <summary>
        /// Convert SNI address to WRAM offset
        /// </summary>
        public static uint FromSNIAddress(uint sniAddress)
        {
            if (sniAddress >= SNI_WRAM_BASE_HIGH + 0x8000)
            {
                // High WRAM region: $F68000+ → $8000+
                return sniAddress - SNI_WRAM_BASE_HIGH;
            }
            else if (sniAddress >= SNI_WRAM_BASE_HIGH)
            {
                // This shouldn't happen with correct addresses, but handle it
                return sniAddress - SNI_WRAM_BASE_HIGH;
            }
            else
            {
                return sniAddress - SNI_WRAM_BASE_LOW;
            }
        }
    }
}