# LMarkupDraft.cs

## `public static class LMarkupDraft`

Writes one stored entry back out as Llyn Markup.
It is the inverse of `LMarkup`, and the two must agree tag for tag.
Markup is the only export that keeps everything, including what the display never shows.

## `public static string LMarkupDraftFormat(LEntryDraft draft, IReadOnlyList<LMarkup.LMarkupReference> sources)`

Sources are declared before the cards, because a citation may only name a source already declared.
A source id is the stored row id, so a file cites what it declares and nothing outside itself.
An unset field is omitted rather than written empty, since an empty element means unreadable.
