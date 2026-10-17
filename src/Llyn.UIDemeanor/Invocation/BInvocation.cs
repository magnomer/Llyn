using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BInvocation
{
    private readonly IReadOnlyList<KeyValuePair<string, string?>> _bInvocationOptions;

    private BInvocation(IReadOnlyList<KeyValuePair<string, string?>> options, BInvocationGlobal global)
    {
        _bInvocationOptions = options;
        BInvocationGlobal = global;
    }

    internal BInvocationGlobal BInvocationGlobal { get; }

    internal bool BInvocationLeased =>
        _bInvocationOptions.Any(static option =>
            option.Key is "new" or "delete" or "import" || (option.Key is "workspace" && option.Value is not null));

    internal static BInvocation? BInvocationParse(
        IReadOnlyList<string> args, BLocalizationCatalog catalog, out string usage)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(catalog);

        Func<string, string, string, string> word = (key, first, second) =>
            string.Format(CultureInfo.CurrentCulture, catalog.BLocalizationTextRead(key), first, second);

        List<KeyValuePair<string, string?>> options = [];
        HashSet<string> seen = new(StringComparer.Ordinal);
        string? verb = null;
        foreach (string argument in args)
        {
            int colon = argument.IndexOf(':', StringComparison.Ordinal);
            string option = colon < 0 ? argument : argument[..colon];
            string? value = colon < 0 ? null : argument[(colon + 1)..];
            if (option.Length < 2 || option[0] != '-' || !char.IsAsciiLetterLower(option[1])
                || !option.Skip(2).All(static letter => char.IsAsciiLetterLower(letter) || char.IsAsciiDigit(letter)))
            {
                usage = word("Invocation.Malformed", argument, string.Empty);
                return null;
            }

            string name = option[1..];
            bool? isVerb = name switch
            {
                "help" or "version" or "workspace" or "find" or "new" or "delete" or "import" or "export" => true,
                "yes" or "no" or "quiet" or "color" or "format" => false,
                "headword" or "language" or "pronunciation" or "note" or "to" or "as" => false,
                _ => null,
            };
            string? fault = (name, value) switch
            {
                _ when isVerb is null => "Invocation.Unknown",
                ("version" or "yes" or "no" or "quiet", not null) => "Invocation.ValueForbidden",
                ("color" or "format" or "find" or "new" or "delete", null) => "Invocation.ValueMissing",
                ("import" or "export" or "to" or "as", null) => "Invocation.ValueMissing",
                ("headword" or "language" or "pronunciation" or "note", null) => "Invocation.ValueMissing",
                (_, "") => "Invocation.ValueMissing",
                ("color", not ("auto" or "always" or "never")) => "Invocation.ValueUnknown",
                ("format", not ("text" or "tsv" or "json")) => "Invocation.ValueUnknown",
                ("as", not ("markup" or "html" or "markdown" or "docx" or "pdf")) => "Invocation.ValueUnknown",
                ("new", not "entry") => "Invocation.ValueUnknown",
                ("delete" or "export", _)
                    when !long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out long id)
                    || id == 0 => "Invocation.ValueUnknown",
                ("help", not (null or "help" or "version" or "workspace" or "find" or "new" or "delete"
                    or "import" or "export")) => "Invocation.ValueUnknown",
                _ => null,
            };
            fault ??= seen.Add(name) ? null : "Invocation.Repeat";
            if (fault is not null)
            {
                usage = word(fault, option, value ?? string.Empty);
                return null;
            }

            if (isVerb is true && verb is not null)
            {
                usage = word("Invocation.VerbSecond", verb, option);
                return null;
            }

            verb = isVerb is true ? option : verb;
            options.Add(new KeyValuePair<string, string?>(name, value));
        }

        if (seen.Contains("yes") && seen.Contains("no"))
        {
            usage = word("Invocation.Conflict", string.Empty, string.Empty);
            return null;
        }

        if (verb is null)
        {
            usage = word("Invocation.Usage", string.Empty, string.Empty);
            return null;
        }

        if (verb == "-export" && !seen.Contains("to"))
        {
            usage = word("Invocation.QualifierMissing", verb, "-to");
            return null;
        }

        usage = string.Empty;
        bool? answer = seen.Contains("yes") ? true : seen.Contains("no") ? false : null;
        return new BInvocation(options, new BInvocationGlobal(answer, seen.Contains("quiet")));
    }

    internal async Task<int> BInvocationRun(
        CAtelier atelier, BConsole console, BLocalizationCatalog catalog, Func<string, bool> presence)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(catalog);

        BEnvoyStrict envoy = new(console, catalog, this, presence);
        foreach ((string name, string? value) in _bInvocationOptions)
        {
            switch (name)
            {
                case "help":
                    BInvocationHelpPrint(value, console, catalog);
                    break;
                case "version":
                    BInvocationVersionObserve(console, catalog);
                    break;
                case "workspace":
                    BInvocationWorkspaceObserve(value, atelier, envoy, console);
                    break;
                case "find":
                    BInvocationFindObserve(value!, BInvocationValueRead("format") ?? "text", atelier, envoy, console);
                    break;
                case "new":
                    BInvocationNewObserve(atelier, envoy, console);
                    break;
                case "delete":
                    BInvocationDeleteObserve(
                        long.Parse(value!, NumberStyles.None, CultureInfo.InvariantCulture), atelier, envoy);
                    break;
                case "import":
                    await BInvocationImportObserve(atelier, envoy, console);
                    break;
                case "export":
                    await BInvocationExportObserve(
                        long.Parse(value!, NumberStyles.None, CultureInfo.InvariantCulture), atelier, envoy, console);
                    break;
            }
        }

        return envoy.BEnvoyStrictRefusal ? 1 : 0;
    }

    internal string? BInvocationValueRead(string name)
    {
        return _bInvocationOptions.FirstOrDefault(option => option.Key == name).Value;
    }

    private static void BInvocationHelpPrint(string? verb, BConsole console, BLocalizationCatalog catalog)
    {
        string[] names =
        [
            "help", "version", "workspace", "find", "new", "delete", "import", "export",
            "headword", "language", "pronunciation", "note", "to", "as",
            "yes", "no", "quiet", "color", "format",
        ];
        int width = names.Max(static name => name.Length) + 3;
        if (verb is null)
        {
            console.BConsoleResultPrint(catalog.BLocalizationTextRead("Invocation.Help"));
        }

        foreach (string name in verb is null ? names : [verb])
        {
            string key = name switch
            {
                "help" => "Invocation.HelpHelp",
                "version" => "Invocation.HelpVersion",
                "workspace" => "Invocation.HelpWorkspace",
                "find" => "Invocation.HelpFind",
                "new" => "Invocation.HelpNew",
                "delete" => "Invocation.HelpDelete",
                "import" => "Invocation.HelpImport",
                "export" => "Invocation.HelpExport",
                "headword" => "Invocation.HelpHeadword",
                "language" => "Invocation.HelpLanguage",
                "pronunciation" => "Invocation.HelpPronunciation",
                "note" => "Invocation.HelpNote",
                "to" => "Invocation.HelpTo",
                "as" => "Invocation.HelpAs",
                "yes" => "Invocation.HelpYes",
                "no" => "Invocation.HelpNo",
                "quiet" => "Invocation.HelpQuiet",
                "color" => "Invocation.HelpColor",
                _ => "Invocation.HelpFormat",
            };
            console.BConsoleResultPrint(("-" + name).PadRight(width) + catalog.BLocalizationTextRead(key));
        }
    }

    private static void BInvocationVersionObserve(BConsole console, BLocalizationCatalog catalog)
    {
        BInvocationVersionPrint(CAtelier.CAtelierAboutRead(catalog.BLocalizationTextRead), console);
    }

    private static void BInvocationVersionPrint(IReadOnlyList<string> about, BConsole console)
    {
        console.BConsoleResultPrint(string.Join(' ', about));
    }

    private static void BInvocationWorkspaceObserve(
        string? path, CAtelier atelier, BEnvoyStrict envoy, BConsole console)
    {
        BInvocationWorkspacePrint(
            path is null ? atelier.CAtelierPathRead() : atelier.CAtelierWorkspace.CWorkspaceChange(path, envoy),
            console);
    }

    private static void BInvocationWorkspacePrint(string path, BConsole console)
    {
        console.BConsoleResultPrint(path);
    }

    private static void BInvocationFindObserve(
        string query, string format, CAtelier atelier, BEnvoyStrict envoy, BConsole console)
    {
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => false, envoy, BObserver.BObserverCreate<Action>(static run => run()));
        CAperture aperture = library.CLibraryPanel.CPanelAperture;
        BInvocationFindAttach(aperture, library, format, console);
        aperture.CApertureQuerySet(query);
    }

    private static void BInvocationFindAttach(CAperture aperture, CLibrary library, string format, BConsole console)
    {
        aperture.CApertureRowsChanged += () => BInvocationFindPrint(library, format, console);
    }

    private static void BInvocationFindPrint(CLibrary library, string format, BConsole console)
    {
        IReadOnlyList<CVistaRow> rows = library.CLibraryRowsRead();
        string[][] lines = rows
            .Select(static row => new[]
            {
                row.CVistaRowId.ToString(CultureInfo.InvariantCulture),
                row.CVistaRowHeadword,
                row.CVistaRowLanguage,
                row.CVistaRowEpithet,
            })
            .ToArray();
        if (lines.Length == 0)
        {
            return;
        }

        if (format == "json")
        {
            ArrayBufferWriter<byte> buffer = new();
            JsonWriterOptions options = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            using (Utf8JsonWriter writer = new(buffer, options))
            {
                writer.WriteStartArray();
                foreach (CVistaRow row in rows)
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("id", row.CVistaRowId);
                    writer.WriteString("headword", row.CVistaRowHeadword);
                    writer.WriteString("language", row.CVistaRowLanguage);
                    writer.WriteString("epithet", row.CVistaRowEpithet);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }

            console.BConsoleResultPrint(Encoding.UTF8.GetString(buffer.WrittenSpan));
            return;
        }

        if (format == "tsv")
        {
            foreach (string[] line in lines)
            {
                console.BConsoleResultPrint(string.Join('\t', line));
            }

            return;
        }

        BCanvasWidth width = new(false);
        int[] columns =
            [.. Enumerable.Range(0, 3).Select(column => lines.Max(line => width.BCanvasWidthResolve(line[column])))];
        foreach (string[] line in lines)
        {
            StringBuilder text = new();
            for (int column = 0; column < columns.Length; column++)
            {
                text.Append(line[column]).Append(' ', columns[column] - width.BCanvasWidthResolve(line[column]) + 2);
            }

            console.BConsoleResultPrint(text.Append(line[^1]).ToString().TrimEnd(' '));
        }
    }

    private void BInvocationNewObserve(CAtelier atelier, BEnvoyStrict envoy, BConsole console)
    {
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => false, envoy, BObserver.BObserverCreate<Action>(static run => run()));
        BInvocationNewAttach(library.CLibraryEditor.CEditorDesk, console);
        bool created = library.CLibraryEntryCreate(
            BInvocationValueRead("headword"),
            BInvocationValueRead("language"),
            BInvocationValueRead("pronunciation"),
            BInvocationValueRead("note"));
        if (!created && !envoy.BEnvoyStrictRefusal)
        {
            envoy.CEnvoyFailureShow("Invocation.NewEmpty");
        }
    }

    private static void BInvocationNewAttach(CDesk desk, BConsole console)
    {
        desk.CDeskFinished += id => BInvocationNewPrint(id, console);
    }

    private static void BInvocationNewPrint(long id, BConsole console)
    {
        console.BConsoleResultPrint(id.ToString(CultureInfo.InvariantCulture));
    }

    private static void BInvocationDeleteObserve(long id, CAtelier atelier, BEnvoyStrict envoy)
    {
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => false, envoy, BObserver.BObserverCreate<Action>(static run => run()));
        bool deleted = library.CLibraryPanel.CPanelBin.CPanelBinDelete(id);
        if (!deleted && !envoy.BEnvoyStrictRefusal)
        {
            envoy.CEnvoyFailureShow("Invocation.EntryMissing");
        }
    }

    private static async Task BInvocationImportObserve(CAtelier atelier, BEnvoyStrict envoy, BConsole console)
    {
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => false, envoy, BObserver.BObserverCreate<Action>(static run => run()));
        if (await library.CLibraryMarkupImport() is int count)
        {
            BInvocationImportPrint(count, console);
        }
    }

    private static void BInvocationImportPrint(int count, BConsole console)
    {
        console.BConsoleResultPrint(count.ToString(CultureInfo.InvariantCulture));
    }

    private static async Task BInvocationExportObserve(
        long id, CAtelier atelier, BEnvoyStrict envoy, BConsole console)
    {
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => false, envoy, BObserver.BObserverCreate<Action>(static run => run()));
        if (await library.CLibraryEntryExport(id) is string path)
        {
            BInvocationExportPrint(path, console);
        }
        else if (!envoy.BEnvoyStrictRefusal)
        {
            envoy.CEnvoyFailureShow("Invocation.EntryMissing");
        }
    }

    private static void BInvocationExportPrint(string path, BConsole console)
    {
        console.BConsoleResultPrint(path);
    }
}
