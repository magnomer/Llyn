# CReflex.cs
Hash: `226f6693a43e8ed7`

## `public sealed record CReflex(long CReflexId, string CReflexLanguage, string CReflexKind, string CReflexText, string CReflexRomanization, string CReflexMeaning, string CReflexNote, bool CReflexMain, string CReflexRegion, IReadOnlyList<long> CReflexAnchors, CRespellingMark CReflexMark, bool CReflexFolded, bool CReflexLead)`

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
`LReflexAnchorRead` formats the label with it for both panes, so the wording has one owner.

## `public string CReflexLanguageKey`

The localization key the row's language is labelled under.

## `public string CReflexKindKey`

The localization key the row's kind is labelled under.

## `public bool CReflexHiddenCheck(bool opened)`

Whether the row is hidden, true for a folded row while the fold is closed.

## `internal string LReflexFieldRead(CReflexField field)`

The text the row shows in the cell `field`.
`CKindred.CKindredSet` answers it for an edit it did not take.

## `internal static IReadOnlyList<bool> LReflexLeadRead(IReadOnlyList<string> languages)`

Whether each row opens a run of one language, so the language prints once per run.
The scan marks its rows by it.
`CKindred.CKindredSet` asks it again while a language is typed, since a row leads by position.

## `internal static CLecternAnchor LReflexAnchorRead(CEnvoy envoy, LSettingsPort settings, CLedgerNoticed noticed, LDraftPort drafts, long? entry, string headword, IReadOnlyList<CReflex> rows)`

Whether the headword offers anchoring for `entry`, and the anchor text of each row, keyed by its id.
The engine's entry-based anchor rule answers both, and the placements are joined by `LReflexSeparator`.
The reading view and the editor's reflex block both read their anchors here, so the map has one owner.
It lives with the rows it labels, so the sound sheet `CSounding` carries no draft port for it.
No stored entry or a refusal answers no anchor.
A refusal also shows `Display.AnchorFailed` through `envoy`, once through `noticed`, since both callers read on a repaint.
Each caller passes its own envoy and `settings`, as `CCatalog.LCatalogGlyphOpen` takes them.

## `internal static string LReflexKeyRead(string name)`

The key a reflex language or kind is labelled under, `Reflex.` plus the name.
A blank name keys nothing a catalog holds, so a driver prints it as it stands.
`CReflexTyped` keys a typed language or kind by the same rule, so a typed label matches a stored one.
