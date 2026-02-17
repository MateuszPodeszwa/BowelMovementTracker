using System.ComponentModel.DataAnnotations;

namespace BowelMovementTracker.Data;

public class Diary
{
    [Key]
    public Guid DiaryIdentifier { get; init; }
    
    [Required]
    public required User User { get; init; }
    public required Guid DiaryUserIdentifier { get; init; }

    public required ICollection<Log> Logs { get; set; }
    
}
