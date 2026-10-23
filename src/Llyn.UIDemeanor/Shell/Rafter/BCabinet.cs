using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal static class BCabinet
{
    internal static BRoofMenu BCabinetBuild(
        BLocalizationCatalog catalog,
        IReadOnlyDictionary<string, IReadOnlyList<string>> marks,
        string? theme,
        string glyphs,
        string ambiguous,
        CLedgerState? ledger,
        Action<string> chooseTheme,
        Action<string> chooseGlyphs,
        Action<string> chooseAmbiguous,
        CEnvoy envoy,
        Action<string, CEnvoy> save,
        Func<string, bool> select,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(marks);
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(ambiguous);
        ArgumentNullException.ThrowIfNull(chooseTheme);
        ArgumentNullException.ThrowIfNull(chooseGlyphs);
        ArgumentNullException.ThrowIfNull(chooseAmbiguous);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(save);
        ArgumentNullException.ThrowIfNull(select);
        ArgumentNullException.ThrowIfNull(measure);

        Func<string, ConsoleKey?> letter = key => catalog.BLocalizationTextRead(key) is [char single]
            && char.IsAsciiLetter(single)
                ? ConsoleKey.A + (char.ToUpperInvariant(single) - 'A')
                : null;
        string bullet = marks.GetValueOrDefault("Radio") is [string radio] ? radio : "*";
        string blank = new(' ', measure.BCanvasWidthResolve(bullet));
        Func<IEnumerable<KeyValuePair<string, string>>, string?, Action<string>, BRoofItem?[]> choices =
            (pairs, current, choose) =>
            [
                .. pairs.Select(pair => new BRoofItem(
                    $"{(pair.Key == current ? bullet : blank)} {pair.Value}",
                    null,
                    null,
                    () => choose(pair.Key),
                    static () => true)),
            ];
        Func<string, Action, Func<bool>, IReadOnlyList<BRoofItem?>?, BRoofItem> item = (key, action, live, submenu) =>
            new(catalog.BLocalizationTextRead(key), null, letter($"{key}.Hotkey"), action, live, submenu);
        KeyValuePair<string, string>[] themes =
        [
            new(BTheme.BThemeDefault, catalog.BLocalizationTextRead("Roof.Options.Theme.Blue")),
            new("console-dark", catalog.BLocalizationTextRead("Roof.Options.Theme.Dark")),
        ];
        KeyValuePair<string, string>[] sets =
        [
            new("double", catalog.BLocalizationTextRead("Roof.Options.Glyphs.Double")),
            new("single", catalog.BLocalizationTextRead("Roof.Options.Glyphs.Single")),
            new("ascii", catalog.BLocalizationTextRead("Roof.Options.Glyphs.Ascii")),
        ];
        KeyValuePair<string, string>[] widths =
        [
            new("narrow", catalog.BLocalizationTextRead("Roof.Options.Ambiguous.Narrow")),
            new("wide", catalog.BLocalizationTextRead("Roof.Options.Ambiguous.Wide")),
        ];
        BRoofItem?[] languages = choices(
            ledger?.CLedgerStateLanguages ?? [], ledger?.CLedgerStateLocalization, language => save(language, envoy));

        return new BRoofMenu(
            catalog.BLocalizationTextRead("Roof.Options"),
            letter("Roof.Options.Hotkey"),
            [
                item("Roof.Options.Theme", static () => { }, static () => true, choices(themes, theme, chooseTheme)),
                item("Roof.Options.Glyphs", static () => { }, static () => true, choices(sets, glyphs, chooseGlyphs)),
                item(
                    "Roof.Options.Ambiguous",
                    static () => { },
                    static () => true,
                    choices(widths, ambiguous, chooseAmbiguous)),
                null,
                item("Roof.Options.Language", static () => { }, () => ledger is not null, languages),
                item("Roof.Options.Settings", () => select("Settings"), static () => true, null),
            ]);
    }
}
