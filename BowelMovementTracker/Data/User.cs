using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BowelMovementTracker.Data;

public class User
{
    [Key]
    public Guid UserIdentifier { get; init; }

    [Required]
    public required string UserEmailAddress { internal get; set; }
    public required string UserPasswordHash { internal get; set; }

    [Required]
    public required Diary Diary { get; init; }
}   