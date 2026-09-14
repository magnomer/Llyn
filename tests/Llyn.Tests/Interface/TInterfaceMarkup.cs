using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static IReadOnlyList<LMarkupEntry> TMarkupParse(string text) =>
        LMarkup.LMarkupParse(text);

    internal static IReadOnlyList<LMarkupEntry> TMarkupParse(
        string text, out IReadOnlyList<LMarkupOmission> omissions) =>
        LMarkup.LMarkupParse(text, out omissions);

    internal static string TMarkupFormat(IReadOnlyList<LMarkupEntry> entries) =>
        LMarkup.LMarkupFormat(entries);

    internal static LMarkupIntake TMarkupIntakeCreate(int index, LMarkupMode mode, long target = 0) =>
        new(index, mode, target);

    internal static LMarkupEntry TMarkupEntryCreate(
        string headword, string language, IReadOnlyList<LForm>? forms = null, string note = "") =>
        new(headword, language, LMarkupEntryForm: forms, LMarkupEntryNote: note);
}
