using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using BowelMovementTracker.Data.Enums;

namespace BowelMovementTracker.Models;

public class LogItemsViewModel
{
    public Guid Identifier { get; init; }
    
    public required BristolStoolScale BowelMovementType { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime? DateTime { get; set; } // Original Log Date
    public DateTime? LastUpdated { get; set; }
    
    [Required, DefaultValue(false)]
    public bool WasCoffeeConsumed { get; set; }
    public bool WasMilkConsumed { get; set; }
    
    [MaxLength(512), DataType(DataType.MultilineText)]
    public string? Notes { get; set; }
    
}