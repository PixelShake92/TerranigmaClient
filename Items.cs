using System.Collections.Generic;

namespace TerranigmaClient
{
    public enum ItemType
    {
        Consumable,     // $7E8000-$7E8035 (27 slots)
        KeyItem,        // $7E8036-$7E8047 (9 slots) - quest items, story items
        Weapon,         // $7E8048-$7E8067 (16 slots)
        Armor,          // $7E8068-$7E807F (12 slots) - Note: starts at 8068, not 8066!
        Ring,           // $7E8080-$7E8093 (10 slots)
        Gems,           // Special - adds to gem counter
        Special         // Unknown/unhandled
    }

    public class ItemInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public int Power { get; set; }
        public int Value { get; set; } // For gems
        
        public ItemInfo(int id, string name, ItemType type, int power = 0, int value = 0)
        {
            Id = id;
            Name = name;
            Type = type;
            Power = power;
            Value = value;
        }
    }

    public static class Items
    {
        // Progression key items that are required to beat the game
        public static readonly HashSet<string> ProgressionKeyItems = new HashSet<string>
        {
            // Chapter 1
            "Sleepless Seal",
            "Crystal Thread",
            "ElleCape",
            "Sharp Claws",
            
            // Chapter 2
            "Giant Leaves",
            "Ra Dewdrop",
            "RocSpear",
            "Snowgrass Leaf",
            
            // Chapter 3
            "Red Scarf",
            "Holy Seal",
            "Mushroom",
            "Protect Bell",
            "Dog Whistle",
            "Ruby",
            "Sapphire",
            "Black Opal",
            "Topaz",
            "Tower Key",
            "Speed Shoes",
            "Engagement Ring",
            "Magic Anchor",
            "Air Herb",
            "Sewer Key",
            "Transceiver",
            
            // Chapter 4
            "Starstone",
            "Time Bomb",
            
            // Other
            "Jail Key",
            "Ginseng"
        };

        // Item database - ALL non-consumable, non-equipment items go to KeyItem type
        public static readonly Dictionary<int, ItemInfo> Database = new Dictionary<int, ItemInfo>
        {
            // Rings (0x01-0x0F) - These go to Ring inventory
            { 0x01, new ItemInfo(0x01, "FireRing", ItemType.Ring, 1) },
            { 0x02, new ItemInfo(0x02, "PyroRing", ItemType.Ring, 2) },
            { 0x03, new ItemInfo(0x03, "IceRing", ItemType.Ring, 1) },
            { 0x04, new ItemInfo(0x04, "SnowRing", ItemType.Ring, 2) },
            { 0x05, new ItemInfo(0x05, "ZapRing", ItemType.Ring, 1) },
            { 0x06, new ItemInfo(0x06, "BoomRing", ItemType.Ring, 3) },
            { 0x07, new ItemInfo(0x07, "GeoRing", ItemType.Ring, 3) },
            { 0x08, new ItemInfo(0x08, "SkyRing", ItemType.Ring, 5) },
            { 0x09, new ItemInfo(0x09, "RayRing", ItemType.Ring, 5) },
            { 0x0A, new ItemInfo(0x0A, "ElecRing", ItemType.Ring, 5) },
            { 0x0B, new ItemInfo(0x0B, "GrassPin", ItemType.Ring, 6) },
            { 0x0C, new ItemInfo(0x0C, "WindPin", ItemType.Ring, 6) },
            { 0x0D, new ItemInfo(0x0D, "BonePin", ItemType.Ring, 6) },
            { 0x0E, new ItemInfo(0x0E, "HornPin", ItemType.Ring, 6) },
            { 0x0F, new ItemInfo(0x0F, "WaterPin", ItemType.Ring, 6) },
            
            // Consumables (0x10-0x1A) - These go to Consumable inventory
            { 0x10, new ItemInfo(0x10, "S.Bulb", ItemType.Consumable, 1) },
            { 0x11, new ItemInfo(0x11, "M.Bulb", ItemType.Consumable, 2) },
            { 0x12, new ItemInfo(0x12, "L.Bulb", ItemType.Consumable, 3) },
            { 0x13, new ItemInfo(0x13, "P. Cure", ItemType.Consumable, 2) },
            { 0x14, new ItemInfo(0x14, "Stardew", ItemType.Consumable, 3) },
            { 0x15, new ItemInfo(0x15, "Serum", ItemType.Consumable, 4) },
            { 0x16, new ItemInfo(0x16, "H.Water", ItemType.Consumable, 4) },
            { 0x17, new ItemInfo(0x17, "STR Potion", ItemType.Consumable, 5) },
            { 0x18, new ItemInfo(0x18, "DEF Potion", ItemType.Consumable, 5) },
            { 0x19, new ItemInfo(0x19, "Luck Potion", ItemType.Consumable, 5) },
            { 0x1A, new ItemInfo(0x1A, "Life Potion", ItemType.Consumable, 6) },
            
            // Key Items (0x30-0x7F) - ALL of these go to Key Item inventory
            { 0x32, new ItemInfo(0x32, "Crystal Thread", ItemType.KeyItem) },
            { 0x33, new ItemInfo(0x33, "Snowgrass Leaf", ItemType.KeyItem) },
            { 0x34, new ItemInfo(0x34, "Red Scarf", ItemType.KeyItem) },
            { 0x35, new ItemInfo(0x35, "Holy Seal", ItemType.KeyItem) },
            { 0x36, new ItemInfo(0x36, "Protect Bell", ItemType.KeyItem) },
            { 0x37, new ItemInfo(0x37, "Sapphire", ItemType.KeyItem) },
            { 0x38, new ItemInfo(0x38, "Black Opal", ItemType.KeyItem) },
            { 0x39, new ItemInfo(0x39, "Ruby", ItemType.KeyItem) },
            { 0x3A, new ItemInfo(0x3A, "Topaz", ItemType.KeyItem) },
            { 0x3B, new ItemInfo(0x3B, "Portrait", ItemType.KeyItem) },
            { 0x3C, new ItemInfo(0x3C, "Sleep Potion", ItemType.KeyItem) },
            { 0x3D, new ItemInfo(0x3D, "Jail Key", ItemType.KeyItem) },
            { 0x3E, new ItemInfo(0x3E, "Tower Key", ItemType.KeyItem) },
            { 0x3F, new ItemInfo(0x3F, "Ra Dewdrop", ItemType.KeyItem) },
            { 0x40, new ItemInfo(0x40, "Royal Letter", ItemType.KeyItem) },
            { 0x43, new ItemInfo(0x43, "Dog Whistle", ItemType.KeyItem) },
            { 0x47, new ItemInfo(0x47, "Transceiver", ItemType.KeyItem) },
            { 0x48, new ItemInfo(0x48, "Time Bomb", ItemType.KeyItem) },
            { 0x49, new ItemInfo(0x49, "Magic Anchor", ItemType.KeyItem) },
            { 0x4A, new ItemInfo(0x4A, "Engagement Ring", ItemType.KeyItem) },
            { 0x4B, new ItemInfo(0x4B, "Sewer Key", ItemType.KeyItem) },
            { 0x4C, new ItemInfo(0x4C, "Starstone", ItemType.KeyItem) },
            { 0x4E, new ItemInfo(0x4E, "Fancy Clothes", ItemType.KeyItem) },
            { 0x4F, new ItemInfo(0x4F, "Matis Painting", ItemType.KeyItem) },
            { 0x50, new ItemInfo(0x50, "Mushroom", ItemType.KeyItem) },
            { 0x51, new ItemInfo(0x51, "Wine", ItemType.KeyItem) },
            { 0x52, new ItemInfo(0x52, "Tasty Meat", ItemType.KeyItem) },
            { 0x53, new ItemInfo(0x53, "Camera", ItemType.KeyItem) },
            { 0x54, new ItemInfo(0x54, "Apartment Key", ItemType.KeyItem) },
            { 0x55, new ItemInfo(0x55, "Crystal", ItemType.KeyItem) },
            { 0x56, new ItemInfo(0x56, "Tinned Sardines", ItemType.KeyItem) },
            { 0x57, new ItemInfo(0x57, "Airfield Plans", ItemType.KeyItem) },
            { 0x58, new ItemInfo(0x58, "Ginseng", ItemType.KeyItem) },
            { 0x59, new ItemInfo(0x59, "Sleepless Seal", ItemType.KeyItem) },
            { 0x5A, new ItemInfo(0x5A, "Pretty Flower", ItemType.KeyItem) },
            { 0x5B, new ItemInfo(0x5B, "Fever Medicine", ItemType.KeyItem) },
            { 0x5C, new ItemInfo(0x5C, "Tin Sheet", ItemType.KeyItem) },
            { 0x5D, new ItemInfo(0x5D, "Nirlake Letter", ItemType.KeyItem) },
            { 0x5E, new ItemInfo(0x5E, "Log", ItemType.KeyItem) },
            
            // Special Key Items (ability slots) - 0x7B-0x7E have fixed positions
            { 0x7B, new ItemInfo(0x7B, "Speed Shoes", ItemType.KeyItem) },
            { 0x7C, new ItemInfo(0x7C, "Giant Leaves", ItemType.KeyItem) },
            { 0x7D, new ItemInfo(0x7D, "Sharp Claws", ItemType.KeyItem) },
            { 0x7E, new ItemInfo(0x7E, "Air Herb", ItemType.KeyItem) },
            
            // Weapons (0x80-0x9F) - These go to Weapon inventory
            { 0x80, new ItemInfo(0x80, "HexRod", ItemType.Weapon, 1) },
            { 0x81, new ItemInfo(0x81, "CrySpear", ItemType.Weapon, 2) },
            { 0x82, new ItemInfo(0x82, "RaSpear", ItemType.Weapon, 3) },
            { 0x83, new ItemInfo(0x83, "RocSpear", ItemType.Weapon, 3) },
            { 0x84, new ItemInfo(0x84, "Sticker", ItemType.Weapon, 4) },
            { 0x85, new ItemInfo(0x85, "Neo Fang", ItemType.Weapon, 4) },
            { 0x86, new ItemInfo(0x86, "Icepick", ItemType.Weapon, 5) },
            { 0x88, new ItemInfo(0x88, "BrnzPike", ItemType.Weapon, 5) },
            { 0x89, new ItemInfo(0x89, "LightRod", ItemType.Weapon, 6) },
            { 0x8B, new ItemInfo(0x8B, "SlverPike", ItemType.Weapon, 6) },
            { 0x8C, new ItemInfo(0x8C, "FirePike", ItemType.Weapon, 7) },
            { 0x8D, new ItemInfo(0x8D, "Trident", ItemType.Weapon, 7) },
            { 0x8E, new ItemInfo(0x8E, "SoulWand", ItemType.Weapon, 8) },
            { 0x8F, new ItemInfo(0x8F, "ThunPike", ItemType.Weapon, 8) },
            { 0x90, new ItemInfo(0x90, "SeaSpear", ItemType.Weapon, 9) },
            { 0x91, new ItemInfo(0x91, "GeoStaff", ItemType.Weapon, 9) },
            { 0x92, new ItemInfo(0x92, "DrgnPike", ItemType.Weapon, 10) },
            { 0x93, new ItemInfo(0x93, "3PartRod", ItemType.Weapon, 10) },
            { 0x94, new ItemInfo(0x94, "LghtPike", ItemType.Weapon, 11) },
            { 0x95, new ItemInfo(0x95, "Fauchard", ItemType.Weapon, 11) },
            { 0x96, new ItemInfo(0x96, "X-Spear", ItemType.Weapon, 12) },
            { 0x9C, new ItemInfo(0x9C, "HeroPike", ItemType.Weapon, 12) },
            { 0x9D, new ItemInfo(0x9D, "EnbuPike", ItemType.Weapon, 12) },
            { 0x9E, new ItemInfo(0x9E, "AlphaRod", ItemType.Weapon, 12) },
            { 0x9F, new ItemInfo(0x9F, "BlockRod", ItemType.Weapon, 12) },
            
            // Armor (0xA0-0xBF) - These go to Armor inventory
            { 0xA0, new ItemInfo(0xA0, "Clothes", ItemType.Armor, 1) },
            { 0xA1, new ItemInfo(0xA1, "Leather", ItemType.Armor, 2) },
            { 0xA2, new ItemInfo(0xA2, "LeafSuit", ItemType.Armor, 3) },
            { 0xA3, new ItemInfo(0xA3, "RaArmr", ItemType.Armor, 4) },
            { 0xA4, new ItemInfo(0xA4, "BirdSuit", ItemType.Armor, 5) },
            { 0xA5, new ItemInfo(0xA5, "FurCoat", ItemType.Armor, 6) },
            { 0xA6, new ItemInfo(0xA6, "Ice Suit", ItemType.Armor, 7) },
            { 0xA8, new ItemInfo(0xA8, "MonkRobe", ItemType.Armor, 8) },
            { 0xA9, new ItemInfo(0xA9, "NiceSuit", ItemType.Armor, 9) },
            { 0xAA, new ItemInfo(0xAA, "RingMail", ItemType.Armor, 10) },
            { 0xAB, new ItemInfo(0xAB, "SlverVest", ItemType.Armor, 11) },
            { 0xAC, new ItemInfo(0xAC, "VestArmr", ItemType.Armor, 11) },
            { 0xAE, new ItemInfo(0xAE, "SlvrArmr", ItemType.Armor, 12) },
            { 0xAF, new ItemInfo(0xAF, "PoshSuit", ItemType.Armor, 12) },
            { 0xB0, new ItemInfo(0xB0, "KungFuGi", ItemType.Armor, 12) },
            { 0xB1, new ItemInfo(0xB1, "DrgnMail", ItemType.Armor, 12) },
            { 0xB2, new ItemInfo(0xB2, "SoulArmr", ItemType.Armor, 12) },
            { 0xB3, new ItemInfo(0xB3, "HolySuit", ItemType.Armor, 12) },
            { 0xB4, new ItemInfo(0xB4, "KingArmr", ItemType.Armor, 12) },
            { 0xB5, new ItemInfo(0xB5, "RedArmr", ItemType.Armor, 12) },
            { 0xB6, new ItemInfo(0xB6, "Rags", ItemType.Armor, 12) },
            { 0xBC, new ItemInfo(0xBC, "HeroArmr", ItemType.Armor, 12) },
            { 0xBD, new ItemInfo(0xBD, "Pro Armr", ItemType.Armor, 12) },
            { 0xBE, new ItemInfo(0xBE, "Sea Mail", ItemType.Armor, 12) },
            { 0xBF, new ItemInfo(0xBF, "ElleCape", ItemType.Armor, 12) },
        };

        // Name to ID lookup
        public static readonly Dictionary<string, int> NameToId = new Dictionary<string, int>();
        
        static Items()
        {
            // Build reverse lookup
            foreach (var kvp in Database)
            {
                if (!NameToId.ContainsKey(kvp.Value.Name))
                {
                    NameToId[kvp.Value.Name] = kvp.Key;
                }
            }
        }

        public static string GetItemName(int itemId)
        {
            // Check for gem items (0x1xxx range)
            if (itemId >= 0x1000 && itemId < 0x2000)
            {
                int gemValue = GetGemValue(itemId);
                return $"{gemValue} Gems";
            }
            
            if (Database.TryGetValue(itemId, out var info))
            {
                return info.Name;
            }
            
            return $"Unknown Item (0x{itemId:X4})";
        }

        public static ItemInfo GetItemInfo(int itemId)
        {
            // Check for gem items (0x1xxx range)
            if (itemId >= 0x1000 && itemId < 0x2000)
            {
                int gemValue = GetGemValue(itemId);
                return new ItemInfo(itemId, $"{gemValue} Gems", ItemType.Gems, 0, gemValue);
            }
            
            if (Database.TryGetValue(itemId, out var info))
            {
                return info;
            }
            return new ItemInfo(itemId, "Unknown", ItemType.Special);
        }

        /// <summary>
        /// Extract gem value from a gem item ID
        /// Format: 0x1XYZ where XYZ encodes the gem amount
        /// </summary>
        public static int GetGemValue(int itemId)
        {
            // The gem value is encoded in the ID
            // 0x1030 = 30 gems, 0x1050 = 50 gems, 0x1100 = 100 gems, etc.
            int encoded = itemId & 0x0FFF;
            
            // Decode based on pattern:
            // 0x030 = 30, 0x050 = 50, 0x100 = 100, 0x200 = 200, 0x300 = 300, 0x500 = 500
            if (encoded <= 0x0FF)
            {
                // Values like 0x030 = 30, 0x044 = 44, 0x050 = 50
                return ((encoded >> 4) * 10) + (encoded & 0x0F);
            }
            else
            {
                // Values like 0x100 = 100, 0x200 = 200, 0x300 = 300, 0x500 = 500
                return ((encoded >> 8) * 100) + (((encoded >> 4) & 0x0F) * 10) + (encoded & 0x0F);
            }
        }

        public static int GetItemId(string name)
        {
            if (NameToId.TryGetValue(name, out var id))
            {
                return id;
            }
            return -1;
        }
    }
}
