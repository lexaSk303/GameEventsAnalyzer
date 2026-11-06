namespace GameEventsAnalyzer;

public sealed record EventSummary(
    int TotalEvents,
    long TotalPoints,
    int KillCount,
    int DeathCount);