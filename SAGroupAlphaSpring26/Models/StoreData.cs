using System.Collections.Generic;

namespace SAGroupAlphaSpring26.Models
{
    public static class StoreData
    {
        public static List<Piece> Pieces { get; } = new List<Piece>
        {
            // Enemies
            new Piece
            {
                Id = 1,
                Name = "Goblin",
                Description = "A sneaky goblin enemy.",
                Price = 10.00m,
                PieceTypeID = 1, // Enemy type
                SetID = 1,
                ImagePath = "/images/goblin.png"
            },
            new Piece
            {
                Id = 2,
                Name = "Skeleton",
                Description = "A spooky skeleton warrior.",
                Price = 12.50m,
                PieceTypeID = 1,
                SetID = 1,
                ImagePath = "/images/skeleton.png"
            },

            // Maps
            new Piece
            {
                Id = 3,
                Name = "Mystic Map",
                Description = "Reveals hidden treasures on your adventure.",
                Price = 25.00m,
                PieceTypeID = 2, // Map type
                SetID = 2,
                ImagePath = "/images/mystic-map.png"
            },

            // Characters
            new Piece
            {
                Id = 4,
                Name = "Hero Knight",
                Description = "A brave knight ready for battle.",
                Price = 50.00m,
                PieceTypeID = 3, // Character type
                SetID = 3,
                ImagePath = "/images/hero-knight.png"
            },

            // Shops/Chests/Interactables
            new Piece
            {
                Id = 5,
                Name = "Treasure Chest",
                Description = "Contains random loot for adventurers.",
                Price = 15.00m,
                PieceTypeID = 4, // Interactable type
                SetID = 4,
                ImagePath = "/images/treasure-chest.png"
            },
            new Piece
            {
                Id = 6,
                Name = "Magic Shop",
                Description = "Purchase magical items and potions.",
                Price = 100.00m,
                PieceTypeID = 4,
                SetID = 4,
                ImagePath = "/images/magic-shop.png"
            }
        };
    }
}
