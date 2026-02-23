using Microsoft.IdentityModel.Tokens;

namespace BowelMovementTracker.Models;

public class AnalyticsViewModel
{
    public Guid UserIdentifier { get; init; }

    // This is your master list of ALL logs from the database
    public required List<LogItemsViewModel> AllLogItems { get; set; }

    // This is the property your UI Dashboard will bind to for the "Details" view!
    public List<LogItemsViewModel> FilteredDashboardLogs { get; private set; } = [];

    // The Controller calls this ONE time to set the date range for the dashboard
    public void ApplyDashboardDateFilter(DateTime startDate, DateTime endDate)
    {
        FilteredDashboardLogs = AllLogItems
            .Where(log => log.DateTime != null &&
                          log.DateTime >= startDate &&
                          log.DateTime <= endDate)
            .ToList();
    }

    // --- Calculations (Now they just run off the pre-filtered list!) ---
    public int GetFilteredLogCount() => FilteredDashboardLogs.Count;

    public double GetFilteredAverageBristolScore()
    {
        if (FilteredDashboardLogs.Count.Equals(0) || FilteredDashboardLogs.IsNullOrEmpty()) return 0.0;
        
        var calc = FilteredDashboardLogs.Average(item => (int)item.BowelMovementType);
        
        return Math.Round(calc, 2);
    }

    public int GetTotalCoffeeLogCount() => FilteredDashboardLogs.Count(log => log?.WasCoffeeConsumed == true);

    public int GetTotalMilkLogCount() => FilteredDashboardLogs.Count(log => log?.WasMilkConsumed == true);

    public TimeSpan GetFilteredAverageLogTimeSpan()
    {
        if (FilteredDashboardLogs.Count < 2) return TimeSpan.Zero;

        var minDate = FilteredDashboardLogs.Min(l => l.DateTime);
        var maxDate = FilteredDashboardLogs.Max(l => l.DateTime);

        // Telescoping Sum ((Max - Min) / (Count - 1)) - allows to bypass all of that heavy lifting of using LINQ.Average()
        var totalSpan = maxDate - minDate ?? TimeSpan.Zero;
        var averageTicks = totalSpan.Ticks / (FilteredDashboardLogs.Count - 1);

        return TimeSpan.FromTicks(averageTicks);
    }
}