# TCardTranslation.cs

## `public sealed class TCardTranslation`

Covers the translation gates of a card over an entry desk on a real workspace, with no delay.
A typed list links each resolved word and keeps the unresolved beside the rest.
A typed word answers the Entries it matches with none chosen, and a blank one offers nothing.
Enter answers the same dropdown with the whole match chosen, and links nothing.
Leaving the field links the whole match and empties the entry, and keeps a word nothing answers.
A padded word finds what the trimmed word finds, since the engine trims it.
A fresh row starts a court under the owner draft's origin.
Its removal drops the link, the court and the court's draft.
A stored Entry removed from a card leaves the card and stays findable.
A fresh row with a blank word shows the translation failure and links nothing.
