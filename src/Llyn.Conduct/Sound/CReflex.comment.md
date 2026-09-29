# CReflex.cs

## `public sealed record CReflex(`

One reflex row of an entry, ready to show in the editor or the reading view.
Conduct resolved its text and its mark, so no driver asks the pack about the row.

**Parameters**

- `CReflexId`: the stored reflex, zero for a fresh one.
- `CReflexLanguage`: the language the reflex belongs to, as stored.
- `CReflexKind`: the kind of reflex, as stored.
- `CReflexText`: the form the mark picks, the respelling or the phonetic text.
- `CReflexRomanization`: the romanization.
- `CReflexMeaning`: the meaning in that language.
- `CReflexNote`: the note.
- `CReflexMain`: whether the reflex is the language's main one.
- `CReflexRegion`: the region the reflex is heard in.
- `CReflexAnchors`: the fanqie readings the reflex anchors to.
- `CReflexMark`: whether the text is the respelling, and the slashes around it.
- `CReflexFolded`: whether the entry's pack folds the row's language away.
- `CReflexLead`: whether the row opens a run of one language, so it alone prints the language.

## `internal const string LReflexSeparator = " · ";`

The mark between two anchored placements in a row's anchor label.
Both panes format the label with it, so the wording has one owner.

## `public string CReflexLanguageKey`

The localization key the row's language is labelled under.

## `public string CReflexKindKey`

The localization key the row's kind is labelled under.

## `public bool CReflexHiddenCheck(bool opened)`

Whether the row is hidden, true for a folded row while the fold is closed.

## `internal static IReadOnlyList<bool> LReflexLeadRead(IReadOnlyList<string> languages)`

Whether each row opens a run of one language, so the language prints once per run.
The scan marks its rows by it.
`CEditor.CEditorLeadRead` asks it again while a language is typed, since a row leads by position.

## `private static string LReflexKeyRead(string name)`

The key a reflex language or kind is labelled under, `Reflex.` plus the name.
A blank name keys nothing a catalog holds, so a driver prints it as it stands.
