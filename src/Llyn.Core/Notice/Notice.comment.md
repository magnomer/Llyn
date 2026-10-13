# Notice

Notice holds the announcements that stored data changed.
A subscriber decides what to re-read from the kind alone.
It never has to know which panel or call made the change.

## `LBulletin`

One announcement that stored data changed, naming the kind of record and the id of the one that changed.
It carries no content, because a subscriber re-reads what it needs through the engine.
It names no origin, because a subscriber that acted on its own change is announced to like any other.
An id of zero or less means the kind changed as a whole rather than one named record.
That is an import, a rename across cards, or the workspace moving.

## `LSubject`

Which kind of stored record a bulletin announces a change to.
`LSubjectWorkspace` is the whole store changing at once, which makes every stored kind a subscriber holds stale.
`LSubjectEntry`, `LSubjectExample`, `LSubjectSituation`, `LSubjectReference` and `LSubjectAuthor` announce store, change, delete, attach and detach.
`LSubjectTag` announces a Tag written onto a card, renamed or deleted.
`LSubjectRegister` announces a creation, and the id is the new Register's.
`LSubjectFavorite` announces an Entry marked or unmarked.
`LSubjectDraft` announces a held Draft changed by a Request.
The surface holding that draft re-reads it and renders what differs.
A draft id of zero means a held draft was cancelled or deleted, so the set shrank.
No surface holds draft zero, so only a surface counting the whole set acts on it.
`LSubjectFrequency`, `LSubjectGrasp`, `LSubjectInflection` and `LSubjectReflex` carry the entry's id.
The surface showing that entry re-reads that one fact alone.
`LSubjectScript` and `LSubjectFanqie` carry the id of the entry whose display asked for the character.
A surface showing any entry of that language re-reads its box, since another entry may share the character.
`LSubjectSettings` carries id zero, because no record changed and every surface showing a reading re-reads it.
It covers a setting that shapes how records are shown, such as the respelling switch.
`LSubjectTenure` carries the draft the tenure holds, so the one panel holding it settles its buttons.
`LSubjectVista` carries the vista's own id, so the one panel holding it re-lists or re-marks its rows.
`LSubjectFold` carries the entry's id, and the fold changes no lexical data.
Subscribers may reshape a held draft, but no lexical reload is required by this subject.
`LSubjectStemFold` carries the series member's entry, and only the series page redraws.
It is apart from the plain fold, so entry and card folds leave the series page alone.

## Stamp

Commit: `c2f335c331fe`
- `LBulletin.cs` `2688781444381d03`
- `LSubject.cs` `2b599722433caba7`
