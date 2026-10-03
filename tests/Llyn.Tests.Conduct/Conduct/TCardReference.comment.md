# TCardReference.cs
Hash: `82668e15ab558208`

## `public sealed class TCardReference`

Covers the citation gate of a card sentence over an entry desk on a real workspace, with no delay.
A typed word offers the stored Sources split around the word, at most eight.
A blank or unmatched word offers nothing.
The byline of the Source the sentence already cites offers nothing, while another sentence still offers it.
A failed citation create shows `Reference.CreateFailed` and keeps the citation empty.
A failed Source read shows `Reference.LoadFailed` and answers none.
