using Llyn.Performance;

LDrill[] drills = [new LDrillMarkup(), new LDrillLanguage()];

string[] chosen = [];
int repeat = 20;
int warmup = 3;
for (int index = 0; index < args.Length; index++)
{
    string value = index + 1 < args.Length ? args[index + 1] : string.Empty;
    switch (args[index])
    {
        case "--list":
            foreach (LDrill drill in drills)
            {
                Console.WriteLine(drill.GetType().Name[nameof(LDrill).Length..]);
            }

            return 0;
        case "--drill":
            chosen = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            index++;
            break;
        case "--repeat":
            repeat = int.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
            index++;
            break;
        case "--warmup":
            warmup = int.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
            index++;
            break;
        default:
            Console.Error.WriteLine($"Unknown argument: {args[index]}");
            return 2;
    }
}

foreach (string name in chosen)
{
    if (!drills.Any(drill => string.Equals(drill.GetType().Name, nameof(LDrill) + name, StringComparison.OrdinalIgnoreCase)))
    {
        Console.Error.WriteLine($"Unknown drill: {name}");
        return 2;
    }
}

foreach (LDrill drill in drills)
{
    string name = drill.GetType().Name[nameof(LDrill).Length..];
    if (chosen.Length > 0 && !chosen.Contains(name, StringComparer.OrdinalIgnoreCase))
    {
        continue;
    }

    drill.LDrillPrepare();
    for (int cycle = 0; cycle < warmup; cycle++)
    {
        drill.LDrillCycleRun();
    }

    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
    drill.LDrillRun(repeat);
    watch.Stop();
    Console.WriteLine($"drill {name}: {repeat} cycles, {watch.Elapsed.TotalMilliseconds / repeat:0.00} ms per cycle");
}

return 0;
