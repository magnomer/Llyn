# CColophon.cs
Hash: `50235a38c806148e`

## `public sealed record CColophon(string CColophonTitle, bool CColophonTitleFaint, string CColophonKind, bool CColophonKindShown, string CColophonYear, bool CColophonYearFaint, bool CColophonYearShown, string CColophonUrl, bool CColophonUrlFaint, bool CColophonUrlShown, string CColophonNote, bool CColophonNoteFaint, bool CColophonNoteShown, string CColophonAuthor, bool CColophonAuthorFaint, bool CColophonAuthorShown, string CColophonTally)`

The sheet of a reference, as the colophon panel shows it.
The engine decides every faint and shown flag, so the panel only places them.

**Parameters**

- `CColophonTitle`: the title.
- `CColophonTitleFaint`: whether the title shows as a placeholder.
- `CColophonKind`: the kind of reference.
- `CColophonKindShown`: whether the kind chip shows.
- `CColophonYear`: the year.
- `CColophonYearFaint`: whether the year shows as a placeholder.
- `CColophonYearShown`: whether the year section shows.
- `CColophonUrl`: the address.
- `CColophonUrlFaint`: whether the address shows as a placeholder.
- `CColophonUrlShown`: whether the address section shows.
- `CColophonNote`: the note.
- `CColophonNoteFaint`: whether the note shows as a placeholder.
- `CColophonNoteShown`: whether the note section shows.
- `CColophonAuthor`: the authors.
- `CColophonAuthorFaint`: whether the authors show as a placeholder.
- `CColophonAuthorShown`: whether the author section shows.
- `CColophonTally`: how many examples cite the reference.
