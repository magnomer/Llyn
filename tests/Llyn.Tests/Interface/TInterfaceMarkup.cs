using System.Collections.Generic;
using System.IO;
using System.Text;
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

    internal const string TMarkupPair = """
        <llyn>
          <entry>
            <headword>ember</headword>
            <language>English</language>
            <meaning>
              <definition>a glowing coal</definition>
              <translation>
                <headword>braise</headword>
                <language>French</language>
              </translation>
            </meaning>
          </entry>
          <entry>
            <headword>braise</headword>
            <language>French</language>
            <meaning>
              <definition>ember</definition>
              <translation>
                <headword>ember</headword>
                <language>English</language>
              </translation>
            </meaning>
          </entry>
        </llyn>
        """;

    internal const string TMarkupLone = """
        <llyn>
          <entry>
            <headword>ember</headword>
            <language>English</language>
            <meaning>
              <definition>a glowing coal</definition>
              <translation>
                <headword>braise</headword>
                <language>French</language>
              </translation>
            </meaning>
          </entry>
        </llyn>
        """;

    internal static string TMarkupSave(TWorkspace workspace, string text)
    {
        string path = Path.Combine(workspace.TWorkspaceFolder, "import.llx");
        File.WriteAllText(path, text, new UTF8Encoding(false));
        return path;
    }

    internal static LCardDraft TCardCreate(string definition, int position)
    {
        return TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate(definition),
            [], [], [], [], [], position);
    }
}
