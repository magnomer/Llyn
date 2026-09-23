using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

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
        LLocalization.LLocalizationTextRead(key);

    internal static string? TLocalizationTextFind(string key) =>
        LLocalization.LLocalizationTextFind(key);

    internal static string TLocalizationNormalize(string? language) =>
        LLocalization.LLocalizationNormalize(language);

    internal static bool TLocalizationDefaultCheck(string? language) =>
        LLocalization.LLocalizationDefaultCheck(language);

    internal static IReadOnlyDictionary<string, string> TThemeColorRead() =>
        LThemeLoader.LThemeLoaderLoad().LThemeColor;

    internal static string TThemeColorRead(string name) =>
        LThemeLoader.LThemeLoaderLoad().LThemeColorRead(name);

    internal static LUsher TUsherFileCreate() =>
        new LUsherFile();

    internal static bool TUsherPathExist(this LUsher usher, string? path) =>
        usher.LUsherPathExist(path);

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
        this LEnsign ensign, int age, IReadOnlyList<string> keys, IReadOnlyList<string?> paths) =>
        ensign.LEnsignPathAdd(age, keys, paths);

    internal static LEnsignRow TEnsignRowCreate(string key, string path) =>
        new(key, path);

    internal static string TEnsignKeyFormat(string language, string variety) =>
        LEnsign.LEnsignKeyFormat(language, variety);

    internal static string? TEnsignPathRead(this LEnsign ensign, string key) =>
        ensign.LEnsignPathRead(key);

    internal static void TEnsignPathDelete(this LEnsign ensign, string path)
    {
        ensign.LEnsignPathDelete(path);
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

    internal static LMentionResult TMentionResultCreate(
        int offset, int length, LMention? stored, IReadOnlyList<LTranslationTarget> targets) =>
        new(offset, length, stored, targets);

    internal static LTranslationTarget TTranslationTargetCreate(long id, string headword, string language) =>
        new(id, headword, language);

    internal static bool TPronunciationDraftMatch(this LPronunciationDraft spoken, string variety) =>
        spoken.LPronunciationDraftMatch(variety);

    internal static bool TStateValueMatch(this LStateValue value, string text) =>
        value.LStateValueMatch(text);

    internal static bool TStateAnchorMatch(this LStateAnchor anchor, long id) =>
        anchor.LStateAnchorMatch(id);

    internal static LStateAnchor TStateAnchorCreate(long id) =>
        LStateAnchor.LStateAnchorCreate(id);

    internal static LStateValue TStateUnreadableCreate(string text) =>
        new(LState.LStateSpecified, text, true);

    internal static IReadOnlyList<LParadigmRow> TParadigmRowScan(IReadOnlyList<LParadigmSlot> slots) =>
        LParadigmRow.LParadigmRowScan(slots);

    internal static LParadigmSlot TParadigmSlotCreate(
        LSpeechValue speech, LMorphology morphology, LInflection? inflection, LState state) =>
        new(speech, morphology, inflection, state, new LParadigm(speech.LSpeechValueId, []));

    internal static string TSentenceOrderFormat(this LSentenceOrder order, string particle, string dependence) =>
        order.LSentenceOrderFormat(particle, dependence);

    internal static string TSentenceOrderFormat(
        this LSentenceOrder order, string particle, string dependence, string text) =>
        order.LSentenceOrderFormat(particle, dependence, text);

    internal static LSentenceOrder TSentenceOrderCreate(int particle, int dependence) =>
        new(particle, dependence);

    internal static LFrequency TFrequencyUnitCreate(string source, string raw, string? band, string unit) =>
        new(source, raw, band, null, unit);

}
