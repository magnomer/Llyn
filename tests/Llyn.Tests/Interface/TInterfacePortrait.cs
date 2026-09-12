using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LPortraitLabel TPortraitLabelCreate(
        string unknown,
        string meaning,
        string meanings,
        string collocation,
        string collocations,
        string incoming,
        string note) =>
        new(unknown, meaning, meanings, collocation, collocations, incoming, note);

    internal static LPortraitLink TPortraitLinkCreate(long id, string headword, string language) =>
        new(id, headword, language);

    internal static LPortraitExample TPortraitExampleCreate(string frame, string text) =>
        new(frame, text);

    internal static LPortraitMedia TPortraitMediaCreate(
        string location, string span, bool moving) =>
        new(location, span, moving);

    internal static LPortraitUsage TPortraitUsageCreate(
        string headword, string title, string owner, string language) =>
        new(headword, title, owner, language);

    internal static LPortraitCard TPortraitCardCreate(
        int position,
        string title,
        string kind,
        string expression,
        string meaning,
        IReadOnlyList<string> situation,
        IReadOnlyList<string> register,
        IReadOnlyList<LPortraitLink> translation,
        IReadOnlyList<LPortraitExample> example,
        IReadOnlyList<string> tag,
        IReadOnlyList<LPortraitMedia> image,
        IReadOnlyList<LPortraitMedia> video) =>
        new(
            position,
            title,
            kind,
            expression,
            meaning,
            situation,
            register,
            translation,
            example,
            tag,
            image,
            video);

    internal static LPortrait TPortraitCreate(
        string headword,
        string language,
        IReadOnlyList<LPortraitReading> pronunciation,
        IReadOnlyList<LPortraitReading> transcription,
        IReadOnlyList<string> speech,
        IReadOnlyList<LPortraitCard> meaning,
        IReadOnlyList<LPortraitCard> collocation,
        IReadOnlyList<LPortraitUsage> incoming,
        string note,
        bool favorite,
        LPortraitLabel label) =>
        new(
            headword,
            language,
            pronunciation,
            transcription,
            speech,
            meaning,
            collocation,
            incoming,
            note,
            favorite,
            label);

    internal static LPortraitReading TPortraitReadingCreate(string label, string text) => new(label, text);

    internal static LTheme TThemeLoad() => LTheme.LThemeLoad();

    internal static string TThemeRead(this LTheme theme, string name) => theme.LThemeRead(name);

    internal static string TSheetFormat(LPortrait portrait, LTheme theme) =>
        LSheet.LSheetFormat(portrait, theme);

    internal static string TOutlineFormat(LPortrait portrait) => LOutline.LOutlineFormat(portrait);

    internal static string TMarkdownNormalize(string? text) => LMarkdown.LMarkdownNormalize(text);

    internal static IReadOnlyList<LMarkdownBlock> TMarkdownParse(string? text) =>
        LMarkdown.LMarkdownParse(text);

    internal static void TFolioSave(LPortrait portrait, LTheme theme, string path)
    {
        LFolio.LFolioSave(portrait, theme, path);
    }
}
