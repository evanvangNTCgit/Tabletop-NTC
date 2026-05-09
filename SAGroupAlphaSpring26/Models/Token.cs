using System.ComponentModel.DataAnnotations.Schema;

namespace SAGroupAlphaSpring26.Models
{
    public class Token
    {
        // Id of the token.
        public int Id { get; set; }

        // Name of token.
        // I am thinking that this name can be overwritten by the DM if they wish to (Like naming a skeleton to 'Bob')
        // However if left null or blank just default to the piece name.
        // So if the piece is a goblin, and the DM doesn't give it a name, it will just be called "Goblin" on the map.
        public string Name { get; set; } = string.Empty;

        // The ID of the piece this token is based on.
        [Required]
        public int PieceID { get; set; }

        // Navigation property to the piece this token is based on.
        public Piece? Piece { get; set; }

        // The current session this token is in.
        [Required]
        public int SessionId { get; set; }

        // The navigation property to the session this token is in.
        public Session? Session { get; set; }

        // The scene this token belongs to, previously tokens only belonged to sessions.
        public int? SceneId { get; set; }

        public Scene? Scene { get; set; }

        // Current X coordinate of the token on the map, defaults to 0.00.
        [Column(TypeName = "decimal(18,2)")]
        public double X { get; set; } = 0.00;

        // Current Y coordinate of the token on the map, defaults to 0.00.
        [Column(TypeName = "decimal(18,2)")]
        public double Y { get; set; } = 0.00;

        // The layer that the token is on. Defaults to 0, so bottom layer.
        [Column(TypeName = "decimal(18,2)")]
        public int ZIndex { get; set; } = 0;

        // Is this token visible or not, by default yes.
        public bool Visibility { get; set; } = true;

        // Notes for the token, can be used by the DM for additional information.
        public string Notes { get; set; } = string.Empty;
    }
}
