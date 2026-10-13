using System.Text.Json;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    static TInterface()
    {
        LLocalization.LLocalizationLanguageSet(new LLocalizationLoader().LLocalizationScan());
    }

    internal static IReadOnlyDictionary<string, string> TLocalizationLoad(string json, string language) =>
        LLocalization.LLocalizationLoad(LLocalizationLoader.LLocalizationLoaderParse(json), language);

    internal static IReadOnlyDictionary<string, string> TLocalizationLoad(string language) =>
        LLocalization.LLocalizationLoad(new LLocalizationLoader(), language);

    internal static IReadOnlyDictionary<string, string> TLocalizationLoaderRead(string language) =>
        new LLocalizationLoader().LLocalizationRead(language);

    internal static IReadOnlyList<string> TLocalizationScan() =>
        new LLocalizationLoader().LLocalizationScan();

    internal static string TLocalizationTextRead(string key) =>
        LLocalization.QLocalizationTextRead(key);

    internal static string? TLocalizationTextFind(string key) =>
        LLocalization.QLocalizationTextFind(key);

    internal static IReadOnlyList<string> TLocalizationGroupFind(
        IReadOnlyList<(string, IReadOnlyList<string>)> groups, string? text) =>
        LLocalization.LLocalizationGroupFind(groups, text);

    internal static string TLocalizationNormalize(string? language) =>
        LLocalization.LLocalizationNormalize(language);

    internal static bool TLocalizationDefaultCheck(string? language) =>
        LLocalization.LLocalizationDefaultCheck(language);

    internal static IReadOnlyDictionary<string, string> TThemeColorRead()
    {
        LTheme theme = LThemeLoader.LThemeLoaderLoad();
        using Stream stream = typeof(LThemeLoader).Assembly
            .GetManifestResourceStream("Llyn.Infrastructure.Themes.default.json")!;
        using JsonDocument document = JsonDocument.Parse(stream);
        Dictionary<string, string> colors = new(StringComparer.Ordinal);
        foreach (JsonProperty color in document.RootElement.GetProperty("colors").EnumerateObject())
        {
            colors[color.Name] = theme.LThemeColorRead(color.Name);
        }

        return colors;
    }

    internal static string TThemeColorRead(string name) =>
        LThemeLoader.LThemeLoaderLoad().LThemeColorRead(name);

    internal static LUsher TUsherFileCreate() =>
        new LUsherFile();

    internal static bool TUsherPathExist(this LUsher usher, string? path) =>
        usher.LUsherPathExist(path);

    internal static bool TUsherLockCheck(this LUsher usher, Exception exception) =>
        usher.LUsherLockCheck(exception);

    internal static void TUsherPathDelete(this LUsher usher, string path)
    {
        usher.LUsherPathDelete(path);
    }

    internal static LEnsign TEnsignCreate(LUsher usher) =>
        new(usher);

    internal static void TEnsignClear(this LEnsign ensign)
    {
        ensign.LEnsignClear();
    }

    internal static string[] TEnsignMissingRead(this LEnsign ensign, IEnumerable<string> keys, out int age) =>
        ensign.LEnsignMissingRead(keys, out age);

    internal static IReadOnlyList<LEnsignRow> TEnsignPathAdd(
        this LEnsign ensign, int age, IReadOnlyList<string> keys, IReadOnlyList<string?> paths)
    {
        List<LEnsignRow> kept = [];
        ensign.LEnsignPathAdd(age, keys, paths, (rows, _) => () => kept.AddRange(rows));
        return kept;
    }

    internal static IReadOnlyList<LEnsignRow> TEnsignClearAdd(
        this LEnsign ensign, int age, IReadOnlyList<string> keys, IReadOnlyList<string?> paths)
    {
        List<LEnsignRow> kept = [];
        ensign.LEnsignPathAdd(age, keys, paths, (rows, _) =>
        {
            ensign.LEnsignClear();
            return () => kept.AddRange(rows);
        });
        return kept;
    }

    internal static LEnsignRow TEnsignRowCreate(string key, string path) =>
        new(key, path);

    internal static string TEnsignKeyFormat(string language, string variety) =>
        LEnsign.LEnsignKeyFormat(language, variety);

    internal static string TEngineEnsignFormat(string language, string variety) =>
        LSettingsPort.LEngineEnsignFormat(language, variety);

    internal static void TEnsignPathDelete(this LEnsign ensign, string path, Exception exception)
    {
        ensign.LEnsignPathDelete(path, exception);
    }

    internal static LFanqieRow TFanqieRowCreate(
        string character,
        string book,
        string text,
        string initial = "",
        string rime = "",
        string heading = "",
        string division = "",
        string tone = "",
        bool rounded = false,
        string reading = "",
        string toneClass = "",
        long id = 0,
        int rank = 0) =>
        new(
            character, book, 0, text, initial, rime, heading, division, tone, rounded,
            LFanqieRowReading: reading, LFanqieRowClass: toneClass, LFanqieRowId: id,
            LFanqieRowRepresentative: rank);

    internal static LFanqieRow TFanqieRowFormat(this LFanqieRow row, string pattern) =>
        row.LFanqieRowFormat(pattern);

    internal static LFanqieBook TFanqieBookCreate(string name, string source) =>
        new(name, string.Empty, new Dictionary<string, string>(), string.Empty, LFanqieBookSource: source);

    internal static IReadOnlyList<LFanqieGroup> TFanqieGroupScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<LFanqieBook> books) =>
        LFanqieGroup.LFanqieGroupScan(rows, books);

    internal static IReadOnlyList<LFanqieRow> TFanqieRowSort(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<LFanqieBook> books) =>
        LFanqieRow.LFanqieRowSort(rows, books);

    internal static string TFanqieReadingFormat(IReadOnlyList<LFanqieGroup> groups, string headword) =>
        LFanqieGroup.LFanqieReadingFormat(groups, headword);

    internal static LScriptStyle TScriptStyleCreate(string name) =>
        new(name, string.Empty, new Dictionary<string, string>(), string.Empty, 0, 0);

    internal static LScriptImage TScriptImageCreate(string character, string style, string gloss) =>
        new(character, style, 0, string.Empty, gloss, []);

    internal static IReadOnlyList<LScriptGroup> TScriptGroupScan(
        IReadOnlyList<LScriptImage> images, IReadOnlyList<LScriptStyle> styles) =>
        LScriptGroup.LScriptGroupScan(images, styles);

    internal static IReadOnlyList<LEpoch> TEpochTableRead(LLanguage pack)
    {
        foreach (LScriptStyle style in pack.LLanguageScripts)
        {
            if (style.LScriptStyleEpoch.Count > 0)
            {
                return style.LScriptStyleEpoch;
            }
        }

        return [];
    }

    internal static (string LEpochFound, string LEpochCaption) TEpochResolve(
        IReadOnlyList<LEpoch> epochs, string caption) =>
        LEpoch.LEpochResolve(epochs, caption);

    internal static LStateAnchor TStateAnchorCreate(long id) =>
        LStateAnchor.LStateAnchorCreate(id);

    internal static LStateValue TStateUnreadableCreate(string text) =>
        new(LState.LStateSpecified, text, true);

    internal static IReadOnlyList<LParadigmRow> TParadigmRowScan(IReadOnlyList<LParadigmSlot> slots) =>
        LParadigmRow.LParadigmRowScan(slots);

    internal static LParadigmStatus TParadigmSlotCheck(LParadigmRow row, bool pending, bool enabled) =>
        row.LParadigmRowFirst.LParadigmSlotCheck(pending, enabled);

    internal static LParadigmSlot TParadigmSlotCreate(
        LSpeechValue speech, LMorphology morphology, LInflection? inflection, LState state) =>
        new(speech, morphology, inflection, state, new LParadigm(speech.LSpeechValueId, []));

    internal static LSentenceOrder TSentenceOrderCreate(int particle, int dependence) =>
        new(particle, dependence);

    internal static LFrequency TFrequencyUnitCreate(string source, string raw, string? band, string unit) =>
        new(source, raw, band, null, unit);

    internal static LEpoch TEpochCreate(string label, string code) =>
        new(label, code);

    internal static LScriptImage TScriptImageCreate(
        string character, string style, int position, string caption, string epoch, long id = 0) =>
        new(character, style, position, caption, string.Empty, [], epoch, id);

    internal static IReadOnlyList<LScriptImage> TScriptImageSort(
        IReadOnlyList<LScriptImage> images, IReadOnlyList<LScriptStyle> styles, IReadOnlyList<string> spelled) =>
        LScriptImage.LScriptImageSort(images, styles, spelled);
}
