<#
.SYNOPSIS
    Estimate how long each Llyn method takes, with the time of every method it calls folded in.
.DESCRIPTION
    Runs a workload under the .NET runtime's own sampling profiler and folds the sampled call
    stacks into a report. The workload is either the drills of the performance project or the
    existing test projects. Nothing is gated: the script only measures and reports.

    The runtime writes one trace per process while DOTNET_EnableEventPipe is set, so the
    testhost process that dotnet test starts is traced like the drill process. Each sample
    weighs the time since its thread's previous sample. Inclusive time holds everything a
    method called, self time only its own code and the framework code it called directly.

    Drill mode counts only samples beneath LDrill.LDrillRun, so warmup and preparation stay
    out. Test mode roots every sample at the outermost frame of a test project, which is the
    test method. Samples of Llyn code resumed after an await, with no root on the stack, are
    gathered under an [async] root.

    Every project-specific value lives in estimateperformance.json. The folding runs in a
    file-based app that reads the traces with TraceEvent. Its source is held in this script
    and written to the fold folder under the output folder before each run.
    Reports go to a stamped folder under the configured output folder: report.md,
    report.json, flame.html, and the raw traces.
.PARAMETER Drill
    Names of the drills to run. Positional. Defaults to every drill.
.PARAMETER Test
    Measure the test projects instead of the drills.
.PARAMETER Filter
    VSTest filter expression passed to dotnet test. Test mode only.
.PARAMETER Project
    Test projects to measure. Defaults to the projects in estimateperformance.json.
.PARAMETER Repeat
    Drill mode: timed cycles per drill. Test mode: dotnet test runs. Every time is divided by it.
.PARAMETER Warmup
    Untimed cycles each drill runs before its timed cycles. Drill mode only.
.PARAMETER Method
    Focus the call tree and the flame graph on one method, merged over every place it was
    called from, and list its callers. Accepts Type.Method or a full name.
.PARAMETER Depth
    Levels of the call tree printed.
.PARAMETER Share
    Percent of the measured time below which a call tree node is folded away.
.PARAMETER Top
    Rows of the inclusive and self tables.
.PARAMETER Configuration
    Build configuration. Defaults to the configuration in estimateperformance.json.
.PARAMETER NoInline
    Set DOTNET_JitNoInline so small methods keep their own frames. Absolute times grow.
.PARAMETER List
    List the drills and exit.
.PARAMETER Help
    Display this help and exit. The alias -? is supported.
.EXAMPLE
    estimateperformance
    Run every drill 20 timed cycles and report.
.EXAMPLE
    estimateperformance Markup -Repeat 50 -Method LMarkupFile.LMarkupFileParse
    Run the markup drill and focus on everything LMarkupFileParse calls.
.EXAMPLE
    estimateperformance -Test -Filter "FullyQualifiedName~TMarkup" -Repeat 3
    Measure the markup tests over three dotnet test runs.
.EXAMPLE
    estimateperformance -Test -Project Llyn.Internal
    Measure the whole portable test project.
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string[]]$Drill = @(),
    [switch]$Test,
    [string]$Filter = '',
    [string[]]$Project = @(),
    [int]$Repeat = 0,
    [int]$Warmup = -1,
    [string]$Method = '',
    [int]$Depth = 0,
    [double]$Share = -1,
    [int]$Top = 0,
    [string]$Configuration = '',
    [switch]$NoInline,
    [switch]$List,
    [Alias('?')]
    [switch]$Help
)

$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)

# The folding app. It lives here, not beside the script, so the name audit never reads tooling code.
$script:FoldSource = @'
#:package Microsoft.Diagnostics.Tracing.TraceEvent@3.2.6
#:property PublishAot=false
#:property Nullable=enable

using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Etlx;

Console.OutputEncoding = new UTF8Encoding(false);
Options options = Options.Parse(args);
Fold fold = new(options);
string[] traces = Directory.GetFiles(options.Traces, "*.nettrace");
Array.Sort(traces, StringComparer.Ordinal);
if (traces.Length == 0)
{
    Console.Error.WriteLine($"No trace was found in {options.Traces}");
    return 1;
}

foreach (string trace in traces)
{
    fold.Read(trace);
}

Report.Write(fold, options);
return 0;

sealed class Options
{
    public string Traces = "";
    public string Output = "";
    public string Label = "";
    public Regex? Gate;
    public HashSet<string> Roots = new(StringComparer.OrdinalIgnoreCase);
    public string Own = "Llyn";
    public double Interval = 1.0;
    public double Gap = 100.0;
    public int Divisor = 1;
    public int Top = 25;
    public int Depth = 6;
    public double Share = 1.0;
    public string? Method;

    public static Options Parse(string[] args)
    {
        Options options = new();
        for (int index = 0; index + 1 < args.Length; index += 2)
        {
            string value = args[index + 1];
            switch (args[index])
            {
                case "--traces": options.Traces = value; break;
                case "--output": options.Output = value; break;
                case "--label": options.Label = value; break;
                case "--gate": options.Gate = value.Length == 0 ? null : new Regex(value, RegexOptions.CultureInvariant); break;
                case "--roots": options.Roots.UnionWith(value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)); break;
                case "--own": options.Own = value; break;
                case "--interval": options.Interval = double.Parse(value, CultureInfo.InvariantCulture); break;
                case "--gap": options.Gap = double.Parse(value, CultureInfo.InvariantCulture); break;
                case "--divisor": options.Divisor = Math.Max(1, int.Parse(value, CultureInfo.InvariantCulture)); break;
                case "--top": options.Top = int.Parse(value, CultureInfo.InvariantCulture); break;
                case "--depth": options.Depth = int.Parse(value, CultureInfo.InvariantCulture); break;
                case "--share": options.Share = double.Parse(value, CultureInfo.InvariantCulture); break;
                case "--method": options.Method = value.Length == 0 ? null : value; break;
                default: throw new ArgumentException($"Unknown argument: {args[index]}");
            }
        }

        if (options.Traces.Length == 0 || options.Output.Length == 0)
        {
            throw new ArgumentException("--traces and --output are required.");
        }

        return options;
    }
}

sealed class Tally
{
    public double Cpu;
    public double Wait;
    public long Count;

    public double Total => Cpu + Wait;

    public void Add(bool cpu, double weight)
    {
        Count++;
        if (cpu) { Cpu += weight; } else { Wait += weight; }
    }

    public void Merge(Tally other)
    {
        Cpu += other.Cpu;
        Wait += other.Wait;
        Count += other.Count;
    }
}

sealed class Node(string name)
{
    public string Name = name;
    public Tally Inclusive = new();
    public Tally Self = new();
    public Dictionary<string, Node> Children = new(StringComparer.Ordinal);

    public Node Child(string name)
    {
        if (!Children.TryGetValue(name, out Node? child))
        {
            child = new Node(name);
            Children[name] = child;
        }

        return child;
    }

    public void Merge(Node other)
    {
        Inclusive.Merge(other.Inclusive);
        Self.Merge(other.Self);
        foreach (Node child in other.Children.Values)
        {
            Child(child.Name).Merge(child);
        }
    }

    public IEnumerable<Node> Ordered() =>
        Children.Values.OrderByDescending(child => child.Inclusive.Total).ThenBy(child => child.Name, StringComparer.Ordinal);
}

readonly record struct Frame(string Module, string Name);

sealed class Fold(Options options)
{
    public const string Async = "[async]";
    private const string Sampler = "Microsoft-DotNETCore-SampleProfiler";

    public readonly Dictionary<string, Tally> Inclusive = new(StringComparer.Ordinal);
    public readonly Dictionary<string, Tally> Self = new(StringComparer.Ordinal);
    public readonly Dictionary<(string Caller, string Callee), Tally> Edges = new();
    public readonly Node Tree = new("all");
    public readonly List<string> Files = [];
    public long Samples;
    public long Outside;
    public long Other;
    public double Interval => _gaps == 0 ? options.Interval : _span / _gaps;
    private readonly Dictionary<string, string> _names = new(StringComparer.Ordinal);
    private readonly Dictionary<int, double> _last = [];
    private double _span;
    private long _gaps;

    public void Read(string trace)
    {
        Files.Add(Path.GetFileName(trace));
        string etlx = Path.ChangeExtension(trace, ".etlx");
        TraceLog.CreateFromEventPipeDataFile(trace, etlx);
        using TraceLog log = new(etlx);
        List<Frame> frames = [];
        _last.Clear();
        foreach (TraceEvent sample in log.Events)
        {
            if (sample.ProviderName != Sampler)
            {
                continue;
            }

            frames.Clear();
            for (TraceCallStack? stack = sample.CallStack(); stack is not null; stack = stack.Caller)
            {
                frames.Add(new Frame(stack.CodeAddress.ModuleName ?? "", Normalize(stack.CodeAddress.FullMethodName ?? "")));
            }

            frames.Reverse();
            Samples++;
            double now = sample.TimeStampRelativeMSec;
            double weight = Interval;
            if (_last.TryGetValue(sample.ThreadID, out double last) && now - last > 0 && now - last < options.Gap)
            {
                weight = now - last;
                _span += weight;
                _gaps++;
            }

            _last[sample.ThreadID] = now;
            Classify(frames, string.Equals(sample.PayloadByName("Type")?.ToString(), "Managed", StringComparison.Ordinal), weight);
        }
    }

    private bool Owned(Frame frame) =>
        frame.Module.StartsWith(options.Own, StringComparison.OrdinalIgnoreCase) && frame.Name.Length > 0;

    private void Classify(List<Frame> frames, bool cpu, double weight)
    {
        int start = 0;
        if (options.Gate is not null)
        {
            int gate = frames.FindLastIndex(frame => Owned(frame) && options.Gate.IsMatch(frame.Name));
            if (gate < 0)
            {
                if (frames.Exists(frame => options.Roots.Contains(frame.Module)))
                {
                    Outside++;
                }
                else if (frames.Exists(Owned))
                {
                    Record(Trail(frames, 0, Async), cpu, weight);
                }
                else
                {
                    Other++;
                }

                return;
            }

            start = gate + 1;
        }

        int root = frames.FindIndex(start, frame => options.Roots.Contains(frame.Module) && frame.Name.Length > 0);
        if (root >= 0)
        {
            Record(Trail(frames, root, null), cpu, weight);
        }
        else if (options.Gate is not null)
        {
            Outside++;
        }
        else if (frames.Exists(Owned))
        {
            Record(Trail(frames, 0, Async), cpu, weight);
        }
        else
        {
            Other++;
        }
    }

    private List<string> Trail(List<Frame> frames, int from, string? head)
    {
        List<string> path = [];
        if (head is not null)
        {
            path.Add(head);
        }

        for (int index = from; index < frames.Count; index++)
        {
            Frame frame = frames[index];
            if (((head is null && index == from) || Owned(frame)) && (path.Count == 0 || path[^1] != frame.Name))
            {
                path.Add(frame.Name);
            }
        }

        return path;
    }

    private void Record(List<string> path, bool cpu, double weight)
    {
        foreach (string name in path.Distinct(StringComparer.Ordinal))
        {
            Slot(Inclusive, name).Add(cpu, weight);
        }

        Slot(Self, path[^1]).Add(cpu, weight);
        HashSet<(string, string)> seen = [];
        for (int index = 0; index + 1 < path.Count; index++)
        {
            (string, string) edge = (path[index], path[index + 1]);
            if (seen.Add(edge))
            {
                if (!Edges.TryGetValue(edge, out Tally? tally))
                {
                    tally = new Tally();
                    Edges[edge] = tally;
                }

                tally.Add(cpu, weight);
            }
        }

        Node node = Tree;
        Tree.Inclusive.Add(cpu, weight);
        foreach (string name in path)
        {
            node = node.Child(name);
            node.Inclusive.Add(cpu, weight);
        }

        node.Self.Add(cpu, weight);
    }

    private static Tally Slot(Dictionary<string, Tally> map, string name)
    {
        if (!map.TryGetValue(name, out Tally? tally))
        {
            tally = new Tally();
            map[name] = tally;
        }

        return tally;
    }

    private static readonly Regex Generic = new(@"\[[^\[\]]*\]", RegexOptions.CultureInvariant);
    private static readonly Regex Arity = new(@"`\d+", RegexOptions.CultureInvariant);
    private static readonly Regex StateMachine = new(@"^(?<type>.+?)\+<(?<method>[^>]+)>d__[\d_]+\.MoveNext$", RegexOptions.CultureInvariant);
    private static readonly Regex Closure = new(@"^(?<type>.+?)\+<>c(?:__DisplayClass[\d_]+)?\.<(?<method>[^>]+)>[bg]__.*$", RegexOptions.CultureInvariant);
    private static readonly Regex Local = new(@"^(?<type>.+)\.<(?<method>[^>]+)>g__.*$", RegexOptions.CultureInvariant);

    private string Normalize(string full)
    {
        if (_names.TryGetValue(full, out string? known))
        {
            return known;
        }

        string name = full;
        int open = name.IndexOf('(');
        if (open >= 0)
        {
            name = name[..open];
        }

        string previous;
        do
        {
            previous = name;
            name = Generic.Replace(name, "");
        }
        while (name != previous);

        name = Arity.Replace(name, "");
        foreach (Regex shape in (Regex[])[StateMachine, Closure, Local])
        {
            Match match = shape.Match(name);
            if (match.Success)
            {
                name = match.Groups["type"].Value + "." + match.Groups["method"].Value;
                break;
            }
        }

        name = name.Replace('+', '.');
        _names[full] = name;
        return name;
    }

    public static string Short(string name)
    {
        if (name.StartsWith('['))
        {
            return name;
        }

        int split = name.EndsWith("..ctor", StringComparison.Ordinal) || name.EndsWith("..cctor", StringComparison.Ordinal)
            ? name.LastIndexOf("..", StringComparison.Ordinal)
            : name.LastIndexOf('.');
        if (split <= 0)
        {
            return name;
        }

        string type = name[..split];
        int dot = type.LastIndexOf('.');
        return (dot >= 0 ? type[(dot + 1)..] : type) + name[split..];
    }
}

static class Report
{
    public static void Write(Fold fold, Options options)
    {
        Directory.CreateDirectory(options.Output);
        double rooted = fold.Tree.Inclusive.Total;
        StringBuilder text = new();
        Line(text, $"# Performance estimate: {options.Label}");
        Line(text, "");
        Line(text, $"Every time is milliseconds per run: sampled time / {options.Divisor} run(s).");
        Line(text, $"Each sample weighs the time since its thread's previous sample, {Number(fold.Interval)} ms on average.");
        Line(text, "Inclusive time holds everything a method called. Self time holds only its own code and the framework code it called directly.");
        Line(text, "CPU samples found the thread running. Wait samples found it blocked.");
        Line(text, "");
        Line(text, "| Samples | Count |");
        Line(text, "|---|---:|");
        Line(text, $"| Measured | {fold.Tree.Inclusive.Count} |");
        Line(text, $"| Outside the measured region | {fold.Outside} |");
        Line(text, $"| No Llyn frame | {fold.Other} |");
        Line(text, $"| Total | {fold.Samples} |");
        Line(text, "");
        Line(text, $"Traces: {string.Join(", ", fold.Files)}");
        Line(text, "");

        Line(text, "## Roots");
        Line(text, "");
        Table(text, fold.Tree.Ordered().Select(node => (node.Name, node.Inclusive, node.Self)), rooted, options, int.MaxValue);

        List<(string Name, Tally Inclusive, Tally Self)> methods = fold.Inclusive
            .Select(pair => (pair.Key, pair.Value, fold.Self.TryGetValue(pair.Key, out Tally? self) ? self : new Tally()))
            .ToList();
        Line(text, "## Top by inclusive time");
        Line(text, "");
        Table(text, methods.OrderByDescending(row => row.Inclusive.Total).ThenBy(row => row.Name, StringComparer.Ordinal), rooted, options, options.Top);
        Line(text, "## Top by self time");
        Line(text, "");
        Table(text, methods.Where(row => row.Self.Total > 0).OrderByDescending(row => row.Self.Total).ThenBy(row => row.Name, StringComparer.Ordinal), rooted, options, options.Top);

        Node focus = fold.Tree;
        if (options.Method is not null)
        {
            focus = Focus(fold, options.Method);
            Line(text, $"## Focus: {options.Method}");
            Line(text, "");
            Line(text, "Callers:");
            Line(text, "");
            foreach (KeyValuePair<(string Caller, string Callee), Tally> edge in fold.Edges
                .Where(pair => Matches(pair.Key.Callee, options.Method))
                .OrderByDescending(pair => pair.Value.Total))
            {
                Line(text, $"- {Fold.Short(edge.Key.Caller)} -> {Fold.Short(edge.Key.Callee)}: {Milliseconds(edge.Value.Total, options)} ms");
            }

            Line(text, "");
        }

        Line(text, "## Call tree");
        Line(text, "");
        Line(text, $"Depth {options.Depth}, nodes under {Number(options.Share)} % of the measured samples are folded away.");
        Line(text, "");
        Line(text, "```");
        StringBuilder tree = new();
        if (focus == fold.Tree)
        {
            Leaves(tree, focus, 0, Math.Max(1e-9, focus.Inclusive.Total), options);
        }
        else
        {
            Branch(tree, focus, 0, Math.Max(1e-9, focus.Inclusive.Total), options);
        }

        text.Append(tree);
        Line(text, "```");

        Save(Path.Combine(options.Output, "report.md"), text.ToString());
        Save(Path.Combine(options.Output, "report.json"), Json(fold, options));
        Save(Path.Combine(options.Output, "flame.html"), Flame(fold, focus, options));

        Console.WriteLine($"Measured {fold.Tree.Inclusive.Count} of {fold.Samples} samples ({fold.Outside} outside, {fold.Other} without a Llyn frame).");
        Console.WriteLine();
        Console.WriteLine($"{"Inclusive ms",12} {"Self ms",10} {"Share",7}  Method");
        foreach ((string Name, Tally Inclusive, Tally Self) row in methods.OrderByDescending(row => row.Inclusive.Total).Take(Math.Min(options.Top, 15)))
        {
            Console.WriteLine($"{Milliseconds(row.Inclusive.Total, options),12} {Milliseconds(row.Self.Total, options),10} {Percent(row.Inclusive.Total, rooted),7}  {Fold.Short(row.Name)}");
        }

        Console.WriteLine();
        Console.Write(tree.ToString());
        Console.WriteLine();
        Console.WriteLine($"Report: {Path.Combine(options.Output, "report.md")}");
        Console.WriteLine($"Flame:  {Path.Combine(options.Output, "flame.html")}");
    }

    private static bool Matches(string name, string method) =>
        string.Equals(name, method, StringComparison.OrdinalIgnoreCase)
        || string.Equals(Fold.Short(name), method, StringComparison.OrdinalIgnoreCase)
        || name.EndsWith("." + method, StringComparison.OrdinalIgnoreCase);

    private static Node Focus(Fold fold, string method)
    {
        Node focus = new(method);
        Collect(fold.Tree, method, focus);
        return focus;
    }

    private static void Collect(Node node, string method, Node focus)
    {
        foreach (Node child in node.Children.Values)
        {
            if (Matches(child.Name, method))
            {
                focus.Merge(child);
            }
            else
            {
                Collect(child, method, focus);
            }
        }
    }

    private static void Branch(StringBuilder text, Node node, int depth, double whole, Options options)
    {
        text.Append(new string(' ', depth * 2))
            .Append(Fold.Short(node.Name))
            .Append("  ").Append(Milliseconds(node.Inclusive.Total, options)).Append(" ms")
            .Append(" (self ").Append(Milliseconds(node.Self.Total, options)).Append(")")
            .Append("  ").Append(Percent(node.Inclusive.Total, whole))
            .Append('\n');
        if (depth + 1 < options.Depth)
        {
            Leaves(text, node, depth + 1, whole, options);
        }
    }

    private static void Leaves(StringBuilder text, Node node, int depth, double whole, Options options)
    {
        double folded = 0;
        int count = 0;
        foreach (Node child in node.Ordered())
        {
            if (child.Inclusive.Total * 100.0 / whole < options.Share)
            {
                folded += child.Inclusive.Total;
                count++;
                continue;
            }

            Branch(text, child, depth, whole, options);
        }

        if (folded > 0)
        {
            text.Append(new string(' ', depth * 2)).Append("[folded ").Append(count).Append("]  ").Append(Milliseconds(folded, options)).Append(" ms\n");
        }
    }

    private static void Table(StringBuilder text, IEnumerable<(string Name, Tally Inclusive, Tally Self)> rows, double whole, Options options, int top)
    {
        Line(text, "| Method | Inclusive ms | Self ms | CPU ms | Wait ms | Share | Samples |");
        Line(text, "|---|---:|---:|---:|---:|---:|---:|");
        foreach ((string Name, Tally Inclusive, Tally Self) row in rows.Take(top))
        {
            Line(text, $"| `{Fold.Short(row.Name)}` | {Milliseconds(row.Inclusive.Total, options)} | {Milliseconds(row.Self.Total, options)} | {Milliseconds(row.Inclusive.Cpu, options)} | {Milliseconds(row.Inclusive.Wait, options)} | {Percent(row.Inclusive.Total, whole)} | {row.Inclusive.Count} |");
        }

        Line(text, "");
    }

    private static string Json(Fold fold, Options options)
    {
        using MemoryStream stream = new();
        using (Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartObject();
            writer.WriteString("label", options.Label);
            writer.WriteNumber("interval", Math.Round(fold.Interval, 3));
            writer.WriteNumber("runs", options.Divisor);
            writer.WriteNumber("samples", fold.Samples);
            writer.WriteNumber("measured", fold.Tree.Inclusive.Count);
            writer.WriteNumber("outside", fold.Outside);
            writer.WriteNumber("other", fold.Other);
            writer.WriteStartArray("methods");
            foreach (KeyValuePair<string, Tally> pair in fold.Inclusive.OrderByDescending(pair => pair.Value.Total).ThenBy(pair => pair.Key, StringComparer.Ordinal))
            {
                Tally self = fold.Self.TryGetValue(pair.Key, out Tally? found) ? found : new Tally();
                writer.WriteStartObject();
                writer.WriteString("method", pair.Key);
                writer.WriteNumber("inclusiveMs", Value(pair.Value.Total, options));
                writer.WriteNumber("selfMs", Value(self.Total, options));
                writer.WriteNumber("cpuMs", Value(pair.Value.Cpu, options));
                writer.WriteNumber("waitMs", Value(pair.Value.Wait, options));
                writer.WriteNumber("samples", pair.Value.Count);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteStartArray("edges");
            foreach (KeyValuePair<(string Caller, string Callee), Tally> pair in fold.Edges.OrderByDescending(pair => pair.Value.Total).ThenBy(pair => pair.Key.Caller, StringComparer.Ordinal).ThenBy(pair => pair.Key.Callee, StringComparer.Ordinal))
            {
                writer.WriteStartObject();
                writer.WriteString("caller", pair.Key.Caller);
                writer.WriteString("callee", pair.Key.Callee);
                writer.WriteNumber("inclusiveMs", Value(pair.Value.Total, options));
                writer.WriteNumber("samples", pair.Value.Count);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WritePropertyName("tree");
            Nest(writer, fold.Tree, options, fold.Tree.Inclusive.Total / 2000);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray()).Replace("\r\n", "\n") + "\n";
    }

    private static void Nest(Utf8JsonWriter writer, Node node, Options options, double floor)
    {
        writer.WriteStartObject();
        writer.WriteString("n", Fold.Short(node.Name));
        writer.WriteString("f", node.Name);
        writer.WriteNumber("v", Value(node.Inclusive.Total, options));
        writer.WriteNumber("s", Value(node.Self.Total, options));
        writer.WriteNumber("w", Value(node.Inclusive.Wait, options));
        writer.WriteStartArray("c");
        foreach (Node child in node.Ordered().Where(child => child.Inclusive.Total > floor))
        {
            Nest(writer, child, options, floor);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    private static string Flame(Fold fold, Node focus, Options options)
    {
        using MemoryStream stream = new();
        using (Utf8JsonWriter writer = new(stream))
        {
            Nest(writer, focus, options, focus.Inclusive.Total / 2000);
        }

        string data = Encoding.UTF8.GetString(stream.ToArray()).Replace("</", "<\\/");
                return FlameTemplate
            .Replace("%TITLE%", System.Net.WebUtility.HtmlEncode(options.Label))
            .Replace("%DATA%", data);
    }

    private static double Value(double time, Options options) =>
        Math.Round(time / options.Divisor, 3);

    private static string Milliseconds(double time, Options options) =>
        (time / options.Divisor).ToString("0.0", CultureInfo.InvariantCulture);

    private static string Percent(double part, double whole) =>
        whole == 0 ? "0.0 %" : (part * 100.0 / whole).ToString("0.0", CultureInfo.InvariantCulture) + " %";

    private static string Number(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    private static void Line(StringBuilder text, string line) => text.Append(line).Append('\n');

    private static void Save(string path, string text) =>
        File.WriteAllText(path, text.Replace("\r\n", "\n"), new UTF8Encoding(false));

    private const string FlameTemplate = """
<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Performance flame</title>
<style>
:root { --bg: #fbfaf8; --fg: #1f2328; --muted: #6a6f76; --line: #d9d6d0; --hue: 28; --light: 72%; }
@media (prefers-color-scheme: dark) { :root { --bg: #16181b; --fg: #e6e4df; --muted: #9aa0a6; --line: #34373c; --light: 38%; } }
* { box-sizing: border-box; }
body { margin: 0; padding: 16px; background: var(--bg); color: var(--fg); font: 13px/1.4 system-ui, sans-serif; }
h1 { font-size: 16px; margin: 0 0 4px; }
p { margin: 0 0 12px; color: var(--muted); }
#crumb { margin-bottom: 8px; color: var(--muted); min-height: 1.4em; }
#crumb a { color: var(--fg); cursor: pointer; text-decoration: underline; }
#chart { position: relative; width: 100%; overflow: hidden; border-top: 1px solid var(--line); }
.bar { position: absolute; height: 20px; padding: 0 4px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis; border: 1px solid var(--bg); border-radius: 3px; cursor: pointer; font-size: 12px; line-height: 18px; color: #111; }
.bar:hover { outline: 2px solid var(--fg); z-index: 1; }
#tip { margin-top: 12px; font-family: ui-monospace, monospace; white-space: pre-wrap; }
</style>
</head>
<body>
<h1>%TITLE%</h1>
<p>Each bar is a method: width is inclusive time, bars beneath are what it called. Click a bar to zoom, click the path to go back.</p>
<div id="crumb"></div>
<div id="chart"></div>
<div id="tip"></div>
<script>
const data = %DATA%;

const chart = document.getElementById("chart");
const crumb = document.getElementById("crumb");
const tip = document.getElementById("tip");
const row = 21;
function hue(name) { let h = 0; for (const c of name.split(".")[0]) { h = (h * 31 + c.charCodeAt(0)) % 360; } return h; }
function ms(v) { return v.toFixed(1) + " ms"; }
function depth(node) { return 1 + node.c.reduce((m, c) => Math.max(m, depth(c)), 0); }
function draw(node, x, w, d, whole) {
  if (w < 0.0015) { return; }
  const bar = document.createElement("div");
  bar.className = "bar";
  bar.style.left = (x * 100) + "%";
  bar.style.width = (w * 100) + "%";
  bar.style.top = (d * row) + "px";
  bar.style.background = "hsl(" + hue(node.n) + " 60% " + getComputedStyle(document.documentElement).getPropertyValue("--light") + ")";
  bar.textContent = node.n;
  bar.title = node.f + "\ninclusive " + ms(node.v) + ", self " + ms(node.s) + ", wait " + ms(node.w) + ", " + (node.v * 100 / whole).toFixed(1) + " %";
  bar.onmouseenter = () => { tip.textContent = bar.title; };
  bar.onclick = (event) => { event.stopPropagation(); zoom(node); };
  chart.appendChild(bar);
  let offset = x;
  for (const child of node.c) { const cw = node.v ? w * child.v / node.v : 0; draw(child, offset, cw, d + 1, whole); offset += cw; }
}
const trail = [];
function zoom(node) {
  const at = trail.indexOf(node);
  if (at >= 0) { trail.length = at + 1; } else { trail.push(node); }
  chart.innerHTML = "";
  chart.style.height = (depth(node) * row + 4) + "px";
  draw(node, 0, 1, 0, node.v || 1);
  crumb.innerHTML = "";
  trail.forEach((n, i) => {
    const a = document.createElement("a");
    a.textContent = n.n;
    a.onclick = () => zoom(n);
    crumb.appendChild(a);
    if (i < trail.length - 1) { crumb.appendChild(document.createTextNode(" > ")); }
  });
}
zoom(data);
</script>
</body>
</html>
""";
}
'@

if ($Help) {
    Get-Help -Name $PSCommandPath -Detailed
    return
}

$root = Split-Path -Parent $PSScriptRoot
$settingsPath = Join-Path $PSScriptRoot 'estimateperformance.json'
$settings = [System.IO.File]::ReadAllText($settingsPath, [System.Text.Encoding]::UTF8) | ConvertFrom-Json

if ($Configuration -eq '') { $Configuration = [string]$settings.configuration }
if ($Depth -le 0) { $Depth = [int]$settings.depth }
if ($Share -lt 0) { $Share = [double]$settings.share }
if ($Top -le 0) { $Top = [int]$settings.top }
if ($Repeat -le 0) {
    if ($Test) { $Repeat = [int]$settings.test.repeat } else { $Repeat = [int]$settings.drill.repeat }
}
if ($Warmup -lt 0) { $Warmup = [int]$settings.drill.warmup }

function Invoke-Native {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$Arguments,
        [switch]$Quiet
    )

    $previous = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        if ($Quiet) {
            $output = & $Name @Arguments 2>&1
            if ($LASTEXITCODE -ne 0) {
                $output | ForEach-Object { Write-Host $_ }
            }
        }
        else {
            & $Name @Arguments 2>&1 | ForEach-Object { Write-Host $_ }
        }

        return $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $previous
    }
}

function Build-Project {
    param([Parameter(Mandatory = $true)][string]$Path)

    Write-Host "Building $([System.IO.Path]::GetFileNameWithoutExtension($Path)) ($Configuration)"
    $code = Invoke-Native -Name 'dotnet' -Arguments @('build', $Path, '-c', $Configuration, '-nologo', '-v', 'q') -Quiet
    if ($code -ne 0) {
        throw "Build failed: $Path"
    }
}

$drillProject = Join-Path $root ([string]$settings.drill.project)
$drillAssembly = Join-Path $root (([string]$settings.drill.assembly).Replace('{configuration}', $Configuration))

if ($List) {
    Build-Project -Path $drillProject
    [void](Invoke-Native -Name 'dotnet' -Arguments @($drillAssembly, '--list'))
    return
}

$mode = 'drill'
if ($Test) { $mode = 'test' }
$stamp = (Get-Date).ToString('yyyyMMdd-HHmmss', [System.Globalization.CultureInfo]::InvariantCulture)
$output = Join-Path (Join-Path $root ([string]$settings.output)) "$stamp-$mode"
$traces = Join-Path $output 'trace'
[void][System.IO.Directory]::CreateDirectory($traces)

$roots = @()
$gate = ''
$label = ''
$runs = @()
if ($Test) {
    $onWindows = [System.Environment]::OSVersion.Platform -eq [System.PlatformID]::Win32NT
    $names = $Project
    if ($names.Count -eq 0) { $names = @($settings.test.projects) }
    foreach ($name in $names) {
        if (-not $onWindows -and @($settings.test.windowsOnly) -contains $name) {
            Write-Host "Skipping $name on a host that is not Windows."
            continue
        }

        $path = Join-Path (Join-Path (Join-Path $root ([string]$settings.test.folder)) $name) "$name.csproj"
        if (-not (Test-Path -LiteralPath $path)) {
            throw "Test project was not found: $path"
        }

        Build-Project -Path $path
        $arguments = @('test', $path, '-c', $Configuration, '--no-build', '-nologo')
        if ($Filter -ne '') { $arguments += @('--filter', $Filter) }
        $runs += , $arguments
        $roots += $name
    }

    $label = "tests $($roots -join ', ')"
    if ($Filter -ne '') { $label += " ($Filter)" }
    $label += ", $Repeat run(s)"
}
else {
    Build-Project -Path $drillProject
    $arguments = @($drillAssembly, '--repeat', [string]$Repeat, '--warmup', [string]$Warmup)
    if ($Drill.Count -gt 0) { $arguments += @('--drill', ($Drill -join ',')) }
    $runs += , $arguments
    $roots = @($settings.drill.roots)
    $gate = [string]$settings.drill.gate
    $label = "drills $(if ($Drill.Count -gt 0) { $Drill -join ', ' } else { 'all' }), $Repeat cycle(s)"
}

if ($runs.Count -eq 0) {
    throw 'Nothing to measure.'
}

$environment = [ordered]@{
    'DOTNET_EnableEventPipe'     = '1'
    'DOTNET_EventPipeOutputPath' = (Join-Path $traces 'trace_{pid}.nettrace')
    'DOTNET_EventPipeConfig'     = [string]$settings.providers
    'DOTNET_JitNoInline'         = $(if ($NoInline) { '1' } else { $null })
}
$saved = @{}
foreach ($key in $environment.Keys) {
    $saved[$key] = [System.Environment]::GetEnvironmentVariable($key, 'Process')
}

$failed = $false
try {
    foreach ($key in $environment.Keys) {
        [System.Environment]::SetEnvironmentVariable($key, $environment[$key], 'Process')
    }

    $passes = 1
    if ($Test) { $passes = $Repeat }
    for ($pass = 1; $pass -le $passes; $pass++) {
        foreach ($arguments in $runs) {
            if ($passes -gt 1) { Write-Host "Run $pass of $passes" }
            $code = Invoke-Native -Name 'dotnet' -Arguments $arguments
            if ($code -ne 0) { $failed = $true }
        }
    }
}
finally {
    foreach ($key in $environment.Keys) {
        [System.Environment]::SetEnvironmentVariable($key, $saved[$key], 'Process')
    }
}

if ($failed) {
    Write-Host 'A measured run failed. Its samples are still folded below.'
}

$foldFolder = Join-Path (Join-Path $root ([string]$settings.output)) 'fold'
$foldPath = Join-Path $foldFolder 'estimateperformance.cs'
$foldText = $script:FoldSource.Replace("`r`n", "`n") + "`n"
[void][System.IO.Directory]::CreateDirectory($foldFolder)
if (-not (Test-Path -LiteralPath $foldPath) -or [System.IO.File]::ReadAllText($foldPath) -ne $foldText) {
    [System.IO.File]::WriteAllText($foldPath, $foldText, [System.Text.UTF8Encoding]::new($false))
}

$fold = @(
    'run', '--file', $foldPath, '--',
    '--traces', $traces,
    '--output', $output,
    '--label', $label,
    '--roots', ($roots -join ','),
    '--own', [string]$settings.own,
    '--interval', ([double]$settings.interval).ToString([System.Globalization.CultureInfo]::InvariantCulture),
    '--gap', ([double]$settings.gap).ToString([System.Globalization.CultureInfo]::InvariantCulture),
    '--divisor', [string]$Repeat,
    '--top', [string]$Top,
    '--depth', [string]$Depth,
    '--share', $Share.ToString([System.Globalization.CultureInfo]::InvariantCulture)
)
if ($gate -ne '') { $fold += @('--gate', $gate) }
if ($Method -ne '') { $fold += @('--method', $Method) }

Write-Host ''
$code = Invoke-Native -Name 'dotnet' -Arguments $fold
if ($code -ne 0) {
    throw 'Folding the traces failed.'
}
