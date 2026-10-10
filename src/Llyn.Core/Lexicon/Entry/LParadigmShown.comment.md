# LParadigmShown.cs
Hash: `5a765c283fe4bad6`

## `public sealed record LParadigmShown(string LParadigmShownText, string? LParadigmShownTip)`

What one paradigm cell shows for its status, with no user present.
It lives in Core so the app box, the slot list and the Joplin note read one answer.

**Parameters**

- `LParadigmShownText`: the form, or the mark that stands in for a missing one.
- `LParadigmShownTip`: the localization key of the tip, or null when the cell shows its form.

## `public static LParadigmShown LParadigmShownResolve(LParadigmStatus status, string text, bool held)`

The one owner of the status wording.
A text status keeps `text` and has no tip.
An unknown status shows a dash, and a pending, lost or absent status shows an ellipsis.
Only a lost status reads `held`, choosing the held key over the lost key.
The editor passes `held` on, since its draft holds the missing forms.
An unrecognized status throws rather than showing a placeholder.
