namespace GameEventsAnalyzer;

public static class GameEventAnalyzer
{
    // П1: 
    // TODO 1, 3, 4
    public static List<PlayerStatistics> AnalyzeWithLinq(IEnumerable<GameEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events); // TODO 9

        return events
            .Where(e => e.Type == "Kill")
            .GroupBy(e => e.PlayerId)
            .Select(group => new PlayerStatistics(
                PlayerId: group.Key,
                KillCount: group.Count(),
                TotalPoints: group.Sum(e => (long)e.Points)))
            .OrderByDescending(x => x.TotalPoints)
            .ThenBy(x => x.PlayerId) // TODO 4: 
            .ToList();
    }

    // TODO 2:
    public static List<PlayerStatistics> AnalyzeWithPlinq(IEnumerable<GameEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);

        return events
            .AsParallel()
            .Where(e => e.Type == "Kill")
            .GroupBy(e => e.PlayerId)
            .Select(group => new PlayerStatistics(
                PlayerId: group.Key,
                KillCount: group.Count(),
                TotalPoints: group.Sum(e => (long)e.Points)))
            .OrderByDescending(x => x.TotalPoints)
            .ThenBy(x => x.PlayerId)
            .ToList();
    }

    // П2
    // TODO 1
    public static List<GameEvent> GetKillsUnordered(IEnumerable<GameEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);
        return events.AsParallel().Where(e => e.Type == "Kill").ToList();
    }

    // TODO 2
    public static List<GameEvent> GetKillsOrdered(IEnumerable<GameEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);
        return events.AsParallel().AsOrdered().Where(e => e.Type == "Kill").ToList();
    }

    // TODO 4, 5, 6
    public static List<GameEvent> GetKillsSortedByTime(IEnumerable<GameEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);
        return events
            .AsParallel()
            .Where(e => e.Type == "Kill")
            .OrderBy(e => e.Timestamp)
            .ThenBy(e => e.Id) // TODO 5
            .ToList();
    }
    public static List<int> GetPlayerIdsCorrect(IEnumerable<GameEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);
        return events.AsParallel().Select(e => e.PlayerId).ToList();
    }

    public static EventSummary AnalyzeWithAggregate(IEnumerable<GameEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);

        return events.AsParallel().Aggregate(
            () => new EventSummaryAccumulator(),
            (local, gameEvent) =>
            {
                local.TotalEvents++;
                local.TotalPoints += gameEvent.Points;
                if (gameEvent.Type == "Kill") local.KillCount++;
                if (gameEvent.Type == "Death") local.DeathCount++;
                return local;
            },
            (first, second) =>
            {
                first.TotalEvents += second.TotalEvents;
                first.TotalPoints += second.TotalPoints;
                first.KillCount += second.KillCount;
                first.DeathCount += second.DeathCount;
                return first;
            },
            result => new EventSummary(result.TotalEvents, result.TotalPoints, result.KillCount, result.DeathCount));
    }

    private sealed class EventSummaryAccumulator
    {
        public int TotalEvents { get; set; }
        public long TotalPoints { get; set; }
        public int KillCount { get; set; }
        public int DeathCount { get; set; }
    }
}