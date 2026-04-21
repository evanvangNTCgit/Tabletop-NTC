using SAGroupAlphaSpring26.Models;
using System.Collections.Generic;

namespace SAGroupAlphaSpring26.ViewModels
{
    /// <summary>
    /// ViewModel for the Piece Usage admin page with filter, stats, and totals.
    /// </summary>
    public class PieceUsageViewModel
    {
/// <summary>
        /// Top 10 PieceUsageStatDto, filtered and ordered (flat DTOs).
        /// </summary>
        public List<PieceUsageStatDto> Stats { get; set; } = new();

        /// <summary>
        /// All available PieceTypes for dropdown filter.
        /// </summary>
        public List<PieceType> Types { get; set; } = new();

        /// <summary>
        /// Selected filter PieceType ID (null for all).
        /// </summary>
        public int? FilterTypeId { get; set; }

        /// <summary>
        /// Total token count for percentage calculations (filtered total).
        /// </summary>
        /// <summary>
        /// Filtered total tokens (for table display)
        /// </summary>
        public int TotalTokens { get; set; }
        /// <summary>
        /// Global total tokens across ALL data (for % calculations)
        /// </summary>
        public int GlobalTotalTokens { get; set; }
    }
}


