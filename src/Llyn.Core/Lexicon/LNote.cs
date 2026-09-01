namespace Llyn.Core;

/// <summary>
/// The single Note an entry owns: subordinate free text hanging from the entry itself, with no id and no
/// order. An entry carries <em>at most one</em> Note — the owning entry id is the identity — so saving a
/// Note replaces whatever Note the entry had.
/// <para>
/// TODO: <c>PNoteContents</c> edits WYSIWYG Markdown, but the serialized format is not yet prescribed.
/// <see cref="LNoteText"/> therefore stores whatever the editor produced, as-is; pin the format down and
/// normalize on write once it is decided.
/// </para>
/// </summary>
/// <param name="LNoteEntryId">Owning entry id — the Note's identity, one per entry.</param>
/// <param name="LNoteText">The note text, stored exactly as the editor produced it.</param>
public sealed record LNote(
    string LNoteEntryId,
    string LNoteText);
