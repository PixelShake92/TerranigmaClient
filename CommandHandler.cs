using System;
using System.Linq;
using TerranigmaClient;

namespace Helpers
{
    /// <summary>
    /// Handles custom client commands like !getitem
    /// </summary>
    public static class CommandHandler
    {
        /// <summary>
        /// Try to handle a custom command. Returns true if command was handled.
        /// </summary>
        public static bool TryHandleCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            
            string trimmed = input.Trim();
            
            // Handle commands with ! prefix or without
            if (trimmed.StartsWith("!"))
                trimmed = trimmed.Substring(1);
            
            string[] parts = trimmed.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return false;
            
            string command = parts[0].ToLowerInvariant();
            string args = parts.Length > 1 ? parts[1] : "";
            
            switch (command)
            {
                case "getitem":
                    HandleGetItem(args);
                    return true;
                    
                case "items":
                case "listitems":
                    HandleListItems(args);
                    return true;
                    
                case "givegems":
                    HandleGiveGems(args);
                    return true;
                    
                case "help":
                    // Only handle if it's asking about our commands
                    if (string.IsNullOrEmpty(args) || args.ToLower() == "getitem")
                    {
                        PrintCustomHelp();
                        return true;
                    }
                    return false;
                    
                default:
                    return false;
            }
        }
        
        private static void HandleGetItem(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                Console.WriteLine("Usage: !getitem <item name>");
                Console.WriteLine("Example: !getitem S.Bulb");
                Console.WriteLine("Example: !getitem Sharp Claws");
                Console.WriteLine("Example: !getitem 100 Gems");
                Console.WriteLine("Use !items to list all available items.");
                return;
            }
            
            var itemInfo = Items.GetItemByName(itemName);
            
            if (itemInfo == null)
            {
                Console.WriteLine($"Unknown item: '{itemName}'");
                Console.WriteLine("Use !items to list all available items.");
                
                // Try to suggest similar items
                var allItems = Items.GetAllItemNames();
                var suggestions = allItems
                    .Where(n => n.ToLowerInvariant().Contains(itemName.ToLowerInvariant().Substring(0, Math.Min(3, itemName.Length))))
                    .Take(5)
                    .ToList();
                
                if (suggestions.Count > 0)
                {
                    Console.WriteLine("Did you mean: " + string.Join(", ", suggestions) + "?");
                }
                return;
            }
            
            Console.WriteLine($"Giving item: {itemInfo.Name} (ID: 0x{itemInfo.Id:X2}, Type: {itemInfo.Type})");
            PlayerStateHelpers.GiveItem(itemInfo.Id, itemInfo.Name);
        }
        
        private static void HandleGiveGems(string amountStr)
        {
            if (string.IsNullOrWhiteSpace(amountStr) || !int.TryParse(amountStr, out int amount) || amount <= 0)
            {
                Console.WriteLine("Usage: !givegems <amount>");
                Console.WriteLine("Example: !givegems 500");
                return;
            }
            
            int gemItemId = Items.GetGemItemId(amount);
            Console.WriteLine($"Giving {amount} gems (ID: 0x{gemItemId:X4})");
            PlayerStateHelpers.GiveItem(gemItemId, $"{amount} Gems");
        }
        
        private static void HandleListItems(string filter)
        {
            var allItems = Items.GetAllItemNames();
            
            if (!string.IsNullOrWhiteSpace(filter))
            {
                allItems = allItems
                    .Where(n => n.ToLowerInvariant().Contains(filter.ToLowerInvariant()))
                    .ToList();
            }
            
            Console.WriteLine($"Available items ({allItems.Count}):");
            
            // Group by type for better readability
            Console.WriteLine("\n--- Consumables ---");
            foreach (var name in allItems.Where(n => {
                var info = Items.GetItemByName(n);
                return info != null && info.Type == ItemType.Consumable;
            }))
            {
                Console.WriteLine($"  {name}");
            }
            
            Console.WriteLine("\n--- Key Items ---");
            foreach (var name in allItems.Where(n => {
                var info = Items.GetItemByName(n);
                return info != null && info.Type == ItemType.KeyItem;
            }))
            {
                Console.WriteLine($"  {name}");
            }
            
            Console.WriteLine("\n--- Weapons ---");
            foreach (var name in allItems.Where(n => {
                var info = Items.GetItemByName(n);
                return info != null && info.Type == ItemType.Weapon;
            }))
            {
                Console.WriteLine($"  {name}");
            }
            
            Console.WriteLine("\n--- Armor ---");
            foreach (var name in allItems.Where(n => {
                var info = Items.GetItemByName(n);
                return info != null && info.Type == ItemType.Armor;
            }))
            {
                Console.WriteLine($"  {name}");
            }
            
            Console.WriteLine("\n--- Rings ---");
            foreach (var name in allItems.Where(n => {
                var info = Items.GetItemByName(n);
                return info != null && info.Type == ItemType.Ring;
            }))
            {
                Console.WriteLine($"  {name}");
            }
            
            Console.WriteLine("\n--- Gems ---");
            foreach (var name in allItems.Where(n => n.EndsWith("Gems")))
            {
                Console.WriteLine($"  {name}");
            }
        }
        
        private static void PrintCustomHelp()
        {
            Console.WriteLine("\n=== Terranigma Client Commands ===");
            Console.WriteLine("  !getitem <name>  - Give yourself an item");
            Console.WriteLine("  !givegems <amt>  - Give yourself gems");
            Console.WriteLine("  !items [filter]  - List all available items");
            Console.WriteLine("  !help            - Show this help");
            Console.WriteLine("\nExamples:");
            Console.WriteLine("  !getitem S.Bulb");
            Console.WriteLine("  !getitem Sharp Claws");
            Console.WriteLine("  !getitem 100 Gems");
            Console.WriteLine("  !givegems 999");
            Console.WriteLine("  !items weapon");
            Console.WriteLine("");
        }
    }
}
