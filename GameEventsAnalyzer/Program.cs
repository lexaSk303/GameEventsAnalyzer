using GameEventsAnalyzer;
internal class Program
{
    static void Main()
    {
        Console.WriteLine("=== GameEventsAnalyzer ===\n");

        // П1-TODO 6-9
        Console.WriteLine("--- П1: Граничные сценарии ---");

        var testEvents = new List<GameEvent>
        {
            new(1, 1, "Kill", 100, DateTime.Now),
            new(2, 1, "Kill", 150, DateTime.Now),
            new(3, 2, "Kill", 200, DateTime.Now),
            new(4, 1, "Death", 0, DateTime.Now),
            new(5, 2, "Loot", 20, DateTime.Now)
        };

        RunTest("Стандартный тест", testEvents);
        RunTest("Пустая коллекция", new List<GameEvent>()); // TODO 6
        RunTest("Нет событий Kill", new List<GameEvent> { new(1, 1, "Death", 0, DateTime.Now) }); // TODO 7
        RunTest("Все убийства одного игрока", new List<GameEvent> { new(1, 1, "Kill", 100, DateTime.Now), new(2, 1, "Kill", 200, DateTime.Now) }); // TODO 8

        try { GameEventAnalyzer.AnalyzeWithPlinq(null!); } // TODO 9
        catch (ArgumentNullException) { Console.WriteLine("null корректно отклонен.\n"); }


        // П2-TODO 1-6
        Console.WriteLine("--- П2: Проверка порядка ---");
        var orderedEvents = GameEventAnalyzer.GetKillsOrdered(testEvents);
        var sortedEvents = GameEventAnalyzer.GetKillsSortedByTime(testEvents);

        // TODO 3
        bool isOrdered = orderedEvents.Select(e => e.Id).SequenceEqual(orderedEvents.Select(e => e.Id).OrderBy(id => id));
        Console.WriteLine($"AsOrdered сохранил исходный порядок: {isOrdered}");

        Console.WriteLine($"Отсортировано по времени: {string.Join(", ", sortedEvents.Select(e => e.Id))}");

        // TODO 9, 10 П1 и П2
        Console.WriteLine("--- Бенчмарк ---");
        var largeEvents = GameEventGenerator.Generate(1_000_000, seed: 12345);
        BenchmarkRunner.Compare(largeEvents);

        Console.WriteLine("\n=== Объяснения  ===");
        Console.WriteLine("1. Почему PLINQ не всегда быстрее LINQ? П1-TODO 10");
        Console.WriteLine("   Накладные расходы на разделение данных, создание потоков и объединение результатов могут превышать выгоду от параллелизма, особенно на простых или малых данных.");

        Console.WriteLine("2. Зачем нужен ThenBy по Id? П2-TODO 8");
        Console.WriteLine("   Чтобы гарантировать стабильный порядок элементов, если у нескольких событий одинаковое значение основного ключа сортировки (например, Timestamp).");

        Console.WriteLine("3. Почему сохранение порядка (AsOrdered) снижает производительность? П2-TODO 10");
        Console.WriteLine("   PLINQ вынужден буферизировать результаты и синхронизировать потоки, чтобы выдать их в исходном порядке, что убивает преимущества параллельной обработки.");
    }

    static void RunTest(string name, List<GameEvent> events)
    {
        var linq = GameEventAnalyzer.AnalyzeWithLinq(events);
        var plinq = GameEventAnalyzer.AnalyzeWithPlinq(events);
        bool match = linq.SequenceEqual(plinq);
        Console.WriteLine($"[{name}] Совпадение LINQ и PLINQ: {match} (Найдено игроков: {linq.Count})");
    }
}