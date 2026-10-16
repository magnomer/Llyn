using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                "help" or "version" or "workspace" => true,
                "yes" or "no" or "quiet" or "color" or "format" => false,
                _ => null,
            };
            string? fault = (name, value) switch
            {
                _ when isVerb is null => "Invocation.Unknown",
                ("version" or "yes" or "no" or "quiet", not null) => "Invocation.ValueForbidden",
                ("color" or "format", null) => "Invocation.ValueMissing",
                (_, "") => "Invocation.ValueMissing",
                ("color", not ("auto" or "always" or "never")) => "Invocation.ValueUnknown",
                ("format", not ("text" or "tsv" or "json" or "markup" or "html" or "markdown" or "docx" or "pdf"))
                    => "Invocation.ValueUnknown",
                ("help", not (null or "help" or "version" or "workspace")) => "Invocation.ValueUnknown",
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

        usage = string.Empty;
        bool? answer = seen.Contains("yes") ? true : seen.Contains("no") ? false : null;
        return new BInvocation(options, new BInvocationGlobal(answer, seen.Contains("quiet")));
    }

    internal int BInvocationRun(CAtelier atelier, BConsole console, BLocalizationCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(catalog);

        BEnvoyStrict envoy = new(console, catalog, this);
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
        string[] names = ["help", "version", "workspace", "yes", "no", "quiet", "color", "format"];
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
        BInvocationVersionPrint(CAtelier.CAtelierAboutRead(), console, catalog);
    }

    private static void BInvocationVersionPrint(string key, BConsole console, BLocalizationCatalog catalog)
    {
        Version? version = typeof(BInvocation).Assembly.GetName().Version;
        string build = version is null ? string.Empty : version.ToString(3);
        console.BConsoleResultPrint(
            $"{catalog.BLocalizationTextRead("Terms.Product")} {catalog.BLocalizationTextRead(key)} {build}");
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
}
