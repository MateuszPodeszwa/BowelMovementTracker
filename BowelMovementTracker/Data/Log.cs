using System.ComponentModel.DataAnnotations;
using BowelMovementTracker.Data.Enums;

namespace BowelMovementTracker.Data;

public class Log
{
    [Key]
    public Guid LogIdentifier { get; init; }
    
    public required Diary Diary { get; set; }
    public required Guid LogDiaryIdentifier { get; set; }
    
    [Required]
    public required BristolStoolScale LogBowelMovementType { get; set; }
    public required DateTime LogLastUpdated { get; set; }
    
    public DateTime? LogDateTime { get; init; }
    public bool? LogWasCoffeeConsumed { get; set; }
    public bool? LogWasMilkConsumed { get; set; }
    public string? LogNotes { get; set; }

}