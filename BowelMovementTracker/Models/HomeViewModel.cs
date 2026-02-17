namespace BowelMovementTracker.Models;

public class HomeViewModel
{
    public Guid UserIdentifier { get; init; }
    public int TotalLogCount { get; set; }
    public required List<LogItemsViewModel> LogItems { get; set; }
}