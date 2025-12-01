namespace TerranigmaClient
{
    /// <summary>
    /// Location data for all chest locations in Terranigma
    /// </summary>
    public class TerranigmaLocation
    {
        public int ChestId { get; set; }
        public int RomAddress { get; set; }
        public int OriginalItemId { get; set; }
        public string OriginalItemName { get; set; } = "";
        public string MapName { get; set; } = "";
    }

    public static class Locations
    {
        // Base ID for Terranigma locations in Archipelago
        // This should match what's defined in the APWorld
        public const long AP_BASE_ID = 0x54450000; // "TE" in hex + offset
        
        // Portrait chest - must remain in vanilla location
        public const int PORTRAIT_CHEST_ID = 150;
        public const int PORTRAIT_ITEM_ID = 0x003B;

        /// <summary>
        /// All 126 chest locations in Terranigma
        /// </summary>
        public static readonly List<TerranigmaLocation> AllLocations = new List<TerranigmaLocation>
        {
            // === Chests with ID < 128 (Surface world, etc.) ===
            new TerranigmaLocation { ChestId = 6, RomAddress = 0x19e11a, OriginalItemId = 0x0001, OriginalItemName = "FireRing", MapName = "Louran" },
            new TerranigmaLocation { ChestId = 10, RomAddress = 0x19e132, OriginalItemId = 0x10, OriginalItemName = "S.Bulb", MapName = "Safarium" },
            new TerranigmaLocation { ChestId = 11, RomAddress = 0x19e13a, OriginalItemId = 0x0036, OriginalItemName = "Protect Bell", MapName = "Loire Castle - Tower - 3rd Floor" },
            new TerranigmaLocation { ChestId = 12, RomAddress = 0x19e44f, OriginalItemId = 0x005C, OriginalItemName = "Tin Sheet", MapName = "Nirlake - House" },
            new TerranigmaLocation { ChestId = 13, RomAddress = 0x19e429, OriginalItemId = 0x12, OriginalItemName = "L.Bulb", MapName = "Mush (Near Loire)" },
            new TerranigmaLocation { ChestId = 14, RomAddress = 0x19e430, OriginalItemId = 0x8500, OriginalItemName = "500 Gems", MapName = "Mush (Near Loire)" },
            new TerranigmaLocation { ChestId = 15, RomAddress = 0x19e437, OriginalItemId = 0x0050, OriginalItemName = "Mushroom", MapName = "Mush (Near Loire)" },
            new TerranigmaLocation { ChestId = 16, RomAddress = 0x19e43f, OriginalItemId = 0x19, OriginalItemName = "Luck Potion", MapName = "Litz" },
            new TerranigmaLocation { ChestId = 17, RomAddress = 0x19e447, OriginalItemId = 0x1a, OriginalItemName = "Life Potion", MapName = "Nirlake - House" },
            new TerranigmaLocation { ChestId = 18, RomAddress = 0x19e457, OriginalItemId = 0x12, OriginalItemName = "L.Bulb", MapName = "Litz - Ship - Storage" },
            new TerranigmaLocation { ChestId = 19, RomAddress = 0x19e12a, OriginalItemId = 0x14, OriginalItemName = "Stardew", MapName = "Lumina" },
            
            // === Tower chests (ID 128-141) ===
            new TerranigmaLocation { ChestId = 128, RomAddress = 0x19e142, OriginalItemId = 0x10, OriginalItemName = "S.Bulb", MapName = "Tower 1 - 3rd Floor" },
            new TerranigmaLocation { ChestId = 129, RomAddress = 0x19e14a, OriginalItemId = 0x8030, OriginalItemName = "30 Gems", MapName = "Tower 2 - 1st Floor" },
            new TerranigmaLocation { ChestId = 130, RomAddress = 0x19e152, OriginalItemId = 0x10, OriginalItemName = "S.Bulb", MapName = "Tower 2 - 2nd Floor" },
            new TerranigmaLocation { ChestId = 131, RomAddress = 0x19e15a, OriginalItemId = 0x10, OriginalItemName = "S.Bulb", MapName = "Tower 3- 1st Floor" },
            new TerranigmaLocation { ChestId = 132, RomAddress = 0x19e162, OriginalItemId = 0x0059, OriginalItemName = "Sleepless Seal", MapName = "Tower 3 - 4th Floor" },
            new TerranigmaLocation { ChestId = 134, RomAddress = 0x19e16a, OriginalItemId = 0x8044, OriginalItemName = "44 Gems", MapName = "Tower 4 - -1st Floor" },
            new TerranigmaLocation { ChestId = 135, RomAddress = 0x19e172, OriginalItemId = 0x11, OriginalItemName = "M.Bulb", MapName = "Tower 4 - 2nd Floor" },
            new TerranigmaLocation { ChestId = 136, RomAddress = 0x19e179, OriginalItemId = 0x1a, OriginalItemName = "Life Potion", MapName = "Tower 4 - 2nd Floor" },
            new TerranigmaLocation { ChestId = 137, RomAddress = 0x19e181, OriginalItemId = 0x0032, OriginalItemName = "Crystal Thread", MapName = "Tower 4 - 3rd Floor" },
            new TerranigmaLocation { ChestId = 138, RomAddress = 0x19e191, OriginalItemId = 0x82, OriginalItemName = "Ra Spear", MapName = "Tree Cave" },
            new TerranigmaLocation { ChestId = 139, RomAddress = 0x19e1bc, OriginalItemId = 0x11, OriginalItemName = "M.Bulb", MapName = "Tree Cave" },
            new TerranigmaLocation { ChestId = 140, RomAddress = 0x19e1f7, OriginalItemId = 0x007C, OriginalItemName = "Giant Leaves", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 141, RomAddress = 0x19e1ff, OriginalItemId = 0xa2, OriginalItemName = "LeafSuit", MapName = "Tree Cave" },
            
            // === Grecliff / Louran / Sylvain (ID 142-155) ===
            new TerranigmaLocation { ChestId = 142, RomAddress = 0x19e237, OriginalItemId = 0x0083, OriginalItemName = "RocSpear", MapName = "Grecliff - Cave" },
            new TerranigmaLocation { ChestId = 143, RomAddress = 0x19e130, OriginalItemId = 0x0034, OriginalItemName = "Red Scarf", MapName = "Louran-Meilins House" },
            new TerranigmaLocation { ChestId = 144, RomAddress = 0x19e301, OriginalItemId = 0x11, OriginalItemName = "M.Bulb", MapName = "Louran - House" },
            new TerranigmaLocation { ChestId = 145, RomAddress = 0x19e309, OriginalItemId = 0x0035, OriginalItemName = "Holy Seal", MapName = "Louran - House" },
            new TerranigmaLocation { ChestId = 146, RomAddress = 0x19e328, OriginalItemId = 0x89, OriginalItemName = "LightRod", MapName = "Louran - House" },
            new TerranigmaLocation { ChestId = 147, RomAddress = 0x19e358, OriginalItemId = 0x003E, OriginalItemName = "Tower Key", MapName = "Sylvain Castle" },
            new TerranigmaLocation { ChestId = 148, RomAddress = 0x19e35f, OriginalItemId = 0x86, OriginalItemName = "Icepick", MapName = "Sylvain Castle" },
            new TerranigmaLocation { ChestId = 149, RomAddress = 0x19e367, OriginalItemId = 0x0039, OriginalItemName = "Ruby", MapName = "Sylvian Castle" },
            new TerranigmaLocation { ChestId = 150, RomAddress = 0x19e386, OriginalItemId = 0x003B, OriginalItemName = "Portrait", MapName = "Stockholm - House" }, // NOT RANDOMIZED
            new TerranigmaLocation { ChestId = 151, RomAddress = 0x19e38d, OriginalItemId = 0x11, OriginalItemName = "M.Bulb", MapName = "Stockholm - House" },
            new TerranigmaLocation { ChestId = 152, RomAddress = 0x19e3b5, OriginalItemId = 0x0049, OriginalItemName = "Magic Anchor", MapName = "Great Lakes Cavern" },
            new TerranigmaLocation { ChestId = 153, RomAddress = 0x19e3d4, OriginalItemId = 0x004B, OriginalItemName = "Sewer Key", MapName = "Sewer" },
            new TerranigmaLocation { ChestId = 154, RomAddress = 0x19e198, OriginalItemId = 0x10, OriginalItemName = "S.Bulb", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 155, RomAddress = 0x19e1a6, OriginalItemId = 0x13, OriginalItemName = "P. Cure", MapName = "Tree cave" },
            
            // === More dungeons (ID 156-179) ===
            new TerranigmaLocation { ChestId = 156, RomAddress = 0x19e1ad, OriginalItemId = 0x11, OriginalItemName = "M.Bulb", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 157, RomAddress = 0x19e227, OriginalItemId = 0x17, OriginalItemName = "STR Potion", MapName = "Grecliff - Cave" },
            new TerranigmaLocation { ChestId = 158, RomAddress = 0x19e22f, OriginalItemId = 0x1a, OriginalItemName = "Life Potion", MapName = "Grecliff - Cave" },
            new TerranigmaLocation { ChestId = 159, RomAddress = 0x19e23f, OriginalItemId = 0x19, OriginalItemName = "Luck Potion", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 160, RomAddress = 0x19e207, OriginalItemId = 0x11, OriginalItemName = "M.Bulb", MapName = "Grecliff" },
            new TerranigmaLocation { ChestId = 161, RomAddress = 0x19e20f, OriginalItemId = 0x0010, OriginalItemName = "S.Bulb", MapName = "Grecliff" },
            new TerranigmaLocation { ChestId = 162, RomAddress = 0x19e217, OriginalItemId = 0x8087, OriginalItemName = "87 Gems", MapName = "Grecliff" },
            new TerranigmaLocation { ChestId = 163, RomAddress = 0x19e21f, OriginalItemId = 0x11, OriginalItemName = "M.Bulb", MapName = "Grecliff" },
            new TerranigmaLocation { ChestId = 164, RomAddress = 0x19e1e8, OriginalItemId = 0x11, OriginalItemName = "M.Bulb", MapName = "Tree Cave" },
            new TerranigmaLocation { ChestId = 165, RomAddress = 0x19e26d, OriginalItemId = 0x13, OriginalItemName = "P. Cure", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 166, RomAddress = 0x19e275, OriginalItemId = 0x8065, OriginalItemName = "65 Gems", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 167, RomAddress = 0x19e27c, OriginalItemId = 0x1a, OriginalItemName = "Life Potion", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 168, RomAddress = 0x19e283, OriginalItemId = 0x10, OriginalItemName = "S.Bulb", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 169, RomAddress = 0x19e28b, OriginalItemId = 0x14, OriginalItemName = "Stardew", MapName = "Eklemata" },
            new TerranigmaLocation { ChestId = 170, RomAddress = 0x19e293, OriginalItemId = 0x1a, OriginalItemName = "Life Potion", MapName = "Eklemata" },
            new TerranigmaLocation { ChestId = 171, RomAddress = 0x19e29b, OriginalItemId = 0x8099, OriginalItemName = "99 Gems", MapName = "Eklemata" },
            new TerranigmaLocation { ChestId = 172, RomAddress = 0x19e1ef, OriginalItemId = 0x0011, OriginalItemName = "M.Bulb", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 173, RomAddress = 0x19e2a2, OriginalItemId = 0x0014, OriginalItemName = "Stardew", MapName = "Eklemata" },
            new TerranigmaLocation { ChestId = 174, RomAddress = 0x19e2aa, OriginalItemId = 0x00A6, OriginalItemName = "Ice Suit", MapName = "Eklemata" },
            new TerranigmaLocation { ChestId = 175, RomAddress = 0x19e2b2, OriginalItemId = 0x0019, OriginalItemName = "Luck Potion", MapName = "Eklemata (Unused)" },
            new TerranigmaLocation { ChestId = 176, RomAddress = 0x19e2ba, OriginalItemId = 0x008C, OriginalItemName = "FirePike", MapName = "Eklemata" },
            new TerranigmaLocation { ChestId = 177, RomAddress = 0x19e2c2, OriginalItemId = 0x8100, OriginalItemName = "100 Gems", MapName = "Eklemata" },
            new TerranigmaLocation { ChestId = 178, RomAddress = 0x19e1e0, OriginalItemId = 0x0013, OriginalItemName = "P. Cure", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 179, RomAddress = 0x19e2c9, OriginalItemId = 0x0014, OriginalItemName = "Stardew", MapName = "Eklemata" },
            
            // === Norfest / Sylvain / Louran extended (ID 180-191) ===
            new TerranigmaLocation { ChestId = 180, RomAddress = 0x19e2d1, OriginalItemId = 0x00AA, OriginalItemName = "RingMail", MapName = "Norfest" },
            new TerranigmaLocation { ChestId = 181, RomAddress = 0x19e340, OriginalItemId = 0x0014, OriginalItemName = "Stardew", MapName = "Sylvian Castle" },
            new TerranigmaLocation { ChestId = 182, RomAddress = 0x19e348, OriginalItemId = 0x0017, OriginalItemName = "STR Potion", MapName = "Sylvian Castle" },
            new TerranigmaLocation { ChestId = 183, RomAddress = 0x19e36e, OriginalItemId = 0x0012, OriginalItemName = "L.Bulb", MapName = "Sylvian Castle" },
            new TerranigmaLocation { ChestId = 184, RomAddress = 0x19e350, OriginalItemId = 0x0018, OriginalItemName = "DEF Potion", MapName = "Sylvian Castle" },
            new TerranigmaLocation { ChestId = 185, RomAddress = 0x19e376, OriginalItemId = 0x8651, OriginalItemName = "651 Gems", MapName = "Sylvian Castle" },
            new TerranigmaLocation { ChestId = 187, RomAddress = 0x19e319, OriginalItemId = 0x00B6, OriginalItemName = "Rags", MapName = "Louran - Storage" },
            new TerranigmaLocation { ChestId = 188, RomAddress = 0x19e320, OriginalItemId = 0x0013, OriginalItemName = "P. Cure", MapName = "Louran - Storage" },
            new TerranigmaLocation { ChestId = 190, RomAddress = 0x19e330, OriginalItemId = 0x0012, OriginalItemName = "L.Bulb", MapName = "Louran - House" },
            new TerranigmaLocation { ChestId = 191, RomAddress = 0x19e338, OriginalItemId = 0x8178, OriginalItemName = "178 Gems", MapName = "Louran - north side - room" },
            
            // === Lab / Dragoon / Astarica / Sewer (ID 192-209) ===
            new TerranigmaLocation { ChestId = 192, RomAddress = 0x19e39d, OriginalItemId = 0x00B2, OriginalItemName = "SoulArmr", MapName = "Lab - 1F" },
            new TerranigmaLocation { ChestId = 193, RomAddress = 0x19e45f, OriginalItemId = 0x8200, OriginalItemName = "200 Gems", MapName = "Dragoon Castle - 1st Floor - Room 1" },
            new TerranigmaLocation { ChestId = 194, RomAddress = 0x19e467, OriginalItemId = 0x8300, OriginalItemName = "300 Gems", MapName = "Dragoon Castle - -1st Floor - Room 2" },
            new TerranigmaLocation { ChestId = 195, RomAddress = 0x19e46e, OriginalItemId = 0x0012, OriginalItemName = "L.Bulb", MapName = "Dragoon Castle - -1st Floor - Room 2" },
            new TerranigmaLocation { ChestId = 196, RomAddress = 0x19e476, OriginalItemId = 0x0093, OriginalItemName = "PartRod", MapName = "Dragoon Castle - -1st Floor - Room 3 - A" },
            new TerranigmaLocation { ChestId = 197, RomAddress = 0x19e403, OriginalItemId = 0x00B3, OriginalItemName = "HolySuit", MapName = "Astarica - backroom" },
            new TerranigmaLocation { ChestId = 198, RomAddress = 0x19e3dc, OriginalItemId = 0x0016, OriginalItemName = "H.Water", MapName = "Sewer" },
            new TerranigmaLocation { ChestId = 199, RomAddress = 0x19e3e4, OriginalItemId = 0x0095, OriginalItemName = "Fauchard", MapName = "Sewer" },
            new TerranigmaLocation { ChestId = 200, RomAddress = 0x19e3eb, OriginalItemId = 0x0019, OriginalItemName = "Luck Potion", MapName = "Sewer" },
            new TerranigmaLocation { ChestId = 201, RomAddress = 0x19e2d9, OriginalItemId = 0x001A, OriginalItemName = "Life Potion", MapName = "Norfest" },
            new TerranigmaLocation { ChestId = 202, RomAddress = 0x19e2e1, OriginalItemId = 0x8389, OriginalItemName = "389 Gems", MapName = "Norfest" },
            new TerranigmaLocation { ChestId = 203, RomAddress = 0x19e2e9, OriginalItemId = 0x0043, OriginalItemName = "Dog Whistle", MapName = "Norfest" },
            new TerranigmaLocation { ChestId = 204, RomAddress = 0x19e2f1, OriginalItemId = 0x0011, OriginalItemName = "M.Bulb", MapName = "Norfest" },
            new TerranigmaLocation { ChestId = 205, RomAddress = 0x19e3bc, OriginalItemId = 0x8753, OriginalItemName = "753 Gems", MapName = "Great Lakes Cavern" },
            new TerranigmaLocation { ChestId = 206, RomAddress = 0x19e3ad, OriginalItemId = 0x007E, OriginalItemName = "Air Herb", MapName = "Great Lakes Cavern" },
            new TerranigmaLocation { ChestId = 207, RomAddress = 0x19e3c4, OriginalItemId = 0x0091, OriginalItemName = "GeoStaff", MapName = "Great Lakes Cavern" },
            new TerranigmaLocation { ChestId = 208, RomAddress = 0x19e3fb, OriginalItemId = 0x001A, OriginalItemName = "Life Potion", MapName = "Labtower" },
            new TerranigmaLocation { ChestId = 209, RomAddress = 0x19e3f3, OriginalItemId = 0x00B4, OriginalItemName = "KingArmr", MapName = "Sewer" },
            
            // === Hidden areas / Tree cave extended (ID 210-229) ===
            new TerranigmaLocation { ChestId = 210, RomAddress = 0x19e40b, OriginalItemId = 0x007B, OriginalItemName = "Speed Shoes", MapName = "Hidden area" },
            new TerranigmaLocation { ChestId = 211, RomAddress = 0x19e1c3, OriginalItemId = 0x0013, OriginalItemName = "P. Cure", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 212, RomAddress = 0x19e1ca, OriginalItemId = 0x001A, OriginalItemName = "Life Potion", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 213, RomAddress = 0x19e19f, OriginalItemId = 0x13, OriginalItemName = "P. Cure", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 214, RomAddress = 0x19e1b4, OriginalItemId = 0x10, OriginalItemName = "S.Bulb", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 215, RomAddress = 0x19e256, OriginalItemId = 0x0018, OriginalItemName = "DEF Potion", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 216, RomAddress = 0x19e247, OriginalItemId = 0x0013, OriginalItemName = "P. Cure", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 217, RomAddress = 0x19e24e, OriginalItemId = 0x0011, OriginalItemName = "M.Bulb", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 218, RomAddress = 0x19e25e, OriginalItemId = 0x0084, OriginalItemName = "Sticker", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 219, RomAddress = 0x19e3cc, OriginalItemId = 0x00B1, OriginalItemName = "DrgnMail", MapName = "Great Lakes Cavern" },
            new TerranigmaLocation { ChestId = 220, RomAddress = 0x19e47e, OriginalItemId = 0x001A, OriginalItemName = "Life Potion", MapName = "Hidden area" },
            new TerranigmaLocation { ChestId = 221, RomAddress = 0x19e485, OriginalItemId = 0x8378, OriginalItemName = "378 Gems", MapName = "Hidden area" },
            new TerranigmaLocation { ChestId = 222, RomAddress = 0x19e48d, OriginalItemId = 0x0011, OriginalItemName = "M.Bulb", MapName = "Hidden area" },
            new TerranigmaLocation { ChestId = 223, RomAddress = 0x19e494, OriginalItemId = 0x8378, OriginalItemName = "378 Gems", MapName = "Hidden area" },
            new TerranigmaLocation { ChestId = 224, RomAddress = 0x19e49c, OriginalItemId = 0x0017, OriginalItemName = "STR Potion", MapName = "Hidden area" },
            new TerranigmaLocation { ChestId = 225, RomAddress = 0x19e4a4, OriginalItemId = 0x001A, OriginalItemName = "Life Potion", MapName = "Hidden area near Odemrock" },
            new TerranigmaLocation { ChestId = 226, RomAddress = 0x19e4ab, OriginalItemId = 0x8228, OriginalItemName = "228 Gems", MapName = "Hidden area near Odemrock" },
            new TerranigmaLocation { ChestId = 227, RomAddress = 0x19e412, OriginalItemId = 0x8378, OriginalItemName = "378 Gems", MapName = "Hidden area" },
            new TerranigmaLocation { ChestId = 228, RomAddress = 0x19e4b3, OriginalItemId = 0x0019, OriginalItemName = "Luck Potion", MapName = "Hidden area" },
            new TerranigmaLocation { ChestId = 229, RomAddress = 0x19e4ba, OriginalItemId = 0x9403, OriginalItemName = "1403 Gems", MapName = "Hidden area" },
            
            // === More hidden areas / Sahara (ID 231-241) ===
            new TerranigmaLocation { ChestId = 231, RomAddress = 0x19e4c2, OriginalItemId = 0x009F, OriginalItemName = "BlockRod", MapName = "Hidden area - near tower" },
            new TerranigmaLocation { ChestId = 232, RomAddress = 0x19e4ca, OriginalItemId = 0x8703, OriginalItemName = "703 Gems", MapName = "Hidden area - Sahara" },
            new TerranigmaLocation { ChestId = 233, RomAddress = 0x19e4d1, OriginalItemId = 0x9003, OriginalItemName = "1003 Gems", MapName = "Hidden area - Sahara" },
            new TerranigmaLocation { ChestId = 234, RomAddress = 0x19e4d9, OriginalItemId = 0x0018, OriginalItemName = "DEF Potion", MapName = "Mu" },
            new TerranigmaLocation { ChestId = 235, RomAddress = 0x19e4e0, OriginalItemId = 0x009D, OriginalItemName = "EnbuPike", MapName = "Mu" },
            new TerranigmaLocation { ChestId = 236, RomAddress = 0x19e1d2, OriginalItemId = 0x0010, OriginalItemName = "S.Bulb", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 237, RomAddress = 0x19e1d9, OriginalItemId = 0x8042, OriginalItemName = "42 Gems", MapName = "Tree cave" },
            new TerranigmaLocation { ChestId = 238, RomAddress = 0x19e4e8, OriginalItemId = 0x8961, OriginalItemName = "961 Gems", MapName = "Hidden area" },
            new TerranigmaLocation { ChestId = 239, RomAddress = 0x19e4f0, OriginalItemId = 0x001A, OriginalItemName = "Life Potion", MapName = "Hidden area - Part 2" },
            new TerranigmaLocation { ChestId = 240, RomAddress = 0x19e4f7, OriginalItemId = 0x00BE, OriginalItemName = "Sea Mail", MapName = "Hidden area - Part 2" },
            new TerranigmaLocation { ChestId = 241, RomAddress = 0x19e4ff, OriginalItemId = 0x8892, OriginalItemName = "892 Gems", MapName = "Hidden area near Odemrock (Zoe like)" },
            
            // === Special chests (ID 250-255) ===
            new TerranigmaLocation { ChestId = 250, RomAddress = 0x19e265, OriginalItemId = 0x0011, OriginalItemName = "M.Bulb", MapName = "Zue" },
            new TerranigmaLocation { ChestId = 251, RomAddress = 0x19e311, OriginalItemId = 0x0017, OriginalItemName = "STR Potion", MapName = "Louran north side (Zombies)" },
            new TerranigmaLocation { ChestId = 252, RomAddress = 0x19e3a5, OriginalItemId = 0x0018, OriginalItemName = "DEF Potion", MapName = "Lab - 1F" },
            new TerranigmaLocation { ChestId = 254, RomAddress = 0x19e189, OriginalItemId = 0x004C, OriginalItemName = "Starstone", MapName = "Astarika" },
            new TerranigmaLocation { ChestId = 255, RomAddress = 0x19e395, OriginalItemId = 0x0090, OriginalItemName = "SeaSpear", MapName = "Mermaid Tower - 1st sub floor" },
        };

        // Lookup by chest ID
        private static readonly Dictionary<int, TerranigmaLocation> _locationByChestId;
        
        static Locations()
        {
            _locationByChestId = AllLocations.ToDictionary(loc => loc.ChestId);
        }
        
        public static TerranigmaLocation? GetByChestId(int chestId)
        {
            return _locationByChestId.TryGetValue(chestId, out var loc) ? loc : null;
        }
        
        /// <summary>
        /// Convert chest ID to Archipelago location ID
        /// </summary>
        public static long ToAPLocationId(int chestId)
        {
            return AP_BASE_ID + chestId;
        }
        
        /// <summary>
        /// Convert Archipelago location ID to chest ID
        /// </summary>
        public static int FromAPLocationId(long apLocationId)
        {
            return (int)(apLocationId - AP_BASE_ID);
        }
        
        /// <summary>
        /// Build a list of ILocation objects for Archipelago
        /// This version takes no arguments for compatibility
        /// </summary>
        public static List<Archipelago.Core.Models.ILocation> BuildLocationList()
        {
            var locations = new List<Archipelago.Core.Models.ILocation>();
            
            foreach (var loc in AllLocations)
            {
                // Skip portrait chest - it's not randomized
                if (loc.ChestId == PORTRAIT_CHEST_ID)
                    continue;
                    
                locations.Add(new Archipelago.Core.Models.Location
                {
                    Id = (int)ToAPLocationId(loc.ChestId),
                    Name = $"{loc.MapName} - {loc.OriginalItemName}",
                    Address = (uint)loc.RomAddress,
                    AddressBit = 0
                });
            }
            
            return locations;
        }
        
        /// <summary>
        /// Build a list of ILocation objects for Archipelago
        /// This version takes options parameter for compatibility with LocationHelpers
        /// </summary>
        public static List<Archipelago.Core.Models.ILocation> BuildLocationList(object? options)
        {
            // Just call the no-args version - options not used yet
            return BuildLocationList();
        }
    }
}
